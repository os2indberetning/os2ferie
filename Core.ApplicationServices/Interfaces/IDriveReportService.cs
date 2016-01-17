using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IDriveReportService
    {
        DriveReport Create(DriveReport report);
        void SendMailIfRejectedReport(int key, Delta<DriveReport> delta);

        Person GetResponsibleLeaderForReport(DriveReport driveReport);
        Person GetActualLeaderForReport(DriveReport driveReport);
        bool Validate(DriveReport report);

        void SendMailToUserAndApproverOfEditedReport(DriveReport report, string emailText, Person admin, string action);
    }
}
