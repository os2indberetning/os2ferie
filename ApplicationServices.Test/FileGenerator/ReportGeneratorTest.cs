using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.FileGenerator
{
    [TestFixture]
    public class ReportGeneratorTest
    {
        [Test]
        public void WriteRecordsShouldAlterReportStatusToInvoiced()
        {
            var repoMock = new ReportRepositoryMock();
            var reportGenerator = new ReportGenerator(repoMock, new FileWriterMock());

            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report1.Status, "Status should be accepted before being passed to file generator");
            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report2.Status, "Status should be accepted before being passed to file generator");
            Assert.AreEqual(ReportStatus.Rejected, repoMock.Report3.Status, "Status should be rejected before being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report4.Status, "Status should be invoiced before being passed to file generator"); 
            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report5.Status, "Status should be accepted before being passed to file generator");

            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report1.Status, "Status should be changed to invoiced after being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report2.Status, "Status should be changed to invoiced after being passed to file generator");
            Assert.AreEqual(ReportStatus.Rejected, repoMock.Report3.Status, "Status should not be changed by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report4.Status, "Status should not be changed by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report5.Status, "Status should be changed to invoiced after by being passed to file generator");
        }

        [Test]
        public void WriteRecordsShouldPass3RecordsToTheWriter()
        {
            var repoMock = new ReportRepositoryMock();
            var writerMock = new FileWriterMock();
            var reportGenerator = new ReportGenerator(repoMock, writerMock);

            Assert.AreEqual(0, writerMock.RecordList.Count, "The writer should have an empty list before the generator is called");
            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(3, writerMock.RecordList.Count,
                "The writer should receive three elements when it is called by the generator.");
        }

        [Test]
        public void WriteRecordsShouldGroupRecordsPassedToTheWriterByCPR()
        {
            var repoMock = new ReportRepositoryMock();
            var writerMock = new FileWriterMock();
            var reportGenerator = new ReportGenerator(repoMock, writerMock);

            Assert.AreEqual(0, writerMock.RecordList.Count, "The writer should have an empty list before the generator is called");
 
            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(3, writerMock.RecordList.Count, "The writer should receive three elements when it is called by the generator.");

            Assert.AreEqual("1111111111", writerMock.RecordList.ElementAt(0).CprNr, "CPR of first entry should be 1111111111");
            Assert.AreEqual("1111111111", writerMock.RecordList.ElementAt(1).CprNr, "CPR of second entry should be 1111111111");
            Assert.AreEqual("2222222222", writerMock.RecordList.ElementAt(2).CprNr, "CPR of third entry should be 22222222");
        }
    
    }

    class FileWriterMock : IReportFileWriter
    {
        public ICollection<FileRecord> RecordList = new List<FileRecord>(); 

        public void WriteRecordsToFile(ICollection<FileRecord> recordList)
        {
            RecordList = recordList;
        }
    }
}
