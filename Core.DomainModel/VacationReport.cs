using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{

    public enum VacationType
    {
        Regular,
        SixthVacationWeek,
        Other
    }

    public class VacationReport : Report
    {
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
        public int VacationYear { get; set; }
        public int VacationHours { get; set; }

        public VacationType VacationType { get; set; }
    }
}
