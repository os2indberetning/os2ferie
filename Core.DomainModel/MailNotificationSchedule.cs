
using System;

namespace Core.DomainModel
{
    public class MailNotificationSchedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Notified { get; set; }
        public DateTime NextGenerationDate { get; set; }
    }
}
