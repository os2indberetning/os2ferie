
using System;

namespace EIndberetningMigration.Models
{
    public class EindDriveReport
    {
        public DateTime Date { get; set; }
        public String Purpose { get; set; }
        public String RegistrationNumber { get; set; }
        public int EmploymentID { get; set; }
        public double AmmountToReimburse { get; set; }
        public string ManualEntryRemark { get; set; }
        public bool IsExtraDistance { get; set; }
        public double ReimbursableDistance { get; set; }
        public int?  ApproverEmploymentID { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int RateID { get; set; }
        public DateTime CreationDate { get; set; }
        public int Id { get; set; }
        public Boolean Approved { get; set; }
        public bool Rejected { get; set; }
        public bool Reimbursed { get; set; }
        public DateTime? ReimbursementDate { get; set; }
        public string CPR { get; set; }
        public string RouteDescription { get; set; }
    }
}
