using System;
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
        private readonly IGenericRepository<VacationReport> _reportRepo;
        private readonly IKMDAbsenceService _absenceService;
        private readonly IKMDAbsenceReportBuilder _absenceBuilder;

        public VacationReportService(IGenericRepository<VacationReport> reportRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, IKMDAbsenceService absenceService, IKMDAbsenceReportBuilder absenceBuilder, ILogger logger) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository, logger, SubstituteType.Vacation)
        {
            _reportRepo = reportRepo;
            _absenceService = absenceService;
            _absenceBuilder = absenceBuilder;
        }

        public override VacationReport Create(VacationReport report)
        {
            if (!Validate(report)) throw new Exception("Vacation report has invalid parameters");

            report.ResponsibleLeaderId = GetResponsibleLeaderForReport(report).Id;
            report.ActualLeaderId = GetActualLeaderForReport(report).Id;

            if (report.Comment == null) report.Comment = "";

            _reportRepo.Insert(report);

            _reportRepo.Save();

            return report;
        }

        public override bool Validate(VacationReport report)
        {
            if (report.PersonId == 0) return false;
            if (report.EndTimestamp < report.StartTimestamp) return false;
            if (report.StartTime > report.EndTime &&
                report.StartTimestamp.ToDateTime().Date == report.EndTimestamp.ToDateTime().Date) return false;
            //if (!report.Employment.OrgUnit.HasAccessToVacation) return false;

            return true;
        }

        public override void SendMailToUserAndApproverOfEditedReport(VacationReport report, string emailText, Person admin, string action)
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

            if (report.Comment != null)
                mailContent += Environment.NewLine + "Kommentar: " + report.Comment;

            mailContent += Environment.NewLine + Environment.NewLine
            + "Hvis du mener at dette er en fejl, så kontakt mig da venligst på " + admin.Mail + Environment.NewLine
            + "Med venlig hilsen " + admin.FullName + Environment.NewLine + Environment.NewLine
            + "Besked fra administrator: " + Environment.NewLine + emailText;

            _mailSender.SendMail(report.Person.Mail, "En administrator har ændret i din indberetning.", mailContent);

            _mailSender.SendMail(report.ApprovedBy.Mail, "En administrator har ændret i en indberetning du har godkendt.", mailContent);
        }

        public void ApproveReport(VacationReport report, Person approver, string emailText)
        {
            report.Status = ReportStatus.Accepted;
            report.Comment = emailText;
            report.ClosedDateTimestamp = (DateTime.UtcNow.ToTimestamp());
            report.ApprovedById = approver.Id;

            _reportRepo.Save();

            var absenceReport = _absenceBuilder.Create(report);

#if !DEBUG
            _absenceService.ReportAbsence(absenceReport);
#endif
            report.ProcessedDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _reportRepo.Save();
        }

        public void RejectReport(VacationReport report, Person approver, string emailText)
        {
            report.Status = ReportStatus.Rejected;
            report.Comment = emailText;
            report.ClosedDateTimestamp = (DateTime.UtcNow.ToTimestamp());
            report.ApprovedById = approver.Id;

            _reportRepo.Save();

            var absenceReport = _absenceBuilder.Create(report);

#if !DEBUG
            _absenceService.ReportAbsence(absenceReport);
#endif
            report.ProcessedDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _reportRepo.Save();
        }
    }
}
