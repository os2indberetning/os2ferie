using System.Web.OData;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IVacationReportService : IReportService<VacationReport>
    {
        void PrepareReport(VacationReport report);
        VacationReport Edit(Delta<VacationReport> delta);
        void Delete(int id);
        void ApproveReport(VacationReport report, Person approver, string emailText);
        void RejectReport(VacationReport report, Person approver, string emailText);
    }
}
