
using System;

namespace Core.DomainModel
{
    public class MailNotificationSchedule
    {
        public int Id { get; set; }
        public long DateTimestamp { get; set; }
        public long PayRoleTimestamp { get; set; }
        public bool Notified { get; set; }
        public bool Repeat { get; set; }
    }
}
