using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpdater.Models
{
    public class VacationBalance
    {
        public string MunicipalityInfo { get; set; }
        public string SocialSecurityNumber { get; set; }

        public string EmploymentRelationshipNumber { get; set; }
        public string SalaryKind { get; set; }
        public string VacationEarnedYear { get; set; }
        public string BalanceDate { get; set; }
        public int? VacationHours_WithPay { get; set; }
        public int? PossibleVacationDays_WithPay { get; set; }
        public int? VacationHours_WithoutPay { get; set; }
        public int? PossibleVacationDays_WithoutPay { get; set; }
        public int? TransferredVacationHours { get; set; }
        public int? PossibleTransferredVacationDays { get; set; }
        public int? FreeVacationHours_Total { get; set; }
        public double? VacationHours_WithPayDec { get; set; }
        public double? VacationHours_WithoutPayDec { get; set; }
        public double? TransferredVacationHoursDec { get; set; }
        public double? FreeVacationHours_TotalDec { get; set; }
        public DateTime? OpdateDate { get; set; }

    }
}
