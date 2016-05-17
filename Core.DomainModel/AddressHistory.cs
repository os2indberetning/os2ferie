using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class AddressHistory
    {
        public int Id { get; set; }
        public int EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }
        public int? WorkAddressId { get; set; }
        public virtual WorkAddress WorkAddress { get; set; }
        public int? HomeAddressId { get; set; }
        public virtual PersonalAddress HomeAddress { get; set; }
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
        public bool IsMigrated { get; set; }
    }
}
