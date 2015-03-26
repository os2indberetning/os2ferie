using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web.OData;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Ninject;
using OS2Indberetning;


namespace Core.ApplicationServices
{
    public class DriveReportService : IDriveReportService
    {
        private readonly IRoute<RouteInformation> _route;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<DriveReport> _driveReportRepository;
        private readonly IReimbursementCalculator _calculator;
        private readonly IGenericRepository<OrgUnit> _orgUnitRepository;
        private readonly IGenericRepository<Employment> _employmentRepository;
        private readonly IGenericRepository<Substitute> _substituteRepository;
        private readonly IMailSender _mailSender;

        public DriveReportService(IMailSender mailSender, IGenericRepository<DriveReport> driveReportRepository, IReimbursementCalculator calculator, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository)
        {
            _route = new BestRoute();
            _coordinates = new AddressCoordinates();
            _calculator = calculator;
            _orgUnitRepository = orgUnitRepository;
            _employmentRepository = employmentRepository;
            _substituteRepository = substituteRepository;
            _mailSender = mailSender;
            _driveReportRepository = driveReportRepository;
        }

        public IQueryable<DriveReport> AddFullName(IQueryable<DriveReport> repo)
        {
            var set = repo.ToList();

            // Add fullname to the resultset
            foreach (var driveReport in set)
            {
                AddFullName(driveReport);
            }
            return set.AsQueryable();
        }

        public void AddFullName(DriveReport driveReport)
        {
            if (driveReport == null)
            {
                return;
            }
            driveReport.Fullname = driveReport.Person.FirstName;

            if (!string.IsNullOrEmpty(driveReport.Person.MiddleName))
            {
                driveReport.Fullname += " " + driveReport.Person.MiddleName;
            }

            driveReport.Fullname += " " + driveReport.Person.LastName;
            driveReport.Fullname += " [" + driveReport.Person.Initials + "]";
        }

        public DriveReport Create(DriveReport report)
        {
            if (report.PersonId == 0)
            {
                throw new Exception("No person provided");
            }

            if (!Validate(report))
            {
                throw new Exception("DriveReport has some invalid parameters");
            }

            if (report.KilometerAllowance != KilometerAllowance.Read)
            {
                var pointsWithCoordinates =
                    report.DriveReportPoints.Select((t, i) => report.DriveReportPoints.ElementAt(i))
                        .Select(currentPoint => (DriveReportPoint)_coordinates.GetAddressCoordinates(currentPoint))
                        .ToList();

                report.DriveReportPoints = pointsWithCoordinates;

                var drivenRoute = _route.GetRoute(report.DriveReportPoints);

                report.Distance = (double)drivenRoute.Length / 1000;


            }


            report = _calculator.Calculate(report);




            var createdReport = _driveReportRepository.Insert(report);
            _driveReportRepository.Save();

            if (report.KilometerAllowance != KilometerAllowance.Read)
            {
                for (var i = 0; i < createdReport.DriveReportPoints.Count; i++)
                {
                    var currentPoint = createdReport.DriveReportPoints.ElementAt(i);

                    if (i == report.DriveReportPoints.Count - 1)
                    {
                        // last element   
                        currentPoint.PreviousPointId = createdReport.DriveReportPoints.ElementAt(i - 1).Id;
                    }
                    else if (i == 0)
                    {
                        // first element
                        currentPoint.NextPointId = createdReport.DriveReportPoints.ElementAt(i + 1).Id;
                    }
                    else
                    {
                        // between first and last
                        currentPoint.NextPointId = createdReport.DriveReportPoints.ElementAt(i + 1).Id;
                        currentPoint.PreviousPointId = createdReport.DriveReportPoints.ElementAt(i - 1).Id;
                    }
                }
                _driveReportRepository.Save();
            }





            return report;
        }

        private bool Validate(DriveReport report)
        {
            if (report.KilometerAllowance == KilometerAllowance.Read && report.Distance <= 0)
            {
                return false;
            }
            if (report.KilometerAllowance != KilometerAllowance.Read && report.DriveReportPoints.Count < 2)
            {
                return false;
            }
            if (string.IsNullOrEmpty(report.Purpose))
            {
                return false;
            }
            return true;
        }

