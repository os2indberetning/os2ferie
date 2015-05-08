using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
using log4net;
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

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public void AddFullName(DriveReport driveReport)
        {
            if (driveReport == null)
            {
                return;
            }
            driveReport.FullName = driveReport.Person.FirstName;

            if (!string.IsNullOrEmpty(driveReport.Person.MiddleName))
            {
                driveReport.FullName += " " + driveReport.Person.MiddleName;
            }

            driveReport.FullName += " " + driveReport.Person.LastName;
            driveReport.FullName += " [" + driveReport.Person.Initials + "]";
        }

        public IQueryable<DriveReport> AddApprovedByFullName(IQueryable<DriveReport> repo)
        {
            foreach (var driveReport in repo.Where(driveReport => driveReport.ApprovedBy != null))
            {
                driveReport.ApprovedBy.FullName = driveReport.ApprovedBy.FirstName;

                if (!string.IsNullOrEmpty(driveReport.ApprovedBy.MiddleName))
                {
                    driveReport.ApprovedBy.FullName += " " + driveReport.ApprovedBy.MiddleName;
                }

                driveReport.ApprovedBy.FullName += " " + driveReport.ApprovedBy.LastName;

                driveReport.ApprovedBy.FullName += " [" + driveReport.ApprovedBy.Initials + "]";
            }
            return repo;
        }

        public DriveReport Create(DriveReport report)
        {
            if (report.PersonId == 0)
            {
                Logger.Info("Forsøg på at oprette indberetning uden person angivet.");
                throw new Exception("No person provided");
            }

            if (!Validate(report))
            {
                Logger.Info("Forsøg på at oprette indberetning med ugyldige parametre.");
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

                if (report.Distance < 0)
                {
                    report.Distance = 0;
                }
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



            //AddFullName(report);

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
            if (delta.TryGetPropertyValue("Status", out status))
            {
                if (status.ToString().Equals("Rejected"))
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
        }

        public IQueryable<DriveReport> AttachResponsibleLeader(IQueryable<DriveReport> repo)
        {
            var res = repo.ToList();
            foreach (var driveReport in res)
            {
                var responsibleLeader = GetResponsibleLeaderForReport(driveReport);

                if (responsibleLeader != null)
                {
                    SetResponsibleLeaderOnReport(driveReport, responsibleLeader);

                }
                else { 
                    //Indicate drivereports where we could not find a leader
                    SetResponsibleLeaderOnReport(driveReport, new Person()
                    {
                        FirstName = "Var ikke i stand til at finde godkendede leder",
                        LastName = "",
                        Initials = "FEJL"
                    });
                }
            }

            return res.AsQueryable();
        }

        public Person GetResponsibleLeaderForReport(DriveReport driveReport)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var person = driveReport.Person;

            //Fetch personal approver for the person (Person and Leader of the substitute is the same)
            var personalApprover =
                _substituteRepository.AsQueryable()
                    .SingleOrDefault(
                        s =>
                            s.PersonId != s.LeaderId && s.PersonId == person.PersonId &&
                            s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp);
            if (personalApprover != null)
            {
                return personalApprover.Sub;
            }

            //Find an org unit where the person is not the leader, and then find the leader of that org unit to attach to the drive report
            var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == driveReport.Employment.OrgUnitId);
            var leaderOfOrgUnit =
                _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader);

            if (leaderOfOrgUnit == null || orgUnit == null)
            {
                return null;
            }
            while (leaderOfOrgUnit.PersonId == person.Id)
            {
                orgUnit = orgUnit.Parent;
                leaderOfOrgUnit = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader);
                if (leaderOfOrgUnit == null)
                {
                    break;
                }
            }

            if (orgUnit != null)
            {
                var leaderEmpl = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnitId == orgUnit.Id && e.IsLeader);
                if (leaderEmpl != null)
                {
                    var leader = leaderEmpl.Person;
                    var sub = _substituteRepository.AsQueryable().SingleOrDefault(s => s.PersonId == leader.Id && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp);
                    if (sub != null)
                    {
                        return sub.Sub;
                    }
                    else
                    {
                        return leaderEmpl.Person;
                    }
                }

            }
            return null;
        }

        private void SetResponsibleLeaderOnReport(DriveReport driveReport, Person person)
        {
            driveReport.ResponsibleLeader = person;

            driveReport.ResponsibleLeader.FullName = person.FirstName;
            if (!string.IsNullOrEmpty(person.MiddleName))
            {
                driveReport.ResponsibleLeader.FullName += " " + person.MiddleName;
            }
            driveReport.ResponsibleLeader.FullName += " " + person.LastName;
            driveReport.ResponsibleLeader.FullName += " [" + person.Initials + "]";
        }

        public IQueryable<DriveReport> FilterByLeader(IQueryable<DriveReport> repo, int leaderId, bool getReportsWhereSubExists = false)
        {
            var result = new List<DriveReport>();

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var leaderEmpl = _employmentRepository.AsQueryable().Where(e => e.Person.Id == leaderId && e.IsLeader).ToList();
            if (leaderEmpl.Any())
            {
                // Iterate all employments belonging to the leader.
                foreach (var employment in leaderEmpl)
                {
                    // Get the orgunit of the empl.
                    var orgUnitId = employment.OrgUnit.Id;

                    if (getReportsWhereSubExists)
                    {
                        // Get the leader of the childOrg if one exists.
                        var childOrg = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.ParentId == orgUnitId);
                        if (childOrg != null)
                        {
                            var childEmpl = _employmentRepository.AsQueryable().SingleOrDefault(e => e.IsLeader && e.OrgUnit.Id == childOrg.Id);
                            if (childEmpl != null)
                            {
                                // Get and add all reports belonging to the leader of the child org.
                                var childLeader = childEmpl.Person;
                                var childLeaderReports = repo.AsQueryable().Where(dr => dr.Person.Id == childLeader.Id && dr.Employment.OrgUnit.Id == childOrg.Id);
                                result.AddRange(childLeaderReports);
                            }
                        }
                        result.AddRange(
                            repo.AsQueryable()
                                .Where(dr => dr.Employment.OrgUnit.Id == orgUnitId && dr.Person.Id != leaderId));
                    }
                    else
                    {
                        if (!(_substituteRepository.AsQueryable().Any(s => s.Person.Id == leaderId && s.StartDateTimestamp < currentTimestamp && s.EndDateTimestamp > currentTimestamp && s.OrgUnit.Id == orgUnitId)))
                        {
                            // Get the leader of the childOrg if one exists.
                            var childOrg = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.ParentId == orgUnitId);
                            if (childOrg != null)
                            {
                                var childEmpl = _employmentRepository.AsQueryable().SingleOrDefault(e => e.IsLeader && e.OrgUnit.Id == childOrg.Id);
                                if (childEmpl != null)
                                {
                                    // Get and add all reports belonging to the leader of the child org.
                                    var childLeader = childEmpl.Person;
                                    var childLeaderReports = repo.AsQueryable().Where(dr => dr.Person.Id == childLeader.Id && dr.Employment.OrgUnit.Id == childOrg.Id);
                                    result.AddRange(childLeaderReports);
                                }
                            }
                            result.AddRange(
                                repo.AsQueryable()
                                    .Where(dr => dr.Employment.OrgUnit.Id == orgUnitId && dr.Person.Id != leaderId));
                        }
                    }


                }
            }

            return result.AsQueryable();
        }

    }
}
