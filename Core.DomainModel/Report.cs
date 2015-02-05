
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
        public long CreatedDateTimestamp { get; set; }
        public long EditedDateTimestamp { get; set; }
        public String Comment { get; set; }
        public long ClosedDateTimestamp { get; set; }
        public long ProcessedDateTimestamp { get; set; }

        public virtual Person Person { get; set; }
        public virtual Employment Employment { get; set; }
    }
}
