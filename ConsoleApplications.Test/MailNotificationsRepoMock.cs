using System;
using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace ConsoleApplications.Test.Mailer
{
    public class MailNotificationsRepoMock : GenericRepositoryMock<MailNotificationSchedule>
    {
        public MailNotificationSchedule noti1 = new MailNotificationSchedule()
        {
            Id = 1,
            DateTimestamp = ToUnixTime(DateTime.Now),
            Notified = false,
            Repeat = true
        };

        public MailNotificationSchedule noti2 = new MailNotificationSchedule()
        {
            Id = 2,
            DateTimestamp = ToUnixTime(DateTime.Now.AddDays(1)),
            Notified = false,
            Repeat = true
        };

        protected override List<MailNotificationSchedule> Seed()
        {

            return new List<MailNotificationSchedule>
            {
                noti1,
                noti2,
            };
        }

        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
