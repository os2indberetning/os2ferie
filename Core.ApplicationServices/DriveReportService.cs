using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;


namespace Core.ApplicationServices
{
    public class DriveReportService
    {
        private readonly IRoute _route;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<DriveReportPoint> _driveReportPointRepository;
        private readonly IGenericRepository<DriveReport> _driveReportRepository;
        private readonly IGenericRepository<Person> _personRepository;
        private readonly ReimbursementCalculator _calculator;

        public DriveReportService()
        {
            _route = new BestRoute();
            _coordinates = new AddressCoordinates();
            _driveReportPointRepository = new GenericRepository<DriveReportPoint>(new DataContext());
            _driveReportRepository = new GenericRepository<DriveReport>(new DataContext());
            _personRepository = new GenericRepository<Person>(new DataContext());
            _calculator = new ReimbursementCalculator();
        }

        public DriveReportService(IRoute route, IAddressCoordinates coordinates, IGenericRepository<DriveReportPoint> driveReportPointRepository, IGenericRepository<DriveReport> driveReportRepository, IGenericRepository<Person> personRepository, ReimbursementCalculator calculator)
        {
            _route = route;
            _coordinates = coordinates;
            _driveReportPointRepository = driveReportPointRepository;
            _driveReportRepository = driveReportRepository;
            _personRepository = personRepository;
            _calculator = calculator;
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
            var pointsWithCoordinates = new List<DriveReportPoint>();

            foreach (var point in report.DriveReportPoints)
            {
                var result = (DriveReportPoint)_coordinates.GetAddressCoordinates(point);

                pointsWithCoordinates.Add(result);
            }

            report.DriveReportPoints = pointsWithCoordinates;

            var drivenRoute = _route.GetRoute(report.DriveReportPoints);

            report.Distance = (double)drivenRoute.Length / 1000;


            if (report.PersonId == 0)
            {
                throw new Exception("No person provided");
            }


            report = _calculator.Calculate(report, report.KilometerAllowance.ToString());

            var createdReport = _driveReportRepository.Insert(report);
            _driveReportRepository.Save();


            //Save DriveReportPoints in database            

            var first = report.DriveReportPoints.First();
            first.DriveReportId = createdReport.Id;
            var last = report.DriveReportPoints.Last();
            last.DriveReportId = createdReport.Id;

            if (report.DriveReportPoints.Count > 2)
            {
                foreach (var point in report.DriveReportPoints)
                {

                }
            }
            else
            {
                var firstCreated = _driveReportPointRepository.Insert(first);
                _driveReportPointRepository.Save();
                last.PreviousPointId = firstCreated.Id;
                var lastCreated = _driveReportPointRepository.Insert(last);
                _driveReportPointRepository.Save();
                firstCreated.NextPointId = lastCreated.Id;
                _driveReportPointRepository.Patch(firstCreated);
                _driveReportPointRepository.Save();
            }




            return report;
        }
    }
}
