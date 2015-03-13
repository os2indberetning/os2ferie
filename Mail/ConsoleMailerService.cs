using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.ApplicationServices;
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

        public ConsoleMailerService(IMailService mailService, IGenericRepository<MailNotificationSchedule> repo)
        {
            _mailService = mailService;
            _repo = repo;
        }

        public void RunMailService()
        {
            var startOfDay = ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            var endOfDay = ToUnixTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
            var notifications = _repo.AsQueryable().Where(r => r.DateTimestamp >= startOfDay && r.DateTimestamp <= endOfDay && !r.Notified);

            if (notifications.Any())
            {
                Console.WriteLine("Forsøger at sende emails.");
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
                }
                _repo.Save();
                AttemptSendMails(_mailService, 2);

            }
            else
            {
                Console.WriteLine("Ingen email-adviseringer fundet! Programmet lukker om 3 sekunder.");
                Thread.Sleep(3000);
            }
        }

        
            

        public void AttemptSendMails(IMailService service, int timesToAttempt)
        {
            if (timesToAttempt > 0)
            {
                try
                {
                    service.SendMails();
                }
                catch (System.Net.Mail.SmtpException)
                {
                    Console.WriteLine("Kunne ikke oprette forbindelse til SMTP-Serveren. Forsøger igen...");
                    AttemptSendMails(service, timesToAttempt - 1);
                }
            }
            else
            {
                Console.WriteLine("Alle forsøg fejlede. Programmet lukker om 3 sekunder.");
                Thread.Sleep(3000);
            }
        }

        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
