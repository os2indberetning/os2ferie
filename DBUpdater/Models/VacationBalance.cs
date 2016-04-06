using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpdater.Models
{
    public class VacationBalance
    {
        public string KommuneInfo { get; set; }
        public string CPR { get; set; }

        public string ANS_FORHOLD_NR { get; set; }
        public string Afloenningsform { get; set; }
        public string Ferieoptjeningsaar { get; set; }
        public string DatoForSaldo { get; set; }
        public int? FerieTimer_MLoen { get; set; }
        public int? EVTFerieDage_MLoen { get; set; }
        public int? FerieTimer_ULoen { get; set; }
        public int? EVTFerieDage_ULoen { get; set; }
        public int? Overfoertetimer { get; set; }
        public int? EvtOverfoertedage { get; set; }
        public int? FERIEFRIDAGSTIMER_SUM { get; set; }
        public double? FerieTimer_MLoenDec { get; set; }
        public double? FerieTimer_ULoenDec { get; set; }
        public double? OverfoertetimerDec { get; set; }
        public double? FERIEFRIDAGSTIMER_SUMDec { get; set; }
        public DateTime? Opdateringsdato { get; set; }

    }
}
