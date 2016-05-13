using System;

namespace Core.DomainModel
{
    public class KMDAbsenceReport
    {
        public DateTime StartDate { get; set; }
        public DateTime? OldStartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? OldStartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? OldEndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? OldEndTime { get; set; }
        public string ExtraData { get; set; }
        public KMDAbsenceOperation KmdAbsenceOperation { get; set; }
        public int EmploymentId { get; set; }
        public VacationType Type { get; set; }
        public VacationType? OldType { get; set; }
    }
}
