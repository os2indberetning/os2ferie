using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices.FileGenerator
{
    public class ReportGenerator
    {
        private readonly IGenericRepository<DriveReport> _reportRepo;
        private readonly IReportFileWriter _fileWriter;
        
        public ReportGenerator(IGenericRepository<DriveReport> reportRepo, IReportFileWriter fileWriter)
        {
            _reportRepo = reportRepo;
            _fileWriter = fileWriter;
        }

        public void WriteRecordsToFileAndAlterReportStatus()
        {
            var usersToReimburse = GetUsersAndReportsToReimburse();
            var records = RecordListBuilder(usersToReimburse);

            var success = _fileWriter.WriteRecordsToFile(records);

            if (!success)
            {
                //There was an error writing the file so the reports should not be marked as invoiced
                return;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            foreach (var reports in usersToReimburse.Values)
            {
                foreach (var report in reports)
                {
                    report.Status = ReportStatus.Invoiced;
                    var deltaTime = DateTime.Now.ToUniversalTime() - epoch;
                    report.ProcessedDateTimestamp = (long)deltaTime.TotalSeconds;
                }
            }
            _reportRepo.Save();
        }

        private Dictionary<string, List<DriveReport>> GetUsersAndReportsToReimburse()
        {
            var reportsToReimburse = GetDriveReportsToReimburse();
            var reportsPerUser = new Dictionary<string, List<DriveReport>>();

            foreach (var report in reportsToReimburse)
            {
                var cpr = report.Person.CprNumber;
                if (!reportsPerUser.ContainsKey(cpr))
                {
                    reportsPerUser.Add(cpr, new List<DriveReport>());
                }
                reportsPerUser[cpr].Add(report);                
            }

            return reportsPerUser;
        }

        private IEnumerable<DriveReport> GetDriveReportsToReimburse()
        {
            return _reportRepo.AsQueryable().Where(r => r.Status == ReportStatus.Accepted).ToList();
        }

        private List<FileRecord> RecordListBuilder(Dictionary<string, List<DriveReport>> usersToReimburse)
        {
            var fileRecords = new List<FileRecord>();
            foreach (var cpr in usersToReimburse.Keys)
            {
                var driveReports = usersToReimburse[cpr].OrderBy(x => x.EmploymentId).OrderBy(x => x.TFCode).ThenBy(x => x.DriveDateTimestamp);
                DriveReport currentDriveReport = null;
                var currentTfCode = "";
                var currentMonth = -1;
                foreach (var driveReport in driveReports)
                {
                    var driveDate = TimestampToDate(driveReport.DriveDateTimestamp);
                    if ( ! driveReport.TFCode.Equals(currentTfCode) //We make one file record for each employment and each tf code
                            || driveDate.Month != currentMonth 
                            || currentDriveReport == null 
                            || ! driveReport.EmploymentId.Equals(currentDriveReport.EmploymentId) )
                    {
                        if (currentDriveReport != null)
                        {
                            fileRecords.Add(new FileRecord(currentDriveReport, cpr));
                        }
                        currentMonth = driveDate.Month;
                        currentTfCode = driveReport.TFCode;
                        currentDriveReport = new DriveReport
                        {
                            TFCode = driveReport.TFCode,
                            Employment = driveReport.Employment,
                            EmploymentId = driveReport.EmploymentId,
                            Distance = 0,
                            DriveDateTimestamp = TimetsmpOfLastDayInMonth(driveDate)
                        };

                    }
                    currentDriveReport.Distance += driveReport.Distance;
                }
                if (currentDriveReport != null) { 
                    fileRecords.Add(new FileRecord(currentDriveReport, cpr));
                }

            }
            return fileRecords;
        }

        private DateTime TimestampToDate(long timestamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }

        private long TimetsmpOfLastDayInMonth(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            return (long)(lastDay - epoch).TotalSeconds;
        }
    }
}
