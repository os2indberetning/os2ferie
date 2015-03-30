using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Owin.Testing;
using Ninject;
using NUnit.Framework;
using OS2Indberetning;
using Owin;
using Presentation.Web.Test.Controllers.DriveReports;
using Presentation.Web.Test.Controllers.PersonalRoutes;
using NSubstitute;
using Presentation.Web.Test;
using Substitute = NSubstitute.Substitute;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture]
    public class DriveReportServiceTest : DriveReportMock
    {
        protected TestServer Server;

        [SetUp]
        public void SetUp()
        {
            Server = TestServer.Create(app =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                config.DependencyResolver =
                    new NinjectDependencyResolver(NinjectTestInjector.CreateKernel(GetInjections()));
                app.UseWebApi(config);
            });
            //Bit of a hack to make sure that the repository is seeded
            //before each test, but at the same time that it does not 
            //seed each time it is loaded which forgets state if it is
            //queried multiple times during a single test
        }

        private List<KeyValuePair<Type, Type>> GetInjections()
        {

            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<DriveReport>),typeof (DriveReportsRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Employment>),typeof (EmploymentRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<OrgUnit>),typeof (OrgUnitRepositoryMock)),
            };
        }

        [Test]
        public void AddFullName_CalledWithIQueryable_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            Assert.Null(driveReports.First().Fullname, "Before the service is run the full name should be null");
            Assert.Null(driveReports.Last().Fullname, "Before the service is run the full name should be null");
            var service = NinjectWebKernel.CreateKernel().Get<DriveReportService>();
            driveReports = service.AddFullName(driveReports);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", driveReports.First().Fullname,
                "Service should add full name to the drive report");
            Assert.AreEqual("Morten Rasmussen [MR]", driveReports.Last().Fullname,
                "Service should add full name to the drive report (no middle name)");
        }

        [Test]
        public void AddFullName_CalledWithDriveReport_WithMiddleName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var report = driveReports.First();
            Assert.Null(report.Fullname, "Before the service is run the full name should be null");

            var service = NinjectWebKernel.CreateKernel().Get<DriveReportService>();
            service.AddFullName(report);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", report.Fullname,
                "Service should add full name to the drive report");
        }

        [Test]
        public void AddFullName_CalledWithDriveReport_WithOutMiddleName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var report = driveReports.Last();
            Assert.Null(report.Fullname, "Before the service is run the full name should be null");

            var service = NinjectWebKernel.CreateKernel().Get<DriveReportService>();
            service.AddFullName(report);
            Assert.AreEqual("Morten Rasmussen [MR]", report.Fullname, "Service should add full name to the drive report");
        }

        [Test]
        public void AddFullName_CalledWithDriveReportNull_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => NinjectWebKernel.CreateKernel().Get<DriveReportService>().AddFullName((DriveReport)null));
        }

        [Test]
        public void Create_WithPersonID0_ShouldThrowException()
        {
            var testReport = new DriveReport()
            {
                PersonId = 0
            };

            Assert.Throws<Exception>(() => NinjectWebKernel.CreateKernel().Get<DriveReportService>().Create(testReport));
        }

        [Test]
        public void AttachResponsibleLeader_WithNoSub_ShouldAttachLeader()
        {
            var uut = NinjectWebKernel.CreateKernel().Get<DriveReportService>();

            var report = new DriveReport()
            {
                Employment = new Employment()
                {
                    Id = 1,
                    OrgUnitId = 1,
                    PersonId = 1,
                },
                PersonId = 1,
            };


            var res = uut.AttachResponsibleLeader(new List<DriveReport>() { report }.AsQueryable());
        }

    }

    [TestFixture]
    public class AttachResponsibleLeaderTests
    {
        [Test]
        public void AttachResponsibleLeader_WithNoSub_ShouldAttachLeader()
        {
            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                Person = new Person()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
            };

            var emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            var orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            var subMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(),
                NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(),
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgUnitMock, emplMock,
               subMock);

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                }
            };


            var res = uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(0).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithSub_ShouldAttachSub()
        {
            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                Person = new Person()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            var emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            var orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            var subMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(),
                NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(),
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgUnitMock, emplMock,
               subMock);

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                }
            };


            var res = uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_WithSub_ShouldAttachSub()
        {
            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                Person = new Person()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            var emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            var orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            var subMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(),
                NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(),
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgUnitMock, emplMock,
               subMock);

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                }
            };


            var res = uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(2).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_WithoutSub_ShouldAttachLeader()
        {
            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                Person = new Person()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 2,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            var emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            var orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            var subMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(),
                NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(),
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgUnitMock, emplMock,
               subMock);

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                }
            };


            var res = uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(2).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_SomeWithSubSomeWithout_ShouldAttachCorrectly()
        {
            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                Person = new Person()
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var empl2 = new Employment()
            {
                Id = 1,
                OrgUnitId = 2,
                Person = new Person()
                {
                    Id = 2,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                },
                IsLeader = true
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var orgUnit2 = new OrgUnit()
            {
                Id = 2,
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            var emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl,empl2
            }.AsQueryable());

            var orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit,orgUnit2
            }.AsQueryable());

            var subMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(),
                NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(),
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgUnitMock, emplMock,
               subMock);

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl2,
                    PersonId = 1,
                }
            };


            var res = uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(2).ResponsibleLeader.FullName);
        }
    }
}
