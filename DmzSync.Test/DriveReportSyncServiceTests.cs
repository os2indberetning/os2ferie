using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Impl;
using Infrastructure.DmzSync.Services.Interface;
using NSubstitute;
using NUnit.Framework;
using DriveReport = Core.DmzModel.DriveReport;
using Employment = Core.DomainModel.Employment;
using Rate = Core.DomainModel.Rate;

namespace DmzSync.Test
{
    [TestFixture]
    public class DriveReportSyncServiceTests
    {

        private ISyncService _uut;
        private IGenericRepository<Core.DomainModel.DriveReport> _masterRepoMock; 
        private IGenericRepository<Rate> _rateRepoMock; 
        private IGenericRepository<LicensePlate> _licensePlateRepoMock; 
        private IGenericRepository<Employment> _emplRepo; 
        private ILogger _logger; 
        private IGenericRepository<Core.DmzModel.DriveReport> _dmzRepoMock;
        private List<Core.DmzModel.DriveReport> _dmzReportList = new List<Core.DmzModel.DriveReport>();
        private List<Core.DomainModel.DriveReport> _masterReportList = new List<Core.DomainModel.DriveReport>();
        private IDriveReportService _driveReportServiceMock;
        private IRoute<RouteInformation> _routeMock;
        private IAddressCoordinates _coordinatesMock;
            
        [SetUp]
        public void SetUp()
        {
            _emplRepo = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _logger = NSubstitute.Substitute.For<ILogger>();
            _dmzRepoMock = NSubstitute.Substitute.For<IGenericRepository<Core.DmzModel.DriveReport>>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _masterRepoMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.DriveReport>>();
            _driveReportServiceMock = NSubstitute.Substitute.For<IDriveReportService>();
            _rateRepoMock = NSubstitute.Substitute.For<IGenericRepository<Rate>>();
            _licensePlateRepoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            _routeMock = NSubstitute.Substitute.For<IRoute<RouteInformation>>();
            _routeMock.GetRoute(DriveReportTransportType.Car,new List<Address>()).ReturnsForAnyArgs(new RouteInformation()
            {
                GeoPoints = "geogeo"
            });

            _coordinatesMock.GetAddressFromCoordinates(new Address()).ReturnsForAnyArgs(new Address()
            {
                Latitude = "1",
                Longitude = "1",
                StreetName = "Katrinebjergvej",
                StreetNumber = "93B",
                ZipCode = 8200,
                Town = "Aarhus N"
            });

            _dmzRepoMock.WhenForAnyArgs(x => x.Delete(new Core.DmzModel.DriveReport())).Do(p => _dmzReportList.Remove(p.Arg<Core.DmzModel.DriveReport>()));

         
            _driveReportServiceMock.WhenForAnyArgs(x => x.Create(new Core.DomainModel.DriveReport())).Do(rep => _masterReportList.Add(rep.Arg<Core.DomainModel.DriveReport>()));
            _dmzRepoMock.WhenForAnyArgs(x => x.Insert(new Core.DmzModel.DriveReport())).Do(t => _dmzReportList.Add(t.Arg<Core.DmzModel.DriveReport>()));

            _rateRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Rate>()
            {
                new Rate()
                {
                    Id = 1,
                    KmRate = 12,
                    Active = true,
                    Year = 2015,
                    Type = new RateType()
                    {
                        Id = 1,
                        RequiresLicensePlate = true,
                        TFCode = "1234",
                        Description = "TestDescription"
                    }
                }
            }.AsQueryable());

