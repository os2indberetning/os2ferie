
using System;

namespace Core.DomainModel
{
    public class Rate
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public float KmRate { get; set; }
        public int TypeId { get; set; }
        public virtual RateType Type { get; set; }
        public bool Active { get; set; }
    }
}
