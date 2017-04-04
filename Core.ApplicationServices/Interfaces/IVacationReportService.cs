using System.Web.OData;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IVacationReportService : IReportService<VacationReport>
    {
        void PrepareReport(VacationReport report);
        VacationReport Edit(Delta<VacationReport> delta);
        void Delete(int id);
        void ApproveReport(VacationReport report, Person approver);
        void RejectReport(VacationReport report, Person approver, string comment);
        void DeleteReport(VacationReport report);
        void SendMailIfUserEditedAprovedReport(VacationReport report, string action);
        void SendMailIfRejectedReport(VacationReport report);
    }
}
