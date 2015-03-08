using System.Collections.Generic;

namespace Core.ApplicationServices.MailerService.Interface
{
    public interface IMailService
    {
        void SendMails();
        IEnumerable<string> GetLeadersWithPendingReportsMails();
    }
}
