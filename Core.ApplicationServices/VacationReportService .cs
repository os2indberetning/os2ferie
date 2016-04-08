using System;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;


namespace Core.ApplicationServices
{
    public class VacationReportService : ReportService<VacationReport>
    {

        private readonly IGenericRepository<VacationReport> _reportRepo;

        public VacationReportService(IGenericRepository<VacationReport> reportRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository, SubstituteType.Vacation)
        {
            _reportRepo = reportRepo;
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
    }
}
