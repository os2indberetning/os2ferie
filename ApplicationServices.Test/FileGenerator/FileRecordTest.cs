using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.FileGenerator
{
    [TestFixture]
    public class FileRecordTest
    {
        /**
         * These tests assume the dummy data in App.config. 
         * This is what is assumed is in the settings.
        
            <add key="PROTECTED_KMDFilePath" value="/sti/til/kmd/mappe"/>
            <add key="PROTECTED_KMDFileName" value="kmdFilNavn"/>
            <add key="PROTECTED_KMDHeader" value="første linje i kmd fil"/>
            <add key="PROTECTED_KMDStaticNr" value="1111"/>
            <add key="PROTECTED_CommuneNr" value="2222"/>
            <add key="PROTECTED_KMDReservedNr" value="3333"/>
          
         */


        private static readonly Employment _employment = new Employment
        {
            EmploymentId = 2,
            EmploymentType = 1,
            CostCenter = 1234,
        };

        private readonly DriveReport _report = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 100,
            Employment = _employment
        };

        private const string cpr = "1234567890";

        [Test]
        public void DistanceWithoutDecimalsShouldHave00Appended()
        {
            _report.Distance = 3999;
            var record = new FileRecord(_report, cpr);
            var recordString = record.ToString();
            Assert.AreEqual("399900", getDistanceFromRecordString(recordString));
        }

        [Test]
        public void DistanceShouldBePaddedToFourDigitsBeforeDecimal()
        {
            _report.Distance = 39.99;
            var record = new FileRecord(_report, cpr);
            var recordString = record.ToString();
            Assert.AreEqual("003999", getDistanceFromRecordString(recordString));
        }

        [Test]
        public void DistanceShouldBePaddedToTwoDigitsAfterDecimal()
        {
            _report.Distance = 3999.9;
            var record = new FileRecord(_report, cpr);
            var recordString = record.ToString();
            Assert.AreEqual("399990", getDistanceFromRecordString(recordString));
        }

        private string getDistanceFromRecordString(string recordString)
        {
            return recordString.Substring(25, 6);
        }
    }

}
