using System.Collections.Generic;

namespace Core.DomainModel
{
    public class OrgUnit
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool HasAccessToFourKmRule { get; set; }
        public KilometerAllowance DefaultKilometerAllowance { get; set; }
        public virtual WorkAddress Address { get; set; }
        public int AddressId { get; set; }
        public int Level { get; set; }
        public int? ParentId { get; set; }
        public virtual OrgUnit Parent { get; set; }
        public virtual ICollection<OrgUnit> Children { get; set; }
        public virtual ICollection<Substitute> Substitutes { get; set; }
        public virtual ICollection<Employment> Employments { get; set; }

    }
}
