
using System;

namespace Core.DomainModel
{
    public class Rate
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public String TFCode { get; set; }
        public float KmRate { get; set; }
        public String Type { get; set; }
        public bool Active { get; set; }
    }
}
