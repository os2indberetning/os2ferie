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
using OS2Indberetning.App_Start;
using Owin;
using Presentation.Web.Test.Controllers.DriveReports;
using Presentation.Web.Test.Controllers.PersonalRoutes;
using NSubstitute;
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
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<DriveReport>),
                    typeof (DriveReportsRepositoryMock))
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
            Assert.AreEqual("Jacob Overgaard Jensen", driveReports.First().Fullname,
                "Service should add full name to the drive report");
            Assert.AreEqual("Morten Rasmussen", driveReports.Last().Fullname,
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
            Assert.AreEqual("Jacob Overgaard Jensen", report.Fullname,
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
            Assert.AreEqual("Morten Rasmussen", report.Fullname, "Service should add full name to the drive report");
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

    }
}
