using System;
using System.Linq;
using System.Web.OData;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;

namespace Core.ApplicationServices
{
    public class VacationReportService : ReportService<VacationReport>, IVacationReportService
    {
        private readonly IKMDAbsenceService _absenceService;
        private readonly IKMDAbsenceReportBuilder _absenceBuilder;

        public VacationReportService(IGenericRepository<VacationReport> reportRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, IKMDAbsenceService absenceService, IKMDAbsenceReportBuilder absenceBuilder, ILogger logger) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository, logger, reportRepo)
        {
            _absenceService = absenceService;
            _absenceBuilder = absenceBuilder;
        }

        public void PrepareReport(VacationReport report)
        {
            if (!Validate(report)) throw new Exception("Vacation report has invalid parameters");

            report.ResponsibleLeaderId = GetResponsibleLeaderForReport(report).Id;
            report.ActualLeaderId = GetActualLeaderForReport(report).Id;

            var startDateTime = report.StartTimestamp.ToDateTime();

            report.EndTimestamp = report.EndTimestamp.ToDateTime().Date.ToTimestamp();
            report.StartTimestamp = startDateTime.Date.ToTimestamp();

            report.VacationYear = startDateTime.Year;

            if (startDateTime.Date < new DateTime(report.VacationYear, 5, 1))
            {
                report.VacationYear--;
            }

            // a.start <= b.end && b.start <= a.end;
            var colidingReports = _reportRepo.AsQueryable()
                .Where(
                    x =>
                        x.PersonId == report.PersonId && x.Status != ReportStatus.Rejected && ((x.StartTimestamp < report.EndTimestamp + 86400 && report.StartTimestamp < x.EndTimestamp + 86400) ||
                        x.StartTimestamp == report.StartTimestamp || x.EndTimestamp == report.EndTimestamp));

            if (colidingReports.Any())
            {
                var colides = false;
                foreach (var colidingReport in colidingReports)
                {
                    if (colidingReport.Id == report.Id) continue;

                    var colideStartTotal = (double) colidingReport.StartTimestamp;
                    var colideEndTotal = (double) colidingReport.EndTimestamp;

                    if (colidingReport.StartTime.HasValue) colideStartTotal += colidingReport.StartTime.Value.TotalSeconds;
                    if (colidingReport.EndTime.HasValue)
                    {
                        colideEndTotal += colidingReport.EndTime.Value.TotalSeconds;
                    }
                    else if (colidingReport.EndTimestamp == report.StartTimestamp)
                    {
                        colideEndTotal += 86400;
                    }

                    var reportStartTotal = (double) report.StartTimestamp;
                    var reportEndTotal = (double) report.EndTimestamp;

                    if (report.StartTime.HasValue) reportStartTotal += report.StartTime.Value.TotalSeconds;
                    if (report.EndTime.HasValue)
                    {
                        reportEndTotal += report.EndTime.Value.TotalSeconds;
                    }
                    else if (report.EndTimestamp == colidingReport.StartTimestamp)
                    {
                        reportEndTotal += 86400;
                    }

                    if (!(reportStartTotal < colideEndTotal) || !(colideStartTotal < reportEndTotal)) continue;

                    colides = true;
                    break;
                }
                if (colides) throw new Exception("Coliding report");
            }

            if (report.Comment == null) report.Comment = "";
            if (report.Purpose == null) report.Purpose = "";

            report.ProcessedDateTimestamp = 0;
            report.Status = ReportStatus.Pending;
        }

        public new VacationReport Create(VacationReport report)
        {
            PrepareReport(report);
            _reportRepo.Insert(report);
            _reportRepo.Save();
            return report;
        }

        public VacationReport Edit(Delta<VacationReport> delta)
        {
            var newReport = delta.GetEntity();
            var report = _reportRepo.AsQueryable().First(x => x.Id == newReport.Id);
            PrepareReport(newReport);
            if (report.Status == ReportStatus.Accepted)
            {
                SendMailIfUserEditedAprovedReport(newReport, "redigeret");
            }
            if (report.ProcessedDateTimestamp != 0)
            {
                DeleteReport(report);
            }
            delta.Patch(report);
            _reportRepo.Save();
            return newReport;
        }

        public void Delete(int id)
        {
            var report = _reportRepo.AsQueryable().First(x => x.Id == id);

            if (report.Status == ReportStatus.Accepted)
            {
                SendMailIfUserEditedAprovedReport(report, "slettet");
            }
            if (report.ProcessedDateTimestamp != 0)
            {
                DeleteReport(report);
            }
            _reportRepo.Delete(report);
            _reportRepo.Save();
        }

        public new bool Validate(VacationReport report)
        {
            if (report.PersonId == 0) return false;
            if (report.EndTimestamp < report.StartTimestamp) return false;
            if (report.StartTime > report.EndTime &&
                report.StartTimestamp.ToDateTime().Date == report.EndTimestamp.ToDateTime().Date) return false;
            if (!_employmentRepository.AsQueryable().First(x => x.Id == report.EmploymentId).OrgUnit.HasAccessToVacation) return false;
            if (report.StartTimestamp == report.EndTimestamp && report.StartTime.HasValue && report.EndTime.HasValue && report.StartTime.Value == report.EndTime.Value) return false;

            return true;
        }

