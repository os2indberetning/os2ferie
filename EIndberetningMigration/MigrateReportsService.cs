using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using EIndberetningMigration.Models;
using Infrastructure.DataAccess;

namespace EIndberetningMigration
{
    class MigrateReportsService
    {
        private readonly IGenericRepository<Employment> _employeeRepo;
        private IGenericRepository<DriveReport> _reportRepo;
        private readonly IQueryable<EindRate> _oldRates;

        private static readonly string _filePath = "legacyReports.csv";

        private ICollection<EindDriveReport> _outdatedReports = new List<EindDriveReport>();


        public int MissingReports = 0;

        public MigrateReportsService(IGenericRepository<Employment> employeeRepo, IGenericRepository<DriveReport> reportRepo)
        {
            _employeeRepo = employeeRepo;
            _reportRepo = reportRepo;
            _oldRates = DataProvider.GetRates();
        }

        public void MigrateReports(ICollection<string> initialsToMigrate)
        {
            if (initialsToMigrate.Count > 0)
            {
                // If initials are given then only migrate drive reports belonging to those initials
                foreach (var initials in initialsToMigrate)
                {
                    Console.WriteLine("Migrating initials " + initials);
                    var employees = _employeeRepo.AsQueryable().Where(x => x.Person.Initials == initials).ToList();
                    if ( ! employees.Any())
                    {
                        continue;
                    };
                    var i = 0;
                    var emplCount = employees.Count();
                    foreach (var empl in employees)
                    {
                        Console.WriteLine("Migrating employment " + i + " of " + emplCount);
                        var reports = DataProvider.GetDriveReports().Where(x => x.EmploymentID == empl.EmploymentId);
                        HandleMigrateReports(reports);
                        i++;
                    }
                }
            }
            else
            {
                // If no initials are given then migrate all drive reports.
                var reports = DataProvider.GetDriveReports();
                HandleMigrateReports(reports);
            }

            PrintReportsToFile(_outdatedReports);
        }

        private void HandleMigrateReports(IQueryable<EindDriveReport> reports)
        {
            var j = 0;
            var c = reports.Count();
            foreach (var oldReport in reports)
            {
                if ( j % 100 == 0) {
                    Console.WriteLine("Migrating report " + j + " of " + c);
                }
                j++;

                var employee = _employeeRepo.AsQueryable().FirstOrDefault(x => x.EmploymentId == oldReport.EmploymentID);
                if ( employee == null)
                {
                    _outdatedReports.Add(oldReport);
                    MissingReports++; 
                    continue;
                }
                
                //Fetch drive report points
                var points = DataProvider.GetDriveReportPoints(oldReport.Id);

                var status = ReportStatus.Pending;
                if (oldReport.Approved)
                {
                    status = ReportStatus.Accepted;
                }
                else if (oldReport.Rejected)
                {
                    status = ReportStatus.Rejected;
                }
                //If the old report is reimbursed it is also approved, so it cannot be an else if
                if (oldReport.Reimbursed)
                {
                    status = ReportStatus.Invoiced;
                }

                var approvedById = 0;
                if (status == ReportStatus.Invoiced 
                    || status == ReportStatus.Accepted
                    || status == ReportStatus.Rejected)
                {
                    var approverEmployee =
                        _employeeRepo.AsQueryable()
                            .FirstOrDefault(x => x.EmploymentId == oldReport.ApproverEmploymentID);
                    if (approverEmployee == null)
                    {
                        _outdatedReports.Add(oldReport);
                        MissingReports++;
                        continue;
                    }
                    approvedById = approverEmployee.PersonId;
                }

                var rate = _oldRates.Single(x => x.Id == oldReport.RateID);

                //Create new reports
                var newReport = new DriveReport()
                {
                    Status = status,
                    CreatedDateTimestamp = DateTimeToTimestamp(oldReport.CreationDate),
                    EditedDateTimestamp = DateTimeToTimestamp(oldReport.CreationDate),
                    Comment = oldReport.ManualEntryRemark ?? "",
                    PersonId = employee.PersonId,
                    EmploymentId = employee.Id,
                    Distance = oldReport.ReimbursableDistance,
                    AmountToReimburse = oldReport.AmmountToReimburse,
                    Purpose = oldReport.Purpose,
                    TFCode = rate.TfCode,
                    KmRate = rate.KmRate,
                    DriveDateTimestamp = DateTimeToTimestamp(oldReport.Date),
                    LicensePlate = oldReport.RegistrationNumber,
                    FullName = employee.Person.FullName,
                    IsFromApp = false,
                    IsExtraDistance = oldReport.IsExtraDistance,
                    IsOldMigratedReport = true,
                    DriveReportPoints = points
                };

                //KM Allowance
                if (oldReport.RouteDescription.Equals("Aflæst"))
                {
                    newReport.KilometerAllowance = KilometerAllowance.Read;
                }
                else
                {
                    newReport.KilometerAllowance = KilometerAllowance.Calculated;
                }


                if (status == ReportStatus.Accepted || status == ReportStatus.Rejected || status == ReportStatus.Invoiced)
                {
                    newReport.ClosedDateTimestamp = DateTimeToTimestamp(oldReport.ApprovalDate);
                    newReport.ApprovedById = approvedById;
                }

                if (status == ReportStatus.Invoiced )
                {
                    newReport.ProcessedDateTimestamp = DateTimeToTimestamp(oldReport.ReimbursementDate);
                }

                //Add reports to repo
                _reportRepo.Insert(newReport);

                if (j % 1000 == 0)
                {
                    _reportRepo.Save();
                    _reportRepo = new GenericRepository<DriveReport>(new TempContext());
                }
            }
            _reportRepo.Save();
        }

