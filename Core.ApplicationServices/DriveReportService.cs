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
        private readonly IGenericRepository<LicensePlate> _licensePlateRepository;
        private readonly ReimbursementCalculator _calculator;
        private readonly IMailSender _mailSender;

        public DriveReportService(IMailSender mailSender, IGenericRepository<DriveReport> driveReportRepository, IGenericRepository<LicensePlate> licensePlateRepository)
        {
            _route = new BestRoute();
            _coordinates = new AddressCoordinates();
            _calculator = new ReimbursementCalculator();
            _mailSender = mailSender;
            _driveReportRepository = driveReportRepository;
            _licensePlateRepository = licensePlateRepository;
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
                        .Select(currentPoint => (DriveReportPoint) _coordinates.GetAddressCoordinates(currentPoint))
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
            if (_licensePlateRepository.AsQueryable().FirstOrDefault(x => x.PersonId == report.PersonId && x.Plate == report.Licenseplate) == null)
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

    }
}
