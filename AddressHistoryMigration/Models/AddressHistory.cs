using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressHistoryMigration.Models
{
    public class AddressHistory
    {
        public DateTime AktivFra { get; set; }
        public DateTime AktivTil { get; set; }
        public int MaNr { get; set; }
        public string Navn { get; set; }
        public string HjemmeAdresse { get; set; }
        public int HjemmePostNr { get; set; }
        public string HjemmeBy { get; set; }
        public string HjemmeLand { get; set; }
        public string ArbejdsAdresse { get; set; }
        public int ArbejdsPostNr { get; set; }
        public string ArbejdsBy { get; set; }
    }
}
