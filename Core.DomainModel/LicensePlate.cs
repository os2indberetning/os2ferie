using System;

namespace Core.DomainModel
{
    public class LicensePlate
    {
        public int Id { get; set; }
        public String Plate { get; set; }
        public String Description { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public bool IsPrimary { get; set; }
    }
}
