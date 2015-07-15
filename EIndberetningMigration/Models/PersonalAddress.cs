using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIndberetningMigration.Models
{
    public class PersonalAddress
    {
        public string Description { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public int ZipCode { get; set; }
        public string Town { get; set; }
        public string CprNumber { get; set; }
    }
}
