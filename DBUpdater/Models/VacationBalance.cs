using System;

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
        public int? VacationHoursWithPay { get; set; }
        public int? PossibleVacationDaysWithPay { get; set; }
        public int? VacationHoursWithoutPay { get; set; }
        public int? PossibleVacationDaysWithoutPay { get; set; }
        public int? TransferredVacationHours { get; set; }
        public int? PossibleTransferredVacationDays { get; set; }
        public int? FreeVacationHoursTotal { get; set; }
        public double? VacationHoursWithPayDec { get; set; }
        public double? VacationHoursWithoutPayDec { get; set; }
        public double? TransferredVacationHoursDec { get; set; }
        public double? FreeVacationHoursTotalDec { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
