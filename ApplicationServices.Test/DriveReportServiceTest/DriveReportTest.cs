using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationServices.Test.DriveReportServiceTest;
using NUnit.Framework;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture]
    public class DriveReportTest : DriveReportBaseTest
    {
        [Test]
        public void BaseClass_GetDriveReport_IsNotNull()
        {
            Assert.NotNull(GetDriveReport());
        }

        [Test]
        public void AddFullName_WithFullName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var service = GetDriveReportService();
            driveReports = service.AddFullName(driveReports);
            Assert.AreEqual("Jacob Overgaard Jensen",driveReports.First().Fullname);
        }

        [Test]
        public void AddFullName_NoMiddleName_PopulatesFullNameCorrect()
        {
            var driveReports = GetDriveReportAsQueryable();
            var service = GetDriveReportService();
            driveReports.First().Person.MiddleName = "";
            driveReports = service.AddFullName(driveReports);
            Assert.AreEqual("Jacob Jensen", driveReports.First().Fullname);
        }
    }
}
