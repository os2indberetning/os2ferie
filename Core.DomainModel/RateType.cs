using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class RateType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public String TFCode { get; set; }
        public bool RequiresLicensePlate { get; set; }
        public bool IsBike { get; set; }
    }
}
