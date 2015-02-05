
using System;

namespace Core.DomainModel
{
    public class MailNotificationSchedule
    {
        public int Id { get; set; }
        public long DateTimestamp { get; set; }
        public bool Notified { get; set; }
        public long NextGenerationDateTimestamp { get; set; }
    }
}
