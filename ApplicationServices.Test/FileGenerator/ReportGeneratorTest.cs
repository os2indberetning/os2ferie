using System;
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
            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report6.Status, "Status should be accepted before being passed to file generator");
            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report7.Status, "Status should be accepted before being passed to file generator");
            Assert.AreEqual(ReportStatus.Accepted, repoMock.Report8.Status, "Status should be accepted before being passed to file generator");

            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report1.Status, "Status should be changed to invoiced after being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report2.Status, "Status should be changed to invoiced after being passed to file generator");
            Assert.AreEqual(ReportStatus.Rejected, repoMock.Report3.Status, "Status should not be changed by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report4.Status, "Status should not be changed by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report5.Status, "Status should be changed to invoiced after by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report6.Status, "Status should be changed to invoiced after by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report7.Status, "Status should be changed to invoiced after by being passed to file generator");
            Assert.AreEqual(ReportStatus.Invoiced, repoMock.Report8.Status, "Status should be changed to invoiced after by being passed to file generator");
        }

        [Test]
        public void WriteRecordsShouldPass4RecordsToTheWriter()
        {
            var repoMock = new ReportRepositoryMock();
            var writerMock = new FileWriterMock();
            var reportGenerator = new ReportGenerator(repoMock, writerMock);

            Assert.AreEqual(0, writerMock.RecordList.Count, "The writer should have an empty list before the generator is called");
            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(4, writerMock.RecordList.Count,
                "The writer should receive three elements when it is called by the generator.");
        }

        [Test]
        public void WriteRecordShouldGroupReportsWithSameTFCodeAndSameMonth()
        {
            var repoMock = new ReportRepositoryMock();
            var writerMock = new FileWriterMock();
            var reportGenerator = new ReportGenerator(repoMock, writerMock);

            Assert.AreEqual(0, writerMock.RecordList.Count, "The writer should have an empty list before the generator is called");
            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(600, writerMock.RecordList.ElementAt(0).ReimbursementDistance, "Total distance of person 1s reports should be 600");
            Assert.AreEqual(600, writerMock.RecordList.ElementAt(1).ReimbursementDistance, "Total distance of person 2s reports from march with tf code 310-3 should be 600");
            Assert.AreEqual(800, writerMock.RecordList.ElementAt(2).ReimbursementDistance, "Total distance of person 2s reports from march with tf code 310-4 should be 800");
            Assert.AreEqual(600, writerMock.RecordList.ElementAt(3).ReimbursementDistance, "Total distance of person 2s reports from april with tf code 310-4 should be 600");
        }

        [Test]
        public void WriteRecordShouldSetTheDateToBeTheLastInTheMonth()
        {
            var repoMock = new ReportRepositoryMock();
            var writerMock = new FileWriterMock();
            var reportGenerator = new ReportGenerator(repoMock, writerMock);

            Assert.AreEqual(0, writerMock.RecordList.Count, "The writer should have an empty list before the generator is called");
            reportGenerator.WriteRecordsToFileAndAlterReportStatus();

            Assert.AreEqual(new DateTime(2015, 3, 31).Year, writerMock.RecordList.ElementAt(0).Date.Year, "The date of the first record should be March 31 2015");
            Assert.AreEqual(new DateTime(2015, 3, 31).Month, writerMock.RecordList.ElementAt(0).Date.Month, "The date of the first record should be March 31 2015");
            Assert.AreEqual(new DateTime(2015, 3, 31).Date, writerMock.RecordList.ElementAt(0).Date.Date, "The date of the first record should be March 31 2015");
            Assert.AreEqual(new DateTime(2015, 4, 30).Year, writerMock.RecordList.ElementAt(3).Date.Year, "The date of the first record should be April 30 2015");
            Assert.AreEqual(new DateTime(2015, 4, 30).Month, writerMock.RecordList.ElementAt(3).Date.Month, "The date of the first record should be April 30 2015");
            Assert.AreEqual(new DateTime(2015, 4, 30).Date, writerMock.RecordList.ElementAt(3).Date.Date, "The date of the first record should be April 30 2015");
        }
    }

    class FileWriterMock : IReportFileWriter
    {
        public ICollection<FileRecord> RecordList = new List<FileRecord>(); 

        public bool WriteRecordsToFile(ICollection<FileRecord> recordList)
        {
            RecordList = recordList;
            return true;
        }
    }
}
