using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Interface;
using DriveReport = Core.DomainModel.DriveReport;
using Employment = Core.DmzModel.Employment;
using Rate = Core.DomainModel.Rate;

namespace Infrastructure.DmzSync.Services.Impl
{
    public class DriveReportSyncService : ISyncService
    {
        private IGenericRepository<Core.DmzModel.DriveReport> _dmzDriveReportRepo;
        private IGenericRepository<Core.DomainModel.DriveReport> _masterDriveReportRepo;
        private readonly IGenericRepository<Rate> _rateRepo;
        private readonly IGenericRepository<LicensePlate> _licensePlateRepo;
        private readonly IDriveReportService _driveService;
        private readonly IRoute<RouteInformation> _routeService;
        private readonly IAddressCoordinates _coordinates;

        public DriveReportSyncService(IGenericRepository<Core.DmzModel.DriveReport> dmzDriveReportRepo, IGenericRepository<Core.DomainModel.DriveReport> masterDriveReportRepo, IGenericRepository<Core.DomainModel.Rate> rateRepo, IGenericRepository<LicensePlate> licensePlateRepo, IDriveReportService driveService, IRoute<RouteInformation> routeService, IAddressCoordinates coordinates)
        {
            _dmzDriveReportRepo = dmzDriveReportRepo;
            _masterDriveReportRepo = masterDriveReportRepo;
            _rateRepo = rateRepo;
            _licensePlateRepo = licensePlateRepo;
            _driveService = driveService;
            _routeService = routeService;
            _coordinates = coordinates;
        }

        public void SyncFromDmz()
        {
            var i = 0;
            var reports = _dmzDriveReportRepo.AsQueryable().ToList();
            var max = reports.Count;

            foreach (var report in reports)
            {
                i++;
                Console.WriteLine("Syncing report " + i + " of " + max + " from DMZ.");
                var rate = _rateRepo.AsQueryable().First(x => x.Id.Equals(report.RateId));
                var points = new List<DriveReportPoint>();
                foreach (var gpsCoord in report.Route.GPSCoordinates)
                {
                    var address = _coordinates.GetAddressFromCoordinates(new Address()
                    {
                        Latitude = gpsCoord.Latitude,
                        Longitude = gpsCoord.Longitude
                    });

                    points.Add(new DriveReportPoint
                    {
                        Latitude = gpsCoord.Latitude,
                        Longitude = gpsCoord.Longitude,
                        StreetName = address.StreetName,
                        StreetNumber = address.StreetNumber,
                        ZipCode = address.ZipCode,
                        Town = address.Town
                    });
                }
               
                var newReport = new Core.DomainModel.DriveReport
                {
                    
                    IsFromApp = true,
                    Distance = report.Route.TotalDistance,
                    KilometerAllowance = KilometerAllowance.Read,
                    // Date might not be correct. Depends which culture is delivered from app. 
                    // https://msdn.microsoft.com/en-us/library/cc165448.aspx
                    DriveDateTimestamp = (Int32)(Convert.ToDateTime(report.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
                    CreatedDateTimestamp = (Int32)(Convert.ToDateTime(report.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
                    StartsAtHome = report.StartsAtHome,
                    EndsAtHome = report.EndsAtHome,
                    Purpose = report.Purpose,
                    PersonId = report.ProfileId,
                    EmploymentId = report.EmploymentId,
                    KmRate = rate.KmRate,
                    TFCode = rate.Type.TFCode,
                    UserComment = report.ManualEntryRemark,
                    Status = ReportStatus.Pending,
                    FullName = report.Profile.FullName,
                    LicensePlate = _licensePlateRepo.AsQueryable().First(x => x.PersonId.Equals(report.ProfileId) && x.IsPrimary).Plate,
                    Comment = "",
                };

                var route = _routeService.GetRoute(points);
                if (route != null)
                {
                    newReport.RouteGeometry = route.GeoPoints;
                }

                _driveService.Create(newReport);
            }
        }

        public void SyncToDmz()
        {
            // We are not interested in syncing reports from OS2 to DMZ.
            throw new NotImplementedException();
        }

        public void ClearDmz()
        {
            var i = 0;
            var reports = _dmzDriveReportRepo.AsQueryable().ToList();
            var max = reports.Count;

            foreach (var report in reports)
            {
                i++;
                Console.WriteLine("Clearing report " + i + " of " + max + " from DMZ.");
                _dmzDriveReportRepo.Delete(report);
            }
            _dmzDriveReportRepo.Save();
        }

    }

}
