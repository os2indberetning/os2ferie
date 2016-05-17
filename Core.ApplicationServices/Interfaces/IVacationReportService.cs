using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IVacationReportService : IReportService<VacationReport>
    {
        void PrepareReport(VacationReport report);
        void ApproveReport(VacationReport report, Person approver, string emailText);
    }
}
