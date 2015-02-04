
using System;

namespace Core.DomainModel
{
    public enum ReportStatus
    {
        Accepted,
        Rejected,
        Invoiced,
        Reported
    }

    public class Report
    {
        public int Id { get; set; }
        public ReportStatus status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public String Comment { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime ProcessedDate { get; set; }

        public virtual Person Person { get; set; }
        public virtual Employment Employment { get; set; }
    }
}
