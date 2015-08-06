using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
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
        private readonly IGenericRepository<RateType> _rateTypeRepo;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<DriveReport> _driveReportRepository;
        private readonly IReimbursementCalculator _calculator;
        private readonly IGenericRepository<OrgUnit> _orgUnitRepository;
        private readonly IGenericRepository<Employment> _employmentRepository;
        private readonly IGenericRepository<Substitute> _substituteRepository;
        private readonly IMailSender _mailSender;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DriveReportService(IMailSender mailSender, IGenericRepository<DriveReport> driveReportRepository, IReimbursementCalculator calculator, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, IAddressCoordinates coordinates, IRoute<RouteInformation> route, IGenericRepository<RateType> rateTypeRepo)
        {
            _route = route;
            _rateTypeRepo = rateTypeRepo;
            _coordinates = coordinates;
            _calculator = calculator;
            _orgUnitRepository = orgUnitRepository;
            _employmentRepository = employmentRepository;
            _substituteRepository = substituteRepository;
            _mailSender = mailSender;
            _driveReportRepository = driveReportRepository;
        }

        /// <summary>
        /// Validates report and creates it in the database if it validates.
        /// </summary>
        /// <param name="report">Report to be created.</param>
        /// <returns>Created report.</returns>
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
                        .Select(currentPoint => (DriveReportPoint) _coordinates.GetAddressCoordinates(currentPoint))
                        .ToList();

                report.DriveReportPoints = pointsWithCoordinates;

                var isBike = _rateTypeRepo.AsQueryable().First(x => x.TFCode.Equals(report.TFCode)).IsBike;


                // Set transportType to Bike if isBike is true. Otherwise set it to Car.
                var drivenRoute = _route.GetRoute(
                    isBike ? DriveReportTransportType.Bike : DriveReportTransportType.Car, report.DriveReportPoints);


                report.Distance = (double) drivenRoute.Length/1000;

                if (report.Distance < 0)
                {
                    report.Distance = 0;
                }

                report = _calculator.Calculate(drivenRoute, report);
            }
            else
            {
                report = _calculator.Calculate(null, report);
            }




            // Round off Distance and AmountToReimburse to two decimals.
            report.Distance = Convert.ToDouble(report.Distance.ToString("0.##", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            report.AmountToReimburse = Convert.ToDouble(report.AmountToReimburse.ToString("0.##", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

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

        /// <summary>
        /// Validates report.
        /// </summary>
        /// <param name="report">Report to be validated.</param>
        /// <returns>True or false</returns>
        public bool Validate(DriveReport report)
        {
            // Report does not validate if it is read and distance is less than zero.
            if (report.KilometerAllowance == KilometerAllowance.Read && report.Distance < 0)
            {
                return false;
            }
            // Report does not validate if it is calculated and has less than two points.
            if (report.KilometerAllowance != KilometerAllowance.Read && report.DriveReportPoints.Count < 2)
            {
                return false;
            }
            // Report does not validate if it has no purpose given.
            if (string.IsNullOrEmpty(report.Purpose))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Is called from DriveReport Patch.
        /// Sends email to the user associated with the report identified by key, if his/her report has been rejected.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        public void SendMailIfRejectedReport(int key, Delta<DriveReport> delta)
        {
            var status = new object();
            if (delta.TryGetPropertyValue("Status", out status))
            {
                if (status.ToString().Equals("Rejected"))
                {
                    var report = _driveReportRepository.AsQueryable().FirstOrDefault(r => r.Id == key);
                    var recipient = "";
                    if (report != null && !String.IsNullOrEmpty(report.Person.Mail))
                    {
                        recipient = report.Person.Mail;
                    } else
                    {
                        Logger.Info("Forsøg på at sende mail om afvist indberetning til " + report.Person.FullName + ", men der findes ingen emailadresse.");
                        throw new Exception("Forsøg på at sende mail til person uden emailaddresse");
                    }
                    var comment = new object();
                    if (delta.TryGetPropertyValue("Comment", out comment))
                    {
                        _mailSender.SendMail(recipient, "Afvist indberetning",
                            "Din indberetning er blevet afvist med kommentaren: \n \n" + comment);
                    }
                }

            }
        }

        /// <summary>
        /// Gets the Responsible Leader and sets it for each of the reports in repo.
        /// </summary>
        /// <param name="repo"></param>
        /// <returns>DriveReports with ResponsibleLeader attached</returns>
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
                else
                {
                    //Indicate drivereports where we could not find a leader
                    SetResponsibleLeaderOnReport(driveReport, new Person()
                    {
                        FirstName = "Var ikke i stand til at finde godkendende leder",
                        LastName = "",
                        Initials = "FEJL"
                    });
                }
            }

            return res.AsQueryable();
        }

        /// <summary>
        /// Gets the ResponsibleLeader for driveReport
        /// </summary>
        /// <param name="driveReport"></param>
        /// <returns>DriveReport with ResponsibleLeader attached</returns>
        public Person GetResponsibleLeaderForReport(DriveReport driveReport)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var person = driveReport.Person;

            //Fetch personal approver for the person (Person and Leader of the substitute is the same)
            var personalApprover =
                _substituteRepository.AsQueryable()
                    .SingleOrDefault(
                        s =>
                            s.PersonId != s.LeaderId && s.PersonId == person.Id &&
                            s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp);
            if (personalApprover != null)
            {
                return personalApprover.Sub;
            }

            //Find an org unit where the person is not the leader, and then find the leader of that org unit to attach to the drive report
            var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == driveReport.Employment.OrgUnitId);
            var leaderOfOrgUnit =
                _employmentRepository.AsQueryable().FirstOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader);

            if (orgUnit == null)
            {
                return null;
            }

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            while ((leaderOfOrgUnit == null && orgUnit.Level > 0) || (leaderOfOrgUnit != null && leaderOfOrgUnit.PersonId == person.Id))
            {
                leaderOfOrgUnit = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnit.Id == orgUnit.ParentId && e.IsLeader &&
                                                                                            e.StartDateTimestamp < currentTimestamp &&
                                                                                            (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp)); 
                orgUnit = orgUnit.Parent;
            }


            if (orgUnit == null)
            {
                return null;
            }
            if (leaderOfOrgUnit == null)
            {
                return null;
            }

            var leader = leaderOfOrgUnit.Person;
            var sub = _substituteRepository.AsQueryable().SingleOrDefault(s => s.PersonId == leader.Id && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp && s.PersonId.Equals(s.LeaderId));

            return sub != null ? sub.Sub : leaderOfOrgUnit.Person;
        }

        /// <summary>
        /// Sets Person as ResponsibleLeader on driveReport.
        /// </summary>
        /// <param name="driveReport"></param>
        /// <param name="person"></param>
        private void SetResponsibleLeaderOnReport(DriveReport driveReport, Person person)
        {
            driveReport.ResponsibleLeader = person;
        }

        /// <summary>
        /// Filters the reports in repo by leaderId and getReportsWhereSubExists.
        /// </summary>
        /// <param name="repo">Reports to be filtered.</param>
        /// <param name="leaderId">Id of leader to filter for</param>
        /// <param name="getReportsWhereSubExists">Includes reports where a substitute exists if true</param>
        /// <returns>Filtered list of DriveReports</returns>
        public IQueryable<DriveReport> FilterByLeader(IQueryable<DriveReport> repo, int leaderId, bool getReportsWhereSubExists = false)
        {
            var result = new List<DriveReport>();

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var leaderOrgs = _employmentRepository.AsQueryable().Where(e => e.Person.Id == leaderId && e.IsLeader).Select(e => e.OrgUnit).ToList();

            var subOrgs = _substituteRepository.AsQueryable().Where(sub => sub.Sub.Id.Equals(leaderId) && sub.PersonId.Equals(sub.LeaderId)).Select(s => s.OrgUnit).ToList();

            leaderOrgs.AddRange(subOrgs);

            // Iterate all orgs belonging to the leader or sub.
            foreach (var org in leaderOrgs)
            {
                // Get the orgunit of the empl.
                var orgUnitId = org.Id;

                if (getReportsWhereSubExists)
                {
                    AddReportsOfOrgAndChildOrgLeaders(repo, orgUnitId, result);
                }
                else
                {
                    if (!(_substituteRepository.AsQueryable().Any(s => s.Person.Id == leaderId && s.StartDateTimestamp < currentTimestamp && s.EndDateTimestamp > currentTimestamp && s.OrgUnit.Id == orgUnitId)))
                    {
                        AddReportsOfOrgAndChildOrgLeaders(repo, orgUnitId, result);
                    }
                }
            }

            var personalApproverFor = _substituteRepository.AsQueryable().Where(s => s.Sub.Id == leaderId && !s.PersonId.Equals(s.LeaderId)).ToList();
            foreach (var substitute in personalApproverFor)
            {
                var sub = substitute;
                result.AddRange(repo.AsQueryable().Where(report => report.PersonId.Equals(sub.PersonId)).ToList());
            }
            result = result.Distinct().ToList();

            if (!getReportsWhereSubExists)
            {
                var finalResult = new List<DriveReport>();

                // Remove all reports with personal approver.
                foreach (var driveReport in result)
                {
                    var responsibleLeader = GetResponsibleLeaderForReport(driveReport);
                    if (responsibleLeader.Id.Equals(leaderId))
                    {
                        finalResult.Add(driveReport);
                    }
                }
                return finalResult.AsQueryable();
            }

            return result.AsQueryable();
        }

        /// <summary>
        /// Adds reports belonging to non leaders of the OrgUnit identified by orgUnitId and reports belonging to leaders of child orgs to driveReportList.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="orgUnitId"></param>
        /// <param name="driveReportList"></param>
        private void AddReportsOfOrgAndChildOrgLeaders(IQueryable<DriveReport> repo, int orgUnitId, List<DriveReport> driveReportList)
        {
            //The reports for the leaders of the child org units should also be approved
            var childOrgs = _orgUnitRepository.AsQueryable().Where(o => o.ParentId == orgUnitId).ToList(); //to list to force a data reader to close
            foreach (var childOrg in childOrgs)
            {
                var org = childOrg;
                var childEmpls = _employmentRepository.AsQueryable().Where(e => e.IsLeader && e.OrgUnit.Id == org.Id).ToList();
                if (!childEmpls.Any())
                {
                    AddReportsOfOrgAndChildOrgLeaders(repo, org.Id, driveReportList);
                }
                foreach (var childEmpl in childEmpls)
                {
                    // Get and add all reports belonging to the leader of the child org.
                    var childLeader = childEmpl.Person;
                    var childLeaderReports = repo.AsQueryable().Where(dr => dr.Person.Id == childLeader.Id && dr.Employment.OrgUnit.Id == org.Id);
                    driveReportList.AddRange(childLeaderReports);
                }
            }



            driveReportList.AddRange(
                repo.AsQueryable()
                    .Where(dr => dr.Employment.OrgUnit.Id == orgUnitId && !dr.Employment.IsLeader));
        }
    }
}
