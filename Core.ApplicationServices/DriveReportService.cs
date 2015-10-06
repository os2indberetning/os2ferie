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
                var pointsWithCoordinates = new List<DriveReportPoint>();
                foreach (var driveReportPoint in report.DriveReportPoints)
                {
                    if (string.IsNullOrEmpty(driveReportPoint.Latitude) || driveReportPoint.Latitude == "0" ||
                        string.IsNullOrEmpty(driveReportPoint.Longitude) || driveReportPoint.Longitude == "0")
                    {
                        pointsWithCoordinates.Add(
                            (DriveReportPoint) _coordinates.GetAddressCoordinates(driveReportPoint));
                    }
                    else
                    {
                        pointsWithCoordinates.Add(driveReportPoint);
                    }
                }

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
            createdReport.ResponsibleLeaderId = GetResponsibleLeaderForReport(report).Id;
            createdReport.ActualLeaderId = GetActualLeaderForReport(report).Id;
            _driveReportRepository.Save();

            // If the report is calculated or from an app, then we would like to store the points.
            if (report.KilometerAllowance != KilometerAllowance.Read || report.IsFromApp)
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
                _employmentRepository.AsQueryable().FirstOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader && e.StartDateTimestamp < currentDateTimestamp && (e.EndDateTimestamp > currentDateTimestamp || e.EndDateTimestamp == 0));

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

            // Recursively look for substitutes in child orgs, up to the org of the actual leader.
            // Say the actual leader is leader of orgunit 1 with children 2 and 3. Child 2 has another child 4.
            // A report comes in for orgUnit 4. Check if leader has a substitute for that org.
            // If not then check if leader has a substitute for org 2.
            // If not then return the actual leader.
            var orgToCheck = driveReport.Employment.OrgUnit;
            Substitute sub = null;
            var loopHasFinished = false;
            while (!loopHasFinished)
            {
                sub = _substituteRepository.AsQueryable().SingleOrDefault(s => s.OrgUnitId == orgToCheck.Id && s.PersonId == leader.Id && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp && s.PersonId.Equals(s.LeaderId));
                if (sub != null)
                {
                    if(sub.Sub == null)
                    {
                        // This is a hack fix for a weird bug that happens, where sometimes the Sub navigation property on a Substitute is null, even though the SubId is not.
                        sub.Sub = _employmentRepository.AsQueryable().FirstOrDefault(x => x.PersonId == sub.SubId).Person;
                    }
                    loopHasFinished = true;
                }
                else
                {
                    orgToCheck = orgToCheck.Parent;
                    if (orgToCheck == null || orgToCheck.Id == orgUnit.Parent.Id)
                    {
                        loopHasFinished = true;
                    }
                }
            }
            return sub != null ? sub.Sub : leaderOfOrgUnit.Person;
        }

        public Person GetActualLeaderForReport(DriveReport driveReport)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var person = driveReport.Person;

            //Find an org unit where the person is not the leader, and then find the leader of that org unit to attach to the drive report
            var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == driveReport.Employment.OrgUnitId);
            var leaderOfOrgUnit =
                _employmentRepository.AsQueryable().FirstOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader && e.StartDateTimestamp < currentDateTimestamp && (e.EndDateTimestamp > currentDateTimestamp || e.EndDateTimestamp == 0));

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

            return leaderOfOrgUnit.Person;
        }

       
    }
}
