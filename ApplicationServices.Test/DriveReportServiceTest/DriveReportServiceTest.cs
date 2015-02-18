using System.Linq;
using Core.ApplicationServices;
using NUnit.Framework;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture]
    public class DriveReportServiceTest : DriveReportMock
    {
        [Test]
        public void AddFullName_CalledWithIQueryable_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            Assert.Null(driveReports.First().Fullname, "Before the service is run the full name should be null");
            Assert.Null(driveReports.Last().Fullname, "Before the service is run the full name should be null");
            var service = new DriveReportService();
            driveReports = service.AddFullName(driveReports);
            Assert.AreEqual("Jacob Overgaard Jensen", driveReports.First().Fullname, "Service should add full name to the drive report");
            Assert.AreEqual("Morten Rasmussen", driveReports.Last().Fullname, "Service should add full name to the drive report (no middle name)");
        }

        [Test]
        public void AddFullName_CalledWithDriveReport_WithMiddleName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var report = driveReports.First();
            Assert.Null(report.Fullname, "Before the service is run the full name should be null");

            var service = new DriveReportService();
            service.AddFullName(report);
            Assert.AreEqual("Jacob Overgaard Jensen", report.Fullname, "Service should add full name to the drive report");
        }

        [Test]
        public void AddFullName_CalledWithDriveReport_WithOutMiddleName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var report = driveReports.Last();
            Assert.Null(report.Fullname, "Before the service is run the full name should be null");

            var service = new DriveReportService();
            service.AddFullName(report);
            Assert.AreEqual("Morten Rasmussen", report.Fullname, "Service should add full name to the drive report");
        }
    }
}