        public void SendMailIfRejectedReport(int key, Delta<DriveReport> delta)
        {
            var status = new object();
            if (delta.TryGetPropertyValue("Status", out status) && status.ToString().Equals("Rejected"))
            {
                var recipient = _driveReportRepository.AsQueryable().First(r => r.Id == key).Person.Mail;
                var comment = new object();
                if (delta.TryGetPropertyValue("Comment", out comment))
                {
                    _mailSender.SendMail(recipient, "Afvist indberetning",
                        "Din indberetning er blevet afvist med kommentaren: \n \n" + comment);
                }
            }
        }

        public IQueryable<DriveReport> AttachResponsibleLeader(IQueryable<DriveReport> repo)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            foreach (var driveReport in repo)
            {
                var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == driveReport.Employment.OrgUnitId);

                if (orgUnit != null)
                {
                    var leaderEmpl = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnitId == orgUnit.Id && e.IsLeader);
                    if (leaderEmpl != null)
                    {
                        var leader = leaderEmpl.Person;
                        var sub = _substituteRepository.AsQueryable().SingleOrDefault(s => s.PersonId == leader.Id && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp);
                        if (sub != null)
                        {
                            // Attach sub if one exists.
                            driveReport.ResponsibleLeader = sub.Sub;
                            driveReport.ResponsibleLeader.FullName = sub.Sub.FirstName;
                            if (!string.IsNullOrEmpty(sub.Sub.MiddleName))
                            {
                                driveReport.ResponsibleLeader.FullName += " " + sub.Sub.MiddleName;
                            }
                            driveReport.ResponsibleLeader.FullName += " " + sub.Sub.LastName;
                            driveReport.ResponsibleLeader.FullName += " [" + sub.Sub.Initials + "]";
                        }
                        else
                        {
                            // Attach leader if no sub exists.
                            driveReport.ResponsibleLeader = leaderEmpl.Person;

                            driveReport.ResponsibleLeader.FullName = leaderEmpl.Person.FirstName;
                            if (!string.IsNullOrEmpty(leaderEmpl.Person.MiddleName))
                            {
                                driveReport.ResponsibleLeader.FullName += " " + leaderEmpl.Person.MiddleName;
                            }
                            driveReport.ResponsibleLeader.FullName += " " + leaderEmpl.Person.LastName;
                            driveReport.ResponsibleLeader.FullName += " [" + leaderEmpl.Person.Initials + "]";
                        }
                    }
                }
            }

            return repo;
        }

        public IQueryable<DriveReport> FilterByLeader(IQueryable<DriveReport> repo, int leaderId)
        {
            var result = new List<DriveReport>();

            var leaderEmpl = _employmentRepository.AsQueryable().Where(e => e.Person.Id == leaderId && e.IsLeader);
            if (leaderEmpl.Any())
            {
                // Iterate all employments belonging to the leader.
                foreach (var employment in leaderEmpl)
                {
                    // Get the orgunit of the empl.
                    var orgUnitId = employment.OrgUnit.Id;

                    // Get the leader of the childOrg if one exists.
                    var childOrg = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.ParentId == orgUnitId);
                    if (childOrg != null)
                    {
                        var childEmpl = _employmentRepository.AsQueryable().SingleOrDefault(e => e.IsLeader && e.OrgUnit.Id == childOrg.Id);
                        if (childEmpl != null)
                        {
                            // Get and add all reports belonging to the leader of the child org.
                            var childLeader = childEmpl.Person;
                            var childLeaderReports = repo.AsQueryable().Where(dr => dr.Person.Id == childLeader.Id);
                            result.AddRange(childLeaderReports);
                        }
                    }

                    var reports = repo.AsQueryable()
                        .Where(dr => dr.Employment.OrgUnit.Id == orgUnitId);

                    //TODO: Crashes here.
                    if (reports.Any())
                    {
                        result.AddRange(reports);
                    }

                }
            }

            return result.AsQueryable();
        }

    }
}
