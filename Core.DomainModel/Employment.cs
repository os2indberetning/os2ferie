using System;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public class Employment
    {
        public int Id { get; set; }
        public int EmploymentId { get; set; }
        public string Position { get; set; }
        public bool IsLeader { get; set; }
        public long StartDateTimestamp { get; set; }
        public long EndDateTimestamp { get; set; }
        public int EmploymentType { get; set; }
        public int ExtraNumber { get; set; }
        public double WorkDistanceOverride { get; set; }
        public double HomeWorkDistance { get; set; }
        public virtual PersonalAddress AlternativeWorkAddress { get; set; }
        public int? AlternativeWorkAddressId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int OrgUnitId { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }
        public long? CostCenter { get; set; }
    }

}
