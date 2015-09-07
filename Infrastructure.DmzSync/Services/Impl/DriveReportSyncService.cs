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
using Infrastructure.AddressServices;
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

        /// <summary>
        /// Synchronizes all DriveReports from DMZ to OS2 database.
        /// </summary>
        public void SyncFromDmz()
        {
            var reports = _dmzDriveReportRepo.AsQueryable().ToList();
            var max = reports.Count;

            for (var i = 0; i < max; i++)
            {
                var dmzReport = reports[i];
                dmzReport.Profile = Encryptor.DecryptProfile(dmzReport.Profile);
                Console.WriteLine("Syncing report " + i + " of " + max + " from DMZ.");
                var rate = _rateRepo.AsQueryable().First(x => x.Id.Equals(dmzReport.RateId));
                var points = new List<DriveReportPoint>();
                var viaPoints = new List<DriveReportPoint>();
                for (var j = 0; j < dmzReport.Route.GPSCoordinates.Count; j++)
                {
                    var gpsCoord = dmzReport.Route.GPSCoordinates.ToArray()[j];
                    gpsCoord = Encryptor.DecryptGPSCoordinate(gpsCoord);

                    points.Add(new DriveReportPoint
                    {
                        Latitude = gpsCoord.Latitude,
                        Longitude = gpsCoord.Longitude,
                    });

                    if (gpsCoord.IsViaPoint)
                    {
                        var address = _coordinates.GetAddressFromCoordinates(new Address
                        {
                            Latitude = gpsCoord.Latitude,
                            Longitude = gpsCoord.Longitude
                        });

                        viaPoints.Add(new DriveReportPoint()
                        {
                            Latitude = gpsCoord.Latitude,
                            Longitude = gpsCoord.Longitude,
                            StreetName = address.StreetName,
                            StreetNumber = address.StreetNumber,
                            ZipCode = address.ZipCode,
                            Town = address.Town,
                        });
                    }
                }

                var licensePlate = _licensePlateRepo.AsQueryable().FirstOrDefault(x => x.PersonId.Equals(dmzReport.ProfileId) && x.IsPrimary);
                var plate = licensePlate != null ? licensePlate.Plate : "UKENDT";

                var newReport = new Core.DomainModel.DriveReport
                {

                    IsFromApp = true,
                    Distance = dmzReport.Route.TotalDistance,
                    KilometerAllowance = KilometerAllowance.Read,
                    // Date might not be correct. Depends which culture is delivered from app. 
                    // https://msdn.microsoft.com/en-us/library/cc165448.aspx
                    DriveDateTimestamp = (Int32)(Convert.ToDateTime(dmzReport.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
                    CreatedDateTimestamp = (Int32)(Convert.ToDateTime(dmzReport.Date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
                    StartsAtHome = dmzReport.StartsAtHome,
                    EndsAtHome = dmzReport.EndsAtHome,
                    Purpose = dmzReport.Purpose,
                    PersonId = dmzReport.ProfileId,
                    EmploymentId = dmzReport.EmploymentId,
                    KmRate = rate.KmRate,
                    TFCode = rate.Type.TFCode,
                    UserComment = dmzReport.ManualEntryRemark,
                    Status = ReportStatus.Pending,
                    FullName = dmzReport.Profile.FullName,
                    LicensePlate = plate,
                    Comment = "",
                    DriveReportPoints = viaPoints
                };

                newReport.RouteGeometry = GeoService.Encode(points);

                _driveService.Create(newReport);
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SyncToDmz()
        {
            // We are not interested in syncing reports from OS2 to DMZ.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all DriveReports from DMZ database.
        /// </summary>
        public void ClearDmz()
        {
            var reports = _dmzDriveReportRepo.AsQueryable().ToList();
            _dmzDriveReportRepo.DeleteRange(reports);
            _dmzDriveReportRepo.Save();
        }

    }

}
