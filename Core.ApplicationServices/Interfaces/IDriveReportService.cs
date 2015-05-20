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
        IQueryable<DriveReport> AttachResponsibleLeader(IQueryable<DriveReport> driveReport);
        IQueryable<DriveReport> FilterByLeader(IQueryable<DriveReport> repo, int leaderId, bool getReportsWhereSubExists = false);
        Person GetResponsibleLeaderForReport(DriveReport driveReport);
        bool Validate(DriveReport report);
    }
}