        private long DateTimeToTimestamp(DateTime? dt)
        {
            if (dt == null)
            {
                return 0;
            }
            var nonNullableDt = (DateTime)dt;
            return (long)nonNullableDt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void PrintReportsToFile(IEnumerable<EindDriveReport> reports)
        {
            using (var writer = new StreamWriter(_filePath))
            {
                //Write header
                var header = "Rappot Id\t" +
                             "CPR\t" +
                             "Medarbejdernummer\t" +
                             "Kørselsdato\t" +
                             "Formål\t" +
                             "Registreringsnummer\t" +
                             "Rute\t" +
                             "Rutebeskrivelse\t" +
                             "Distance\t" +
                             "Beløb\t" +
                             "Med Merkørsel\t" +
                             "TF Kode\t" +
                             "Kommentar\t" +
                             "Status\t" +
                             "Godkender medarbejdernummer\t" +
                             "Godkendelsesdato\t" +
                             "Lønkørselsdato\t" +
                             "Oprettelsesdato\t";

                writer.WriteLine(header);

                foreach (var report in reports)
                {
                    WriteReportAsCSVLine(report, writer);
                }
                writer.Close();
            }


        }

        private void WriteReportAsCSVLine(EindDriveReport report, StreamWriter writer)
        {
            var route = createRouteString(report);
            var rate = _oldRates.Single(x => x.Id == report.RateID);
            var status = ReportStatus.Pending;
            if (report.Reimbursed)
            {
                status = ReportStatus.Invoiced;
            } 
            else if (report.Rejected)
            {
                status = ReportStatus.Rejected;
            } 
            else if (report.Approved)
            {
                status = ReportStatus.Accepted;
            }

            if (status != ReportStatus.Invoiced)
            {
                return;
            }

            DateTime approvedDate = report.ApprovalDate ?? new DateTime(1900, 1, 1);
            DateTime reimbursedDate = report.ReimbursementDate ?? new DateTime(1900, 1, 1);

            writer.Write(report.Id + "\t");
            writer.Write(report.CPR + "\t");
            writer.Write(report.EmploymentID + "\t");
            writer.Write(report.Date.ToString("dd-MM-yy") + "\t");
            writer.Write(report.Purpose + "\t");
            writer.Write(report.RegistrationNumber + "\t");
            writer.Write(route + "\t");
            writer.Write(report.RouteDescription + "\t");
            writer.Write(report.ReimbursableDistance + "\t");
            writer.Write(report.AmmountToReimburse + "\t");
            writer.Write(report.IsExtraDistance + "\t");
            writer.Write(rate.TfCode + "\t");
            writer.Write(report.ManualEntryRemark + "\t");
            writer.Write(status + "\t");
            writer.Write(report.ApproverEmploymentID + "\t");
            if (approvedDate.Year == 1900)
            {
                writer.Write(" " + "\t");
            }
            else
            {
                writer.Write(approvedDate.ToString("dd-MM-yy") + "\t");   
            }
            if (reimbursedDate.Year == 1900)
            {
                writer.Write(" " + "\t");
            }
            else
            {
                writer.Write(reimbursedDate.ToString("dd-MM-yy") + "\t");
            }
            writer.Write(report.CreationDate.ToString("dd-MM-yy") + "\t");
            writer.WriteLine();
        }

        private string createRouteString(EindDriveReport report)
        {
            var route = "";
            var points = DataProvider.GetDriveReportPoints(report.Id).ToList();
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                route += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                if (i < points.Count - 1)
                {
                    route += " -> ";
                }
            }
            return route;
        }
    }
}
