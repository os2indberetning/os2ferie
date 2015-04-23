using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpdater.Models
{
    public class Organisation
    {
        public int LOSOrgId { get; set; }
        public int? ParentLosOrgId { get; set; }
        public string KortNavn { get; set; }
        public string Navn { get; set; }
        public string Gade { get; set; }
        public string Stednavn { get; set; }
        public int? Postnr { get; set; }
        public string By { get; set; }
        public long? Omkostningssted { get; set; }
        public int Level { get; set; }
    }
}