            _licensePlateRepoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Id = 1,
                    PersonId = 1,
                    Plate = "TestPlate",
                    IsPrimary = true,
                    Description = "TestDesc",
                }
            }.AsQueryable());

            _dmzReportList = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Purpose = "Test",
                    StartsAtHome = false,
                    EndsAtHome = false,
                    ManualEntryRemark = "ManualEntry",
                    Date = "2015-05-27",
                    EmploymentId = 1,
                    ProfileId = 1,
                    RateId = 1,
                    Profile = new Profile()
                    {
                        FullName = "Test Testesen [TT]"
                    },
                    Route = new Route()
                    {
                        Id = 1,
                        GPSCoordinates = new List<GPSCoordinate>()
                        {
                            new GPSCoordinate()
                            {
                                Latitude = StringCipher.Encrypt("1", Encryptor.EncryptKey),
                                Longitude = StringCipher.Encrypt("1", Encryptor.EncryptKey),
                            },
                            new GPSCoordinate()
                            {
                                Latitude = StringCipher.Encrypt("2", Encryptor.EncryptKey),
                                Longitude = StringCipher.Encrypt("2", Encryptor.EncryptKey),
                            }
                        }
                    }
                },
                new DriveReport()
                {
                    Id = 2,
                    Purpose = "Test2",
                    StartsAtHome = true,
                    EndsAtHome = true,
                    ManualEntryRemark = "ManualEntry",
                    Date = "2015-05-26",
                    EmploymentId = 1,
                    ProfileId = 1,
                    RateId = 1,
                    Profile = new Profile()
                    {
                        FullName = "Test Testesen [TT]"
                    },
                    Route = new Route()
                    {
                        Id = 2,
                        GPSCoordinates = new List<GPSCoordinate>()
                        {
                            new GPSCoordinate()
                            {
                                Latitude = StringCipher.Encrypt("1", Encryptor.EncryptKey),
                                Longitude = StringCipher.Encrypt("1", Encryptor.EncryptKey),
                            },
                            new GPSCoordinate()
                            {
                                Latitude = StringCipher.Encrypt("2", Encryptor.EncryptKey),
                                Longitude = StringCipher.Encrypt("2", Encryptor.EncryptKey),
                            }
                        }
                    }
                }
            };

            _masterRepoMock.AsQueryable().ReturnsForAnyArgs(_masterReportList.AsQueryable());
            _dmzRepoMock.AsQueryable().ReturnsForAnyArgs(_dmzReportList.AsQueryable());

            _uut = new DriveReportSyncService(_dmzRepoMock,_masterRepoMock,_rateRepoMock,_licensePlateRepoMock,_driveReportServiceMock, _routeMock, _coordinatesMock,_emplRepo, _logger);
        }

        [Test]
        public void SyncToDmz_ShouldThrow_NotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => _uut.SyncToDmz());
        }

        [Test]
        public void SyncFromDmz_ShouldCreateReportsInOS2()
        {
            Assert.AreEqual(0, _masterRepoMock.AsQueryable().Count());
            _uut.SyncFromDmz();
            Assert.AreEqual(2, _masterRepoMock.AsQueryable().Count());
        }

        [Test]
        public void SyncFromDmz_ShouldSetLicensePlateCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual("TestPlate", _masterRepoMock.AsQueryable().ElementAt(0).LicensePlate);
            Assert.AreEqual("TestPlate", _masterRepoMock.AsQueryable().ElementAt(1).LicensePlate);
        }

        [Test]
        public void SyncFromDmz_ShouldSetDateCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual(1432684800,_masterRepoMock.AsQueryable().ElementAt(0).DriveDateTimestamp);
            Assert.AreEqual(1432684800,_masterRepoMock.AsQueryable().ElementAt(0).CreatedDateTimestamp);
            Assert.AreEqual(1432598400, _masterRepoMock.AsQueryable().ElementAt(1).CreatedDateTimestamp);
            Assert.AreEqual(1432598400, _masterRepoMock.AsQueryable().ElementAt(1).CreatedDateTimestamp);
        }

        [Test]
        public void SyncFromDmz_ShouldSetCommentCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual("ManualEntry", _masterRepoMock.AsQueryable().ElementAt(0).UserComment);
            Assert.AreEqual("ManualEntry", _masterRepoMock.AsQueryable().ElementAt(1).UserComment);
        }

        [Test]
        public void SyncFromDmz_ShouldSetPurposeCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual("Test", _masterRepoMock.AsQueryable().ElementAt(0).Purpose);
            Assert.AreEqual("Test2", _masterRepoMock.AsQueryable().ElementAt(1).Purpose);
        }

        [Test]
        public void SyncFromDmz_ShouldSetStartsEndsHomeCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual(false, _masterRepoMock.AsQueryable().ElementAt(0).StartsAtHome);
            Assert.AreEqual(false, _masterRepoMock.AsQueryable().ElementAt(0).EndsAtHome);
            Assert.AreEqual(true, _masterRepoMock.AsQueryable().ElementAt(1).StartsAtHome);
            Assert.AreEqual(true, _masterRepoMock.AsQueryable().ElementAt(1).EndsAtHome);
        }

        [Test]
        public void SyncFromDmz_ShouldSetTFCodeCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual("1234", _masterRepoMock.AsQueryable().ElementAt(1).TFCode);
            Assert.AreEqual("1234", _masterRepoMock.AsQueryable().ElementAt(1).TFCode);
        }

        [Test]
        public void SyncFromDmz_ShouldSetKilometerAllowanceCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual(KilometerAllowance.Read, _masterRepoMock.AsQueryable().ElementAt(0).KilometerAllowance);
            Assert.AreEqual(KilometerAllowance.Read, _masterRepoMock.AsQueryable().ElementAt(1).KilometerAllowance);
        }

        [Test]
        public void SyncFromDmz_ShouldSetIsFromAppCorrectly()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual(true, _masterRepoMock.AsQueryable().ElementAt(0).IsFromApp);
            Assert.AreEqual(true, _masterRepoMock.AsQueryable().ElementAt(1).IsFromApp);
        }

    }
}
