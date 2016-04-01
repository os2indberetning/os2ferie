using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class VacationBalance
    {
        public int Id { get; set; }
        public int Year { get; set; }

        // Set at the beginning fo the vacation year
        public int TotalVacationHours { get; set; }

        // From OPUS
        public int VacationHours { get; set; }
        public int TransferredHours { get; set; }
        public int FreeVacationHours { get; set; }
        public long UpdatedAt { get; set; }


        public int EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }
}
