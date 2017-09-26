using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{

    public enum VacationType
    {
        Regular,
        SixthVacationWeek,
        Care,
        Senior,
        Optional,
        Other
    }

    public class VacationReport : Report
    {
        public long StartTimestamp { get; set; }
        public TimeSpan? StartTime { get; set; }
        public long EndTimestamp { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Purpose { get; set; }
        public string AdditionalData { get; set; }
        public string OptionalText { get; set; }
        public int VacationYear { get; set; }
        public int VacationHours { get; set; }
        public VacationType VacationType { get; set; }
    }
}
