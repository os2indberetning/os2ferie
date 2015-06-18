using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailService : IMailService
    {
        private readonly IGenericRepository<DriveReport> _driveRepo;
        private readonly IGenericRepository<Substitute> _subRepo;
        private readonly IMailSender _mailSender;
        private readonly IDriveReportService _driveReportService;

        public MailService(IGenericRepository<DriveReport> driveRepo, IGenericRepository<Substitute> subRepo, IMailSender mailSender, IDriveReportService driveReportService)
        {
            _driveRepo = driveRepo;
            _subRepo = subRepo;
            _mailSender = mailSender;
            _driveReportService = driveReportService;
        }

        /// <summary>
        /// Sends an email to all leaders with pending reports to be approved.
        /// </summary>
        public void SendMails(DateTime payRoleDateTime)
        {
            var mailAddresses = GetLeadersWithPendingReportsMails();

            var mailBody = ConfigurationManager.AppSettings["PROTECTED_MAIL_BODY"];
            mailBody = mailBody.Replace("####", payRoleDateTime.ToString("dd-MM-yyyy"));

            foreach (var mailAddress in mailAddresses)
            {
                _mailSender.SendMail(mailAddress, ConfigurationManager.AppSettings["PROTECTED_MAIL_SUBJECT"], mailBody);
            }

        }

        /// <summary>
        /// Gets the email address of all leaders that have pending reports to be approved.
        /// </summary>
        /// <returns>List of email addresses.</returns>
        public IEnumerable<string> GetLeadersWithPendingReportsMails()
        {
            var approverEmails = new HashSet<String>();

            var reports = _driveRepo.AsQueryable().Where(r => r.Status == ReportStatus.Pending).ToList();
            foreach (var driveReport in reports)
            {
                approverEmails.Add(_driveReportService.GetResponsibleLeaderForReport(driveReport).Mail);
            }

            return approverEmails;
        }
    }
}
