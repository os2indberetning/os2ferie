using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Logger;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace Mail
{
    public class ConsoleMailerService
    {
        private IMailService _mailService;
        private IGenericRepository<MailNotificationSchedule> _repo;
        private ILogger _logger;

        public ConsoleMailerService(IMailService mailService, IGenericRepository<MailNotificationSchedule> repo, ILogger logger)
        {
            _mailService = mailService;
            _repo = repo;
            _logger = logger;
            _logger.Log("MailService startet.", "mail");
        }

        /// <summary>
        /// Checks if there are any mail notifications scheduled for now. If there is mails will be sent. Otherwise nothing happens.
        /// </summary>
        public void RunMailService()
        {
            var startOfDay = ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            var endOfDay = ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
            var notifications = _repo.AsQueryable().Where(r => r.DateTimestamp >= startOfDay && r.DateTimestamp <= endOfDay && !r.Notified);

            if (notifications.Any())
            {
                Console.WriteLine("Forsøger at sende emails.");
                _logger.Log("Attempting to send emails.", "mail");
                foreach (var notification in notifications.ToList())
                {
                    if (notification.Repeat)
                    {
                        var newDateTime = ToUnixTime(FromUnixTime(notification.DateTimestamp).AddMonths(1));
                        _repo.Insert(new MailNotificationSchedule()
                        {
                            DateTimestamp = newDateTime,
                            Notified = false,
                            Repeat = true
                        });
                    }
                    notification.Notified = true;

                    AttemptSendMails(_mailService,FromUnixTime(notification.PayRoleTimestamp), 2);
                }



                _repo.Save();
            }
            else
            {
                Console.WriteLine("Ingen email-adviseringer fundet! Programmet lukker om 3 sekunder.");
                Console.WriteLine(Environment.CurrentDirectory);
                _logger.Log("No mail notifications found.", "mail");
                Thread.Sleep(3000);
            }
        }



        /// <summary>
        /// Attempts to send mails to leaders with pending reports to be approved.
        /// </summary>
        /// <param name="service">IMailService to use for sending mails.</param>
        /// <param name="timesToAttempt">Number of times to attempt to send emails.</param>
        public void AttemptSendMails(IMailService service, DateTime payRoleDateTime, int timesToAttempt)
        {
            if (timesToAttempt > 0)
            {
                try
                {
                    service.SendMails(payRoleDateTime);
                }
                catch (System.Net.Mail.SmtpException)
                {
                    Console.WriteLine("Kunne ikke oprette forbindelse til SMTP-Serveren. Forsøger igen...");
                    _logger.Log("Unable to connect to SMTP-server. Retrying." , "mail");
                    AttemptSendMails(service, payRoleDateTime, timesToAttempt - 1);
                }
            }
            else
            {
                Console.WriteLine("Alle forsøg fejlede. Programmet lukker om 3 sekunder.");
                _logger.Log("All attempts failed. Program will shut down in 3 seconds.", "mail");
                Thread.Sleep(3000);

            }
        }

        /// <summary>
        /// Converts DateTime to timestamp
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <returns>long timestamp</returns>
        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        /// <summary>
        /// Converts timestamp to datetime
        /// </summary>
        /// <param name="unixTime">Timestamp to convert</param>
        /// <returns>DateTime</returns>
        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
