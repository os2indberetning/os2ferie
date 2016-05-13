using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class VacationBalance
    {
        public int Id { get; set; }
        public int Year { get; set; }

        // Not available from OPUS yet
        public double TotalVacationHours { get; set; }

        // From OPUS
        public double VacationHours { get; set; }
        public double TransferredHours { get; set; }
        public double FreeVacationHours { get; set; }
        public long UpdatedAt { get; set; }


        public int EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
