using System;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.KMDVacationService;
using Infrastructure.KMDVacationService.Interfaces;


namespace Core.ApplicationServices
{
    public interface IVacationReportService : IReportService<VacationReport>
    {
        void ApproveReport(VacationReport report, Person approver, string emailText);
    }

    public class VacationReportService : ReportService<VacationReport>, IVacationReportService
    {

        private readonly IGenericRepository<VacationReport> _reportRepo;
        private readonly IReportAbsence _reportAbsence;
        private readonly ILogger _logger;

        public VacationReportService(IGenericRepository<VacationReport> reportRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, IReportAbsence reportAbsence, ILogger logger) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository, SubstituteType.Vacation)
        {
            _reportRepo = reportRepo;
            _reportAbsence = reportAbsence;
            _logger = logger;
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
            //if (!report.Employment.OrgUnit.HasAccessToVacation) return false;
         
            return true;
        }

        public override void SendMailToUserAndApproverOfEditedReport(VacationReport report, string emailText, Person admin, string action)
        {
            throw new NotImplementedException();
        }

        public void ApproveReport(VacationReport report, Person approver, string emailText)
        {

            report.Status = ReportStatus.Accepted;
            report.Comment = emailText;
            report.ClosedDateTimestamp = (DateTime.UtcNow.ToTimestamp());
            report.ApprovedById = approver.Id;

            try
            {
                _reportRepo.Save();
            }
            catch (Exception)
            {
                _logger.Log("Forsøg på at godkende ferieindberetning fejlede. Rapporten er ikke godkendt.", "web", 3);
                throw;
            }

            

            

            //var response = _reportAbsence.Set_Absence_Plus()

            report.ProcessedDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _reportRepo.Save();



        }
    }
}
