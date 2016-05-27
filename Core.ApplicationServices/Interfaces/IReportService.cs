using System.Web.OData;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IReportService<T> where T : Report
    {
        T Create(T report);
        void SendMailIfRejectedReport(int key, Delta<T> delta, Person person);
        Person GetResponsibleLeaderForReport(T report);
        Person GetActualLeaderForReport(T report);
        bool Validate(T report);
        void SendMailToUserAndApproverOfEditedReport(T report, string emailText, Person admin, string action);
    }
}