        public void SendMailIfUserEditedAprovedReport(VacationReport report, string action)
        {
            var mailContent = "Hej," + Environment.NewLine + Environment.NewLine +
                              "Jeg," + report.Person.FullName + " har pr. dags dato " + action + " den følgende godkendte ferieindberetning:" +
                              Environment.NewLine + Environment.NewLine;

            mailContent += "Feriestart: " + report.StartTimestamp.ToDateTime().ToString("dd/MM/yyyy");

            if (report.StartTime != null)
                mailContent += " - " + report.StartTime?.ToString("hh:mm");

            mailContent += Environment.NewLine + "Ferieafslutning: " + report.EndTimestamp.ToDateTime().ToString("dd/MM/yyyy");

            if (report.EndTime != null)
                mailContent += " - " + report.EndTime?.ToString("hh:mm");

            mailContent += Environment.NewLine + "Ferietype: " +
                           (report.VacationType == VacationType.Regular
                               ? "Almindelig ferie"
                               : "6. ferieuge");

            if (report.Purpose != null)
                mailContent += Environment.NewLine + "Bemærkning: " + report.Purpose;

            mailContent += Environment.NewLine + Environment.NewLine
                           + "Med venlig hilsen " + report.Person.FullName + Environment.NewLine + Environment.NewLine;


            _mailSender.SendMail(report.ApprovedBy.Mail, "En medarbejder har ændret i en indberetning du har godkendt.", mailContent);
        }

        public new void SendMailToUserAndApproverOfEditedReport(VacationReport report, string emailText, Person admin, string action)
        {
            var mailContent = "Hej," + Environment.NewLine + Environment.NewLine +
            "Jeg, " + admin.FullName + ", har pr. dags dato " + action + " den følgende godkendte ferieindberetning:" + Environment.NewLine + Environment.NewLine;


            mailContent += "Feriestart: " + report.StartTimestamp.ToDateTime().ToString("dd/MM/yyyy");

            if (report.StartTime != null)
                mailContent += " - " + report.StartTime?.ToString("hh:mm");

            mailContent += Environment.NewLine + "Ferieafslutning: " + report.EndTimestamp.ToDateTime().ToString("dd/MM/yyyy");

            if (report.EndTime != null)
                mailContent += " - " + report.EndTime?.ToString("hh:mm");

            mailContent += Environment.NewLine + "Ferietype: " +
                           (report.VacationType == VacationType.Regular
                               ? "Almindelig ferie"
                               : "6. ferieuge");

            if (report.Purpose != null)
                mailContent += Environment.NewLine + "Bemærkning: " + report.Purpose;

            mailContent += Environment.NewLine + Environment.NewLine
            + "Hvis du mener at dette er en fejl, så kontakt mig da venligst på " + admin.Mail + Environment.NewLine
            + "Med venlig hilsen " + admin.FullName + Environment.NewLine + Environment.NewLine
            + "Besked fra administrator: " + Environment.NewLine + emailText;

            _mailSender.SendMail(report.Person.Mail, "En administrator har ændret i din indberetning.", mailContent);

            _mailSender.SendMail(report.ApprovedBy.Mail, "En administrator har ændret i en indberetning du har godkendt.", mailContent);
        }

        public void SendMailIfRejectedReport(VacationReport report)
        {

            if (report.Status != ReportStatus.Rejected) return;
            if (string.IsNullOrEmpty(report.Person.Mail))
            {
                _logger.Log(
                    "Forsøg på at sende mail om afvist indberetning til " + report.Person.FullName +
                    ", men der findes ingen emailadresse. " + report.Person.FullName +
                    " har derfor ikke modtaget en mailadvisering", "mail", 2);
                throw new Exception("Forsøg på at sende mail til person uden emailaddresse");
            }

            var recipient = report.Person.Mail;

            _mailSender.SendMail(recipient, "Afvist ferieindberetning",
                "Din ferieindberetning er blevet afvist med kommentaren: \n \n" + report.Comment);
        }

        public void ApproveReport(VacationReport report, Person approver)
        {
            report.Status = ReportStatus.Accepted;
            report.ClosedDateTimestamp = (DateTime.UtcNow.ToTimestamp());
            report.ApprovedById = approver.Id;

#if !DEBUG
            var absenceReport = _absenceBuilder.Create(report);
            _absenceService.ReportAbsence(absenceReport);
#endif

            report.ProcessedDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _reportRepo.Save();
        }

        public void RejectReport(VacationReport report, Person approver, string comment)
        {
            report.Status = ReportStatus.Rejected;
            report.ClosedDateTimestamp = (DateTime.UtcNow.ToTimestamp());
            report.Comment = comment;
            report.ApprovedById = approver.Id;
            DeleteReport(report);
            _reportRepo.Save();
        }

        public void DeleteReport(VacationReport report)
        {
#if !DEBUG
            if (report.ProcessedDateTimestamp == 0) return;
            var absenceReport = _absenceBuilder.Delete(report);
            _absenceService.ReportAbsence(absenceReport);
#endif
            report.ProcessedDateTimestamp = 0;
        }
    }
}
