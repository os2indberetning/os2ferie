using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Infrastructure.KMDVacationService.Models
{

    public enum Operation
    {
        Create,
        Edit,
        Delete
    }       

    public class AbsenceObject
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
        public Operation Operation { get; set; }
        public int EmploymentId { get; set; }
        public VacationType Type { get; set; }
        public VacationType OldType { get; set; }

        public AbsenceObject Copy()
        {
            return new AbsenceObject
            {
                StartTime = StartTime,
                OldStartDate = OldStartDate,
                OldStartTime = OldStartTime,
                OldEndDate = OldEndDate,
                OldEndTime = OldEndTime,
                EndTime = EndTime,
                EndDate = EndDate,
                StartDate = StartDate,
                ExtraData = ExtraData,
                Operation = Operation,
                Type = Type,
                OldType = OldType,
                EmploymentId = EmploymentId
            };
        }
    }
}
