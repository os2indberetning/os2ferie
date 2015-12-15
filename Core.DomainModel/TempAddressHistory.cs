using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class TempAddressHistory
    {
        public int Id { get; set; }
        public long AktivFra { get; set; }
        public long AktivTil { get; set; }
        public int MaNr { get; set; }
        public string Navn { get; set; }
        public string HjemmeAdresse { get; set; }
        public int HjemmePostNr { get; set; }
        public string HjemmeBy { get; set; }
        public string HjemmeLand { get; set; }
        public string ArbejdsAdresse { get; set; }
        public int ArbejdsPostNr { get; set; }
        public string ArbejdsBy { get; set; }
        public bool HomeIsDirty { get; set; }
        public bool WorkIsDirty { get; set; }
    }
}
