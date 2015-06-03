using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
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

        public MailService(IGenericRepository<DriveReport> driveRepo, IGenericRepository<Substitute> subRepo, IMailSender mailSender)
        {
            _driveRepo = driveRepo;
            _subRepo = subRepo;
            _mailSender = mailSender;
        }

        /// <summary>
        /// Sends an email to all leaders with pending reports to be approved.
        /// </summary>
        public void SendMails()
        {
            var mailAddresses = GetLeadersWithPendingReportsMails();
            foreach (var mailAddress in mailAddresses)
            {
                _mailSender.SendMail(mailAddress,
                    ConfigurationManager.AppSettings["PROTECTED_MAIL_SUBJECT"],
                     ConfigurationManager.AppSettings["PROTECTED_MAIL_BODY"]);
            }

        }

        /// <summary>
        /// Gets the email address of all leaders that have pending reports to be approved.
        /// </summary>
        /// <returns>List of email addresses.</returns>
        public IEnumerable<string> GetLeadersWithPendingReportsMails()
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var leaders =
                _driveRepo.AsQueryable()
                    .Where(r => r.Status == ReportStatus.Pending)
                    .Select(r => r.Employment.OrgUnit)
                    .Distinct()
                    .SelectMany(x => x.Employments.Where(y => y.IsLeader && y.Person.RecieveMail).Select(e => e.Person)).ToList();

            // Convert list of leaders to hashset to remove dupes.
            var leadersNoDupe = new HashSet<Person>(leaders);

            var substitutes = _subRepo.AsQueryable().Include(x => x.Sub).ToList();
            var leadersOrSubs = new HashSet<String>();

            // Check if the leaders have substitutes.
            foreach (var leader in leadersNoDupe)
            {
                leadersOrSubs.Add(substitutes.Any(s => s.Person == leader && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp)
                    ? substitutes.First(s => s.Person == leader && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp).Sub.Mail
                    : leader.Mail);
            }

            return leadersOrSubs;
        }
    }
}
