using System;
using System.Collections.Generic;
using Core.DomainModel;

namespace Core.ApplicationServices.MailerService.Interface
{
    public interface IMailService
    {
        void SendMails(DateTime payRoleDateTime);
        IEnumerable<Report> GetLeadersWithPendingReportsMails();
    }
}
