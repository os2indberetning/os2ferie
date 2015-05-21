using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpdater.Models
{
    public class Employee
    {
        public int? MaNr { get; set; }
        public DateTime? AnsaettelsesDato { get; set; }
        public DateTime? OphoersDato { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public string ADBrugerNavn { get; set; }
        public string Adresse { get; set; }
        public string Stednavn { get; set; }
        public int? PostNr { get; set; }
        public string By { get; set; }
        public string Land { get; set; }
        public string Email { get; set; }
        public string CPR { get; set; }
        public int? LOSOrgId { get; set; }
        public bool Leder { get; set; }
        public string Stillingsbetegnelse { get; set; }
        public long? Omkostningssted { get; set; }
        public string AnsatForhold { get; set; }
        public int? EkstraCiffer { get; set; }
    }
}
