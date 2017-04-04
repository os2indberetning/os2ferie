using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Core.ApplicationServices.Logger;

namespace Core.ApplicationServices
{
    public class DriveReportService : ReportService<DriveReport>, IDriveReportService
    {
        private readonly IRoute<RouteInformation> _route;
        private readonly IGenericRepository<RateType> _rateTypeRepo;
        private readonly IAddressCoordinates _coordinates;
        private readonly IReimbursementCalculator _calculator;

        public DriveReportService(IGenericRepository<DriveReport> reportRepo, IReimbursementCalculator calculator, IAddressCoordinates coordinates, IRoute<RouteInformation> route, IGenericRepository<RateType> rateTypeRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, ILogger logger) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository, logger, reportRepo)
        {
            _route = route;
            _rateTypeRepo = rateTypeRepo;
            _coordinates = coordinates;
            _calculator = calculator;
        }

        /// <summary>
        /// Validates report and creates it in the database if it validates.
        /// </summary>
        /// <param name="report">Report to be created.</param>
        /// <returns>Created report.</returns>
        public new DriveReport Create(DriveReport report)
        {
            if (report.PersonId == 0)
            {
                throw new Exception("No person provided");
            }

            if (!Validate(report))
            {
                throw new Exception("DriveReport has some invalid parameters");
            }

            if (report.IsFromApp)
            {
                report = _calculator.Calculate(null, report);
            }
            else
            {
                if (report.KilometerAllowance != KilometerAllowance.Read)
                {
                    var pointsWithCoordinates = new List<DriveReportPoint>();
                    foreach (var driveReportPoint in report.DriveReportPoints)
                    {
                        if (string.IsNullOrEmpty(driveReportPoint.Latitude) || driveReportPoint.Latitude == "0" ||
                            string.IsNullOrEmpty(driveReportPoint.Longitude) || driveReportPoint.Longitude == "0")
                        {
                            pointsWithCoordinates.Add(
                                (DriveReportPoint)_coordinates.GetAddressCoordinates(driveReportPoint));
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


                    report.Distance = (double)drivenRoute.Length / 1000;

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
            }

            // Round off Distance and AmountToReimburse to two decimals.
            report.Distance = Convert.ToDouble(report.Distance.ToString("0.##", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            report.AmountToReimburse = Convert.ToDouble(report.AmountToReimburse.ToString("0.##", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            var createdReport = _reportRepo.Insert(report);
            createdReport.ResponsibleLeaderId = GetResponsibleLeaderForReport(report).Id;
            createdReport.ActualLeaderId = GetActualLeaderForReport(report).Id;

            _reportRepo.Save();

            // If the report is calculated or from an app, then we would like to store the points.
            if (report.KilometerAllowance != KilometerAllowance.Read || report.IsFromApp)
            {
                // Reports from app with manual distance have no drivereportpoints.
                if (report.DriveReportPoints.Count > 1)
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
                    _reportRepo.Save();
                }
            }

            return report;
        }

        /// <summary>
        /// Validates report.
        /// </summary>
        /// <param name="report">Report to be validated.</param>
        /// <returns>True or false</returns>
        public new bool Validate(DriveReport report)
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
        /// Sends an email to the owner of and person responsible for a report that has been edited or rejected by an admin.
        /// </summary>
        /// <param name="report">The edited report</param>
        /// <param name="emailText">The message to be sent to the owner and responsible leader</param>
        /// <param name="admin">The admin rejecting or editing</param>
        /// <param name="action">A string included in the email. Should be "afvist" or "redigeret"</param>
        public new void SendMailToUserAndApproverOfEditedReport(DriveReport report, string emailText, Person admin, string action)
        {
            var mailContent = "Hej," + Environment.NewLine + Environment.NewLine +
            "Jeg, " + admin.FullName + ", har pr. dags dato " + action + " den følgende godkendte kørselsindberetning:" + Environment.NewLine + Environment.NewLine;

            mailContent += "Formål: " + report.Purpose + Environment.NewLine;

            if (report.KilometerAllowance != KilometerAllowance.Read)
            {
                mailContent += "Startadresse: " + report.DriveReportPoints.ElementAt(0).ToString() + Environment.NewLine
                + "Slutadresse: " + report.DriveReportPoints.Last().ToString() + Environment.NewLine;
            }

            mailContent += "Afstand: " + report.Distance.ToString().Replace(".",",") + Environment.NewLine
            + "Kørselsdato: " + FromUnixTime(report.DriveDateTimestamp) + Environment.NewLine + Environment.NewLine
            + "Hvis du mener at dette er en fejl, så kontakt mig da venligst på " + admin.Mail + Environment.NewLine
            + "Med venlig hilsen " + admin.FullName + Environment.NewLine + Environment.NewLine
            + "Besked fra administrator: " + Environment.NewLine + emailText;

            _mailSender.SendMail(report.Person.Mail, "En administrator har ændret i din indberetning.", mailContent);

            _mailSender.SendMail(report.ApprovedBy.Mail, "En administrator har ændret i en indberetning du har godkendt.", mailContent);
        }

        /// <summary>
        /// Converts timestamp to datetime
        /// </summary>
        /// <param name="unixTime">Timestamp to convert</param>
        /// <returns>DateTime</returns>
        private string FromUnixTime(long unixTime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
        }


    }
}
