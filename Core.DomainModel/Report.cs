
using System;

namespace Core.DomainModel
{
    public enum ReportStatus
    {
        Pending,
        Accepted,
        Rejected,
        Invoiced
    }

    public enum ReportType
    {
        Unknown = -1,
        Drive = 0,
        Vacation = 1
    }

    public abstract class Report
    {
        public int Id { get; set; }
        public ReportStatus Status { get; set; }
        public long CreatedDateTimestamp { get; set; }
        public long EditedDateTimestamp { get; set; }

        // Leader or admin comment when rejecting a report.
        public String Comment { get; set; }
        public long ClosedDateTimestamp { get; set; }
        public long ProcessedDateTimestamp { get; set; }
        public virtual Person ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }
        public int? ResponsibleLeaderId { get; set; }
        public virtual Person ResponsibleLeader { get; set; }
        public int? ActualLeaderId { get; set; }
        public virtual Person ActualLeader { get; set; }

        public ReportType ReportType
        {
            get
            {
                var type = GetType().BaseType;
                if (type == typeof(DriveReport))
                {
                    return ReportType.Drive;
                }
                if (type == typeof(VacationReport))
                {
                    return ReportType.Vacation;
                }
                return ReportType.Unknown;
            }
        }
    }
}
