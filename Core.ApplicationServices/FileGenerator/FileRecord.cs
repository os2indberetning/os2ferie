using System;
using System.Configuration;
using System.Text;
using Core.DomainModel;

namespace Core.ApplicationServices.FileGenerator
{
    public class FileRecord
    {
        public string CprNr { get; set; }
        public DateTime Date { get; set; }
        public double ReimbursementDistance { get; set; }
        public string TFCode { get; set; }
        public int EmploymentType { get; set; }
        public int ExtraNumber { get; set; }
        public string Account { get; set; }
        public bool IsAdministrativeWorker { get; set; }

        public FileRecord(DriveReport report, string ownerCpr)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var reportDate = dtDateTime.AddSeconds(report.DriveDateTimestamp).ToLocalTime();

            CprNr = ownerCpr;
            Date = reportDate;
            EmploymentType = report.Employment.EmploymentType;
            ExtraNumber = report.Employment.ExtraNumber;
            ReimbursementDistance = report.Distance;
            TFCode = report.TFCode;
            IsAdministrativeWorker =
                report.Employment.CostCenter.ToString()
                    .StartsWith(getSetting("PROTECTED_AdministrativeCostCenterPrefix"));
            if ( ! string.IsNullOrWhiteSpace(report.AccountNumber) && report.AccountNumber.Length == 10)
            {
                Account = report.AccountNumber;
            }
            else if (IsAdministrativeWorker)
            {
                Account = getSetting("PROTECTED_AdministrativeAccount");
            }
        }

        public new string ToString()
        {
            var date  = Date.Day.ToString().PadLeft(2, '0') + Date.Month.ToString().PadLeft(2, '0') + Date.Year.ToString()[2] + Date.Year.ToString()[3];
            var distance = ReimbursementDistance;

            var builder = new StringBuilder();

            builder.Append(getSetting("PROTECTED_KMDStaticNr"));        //KMD statisk identifier
            builder.Append(getSetting("PROTECTED_CommuneNr"));          //Syddjurs' KommuneNr.
            builder.Append(EmploymentType);                             //Ansættelsesform (0,1,3)
            builder.Append(CprNr);                                      //CPR Nr.
            builder.Append(ExtraNumber);                                //Ekstra ciffer (0,1,2,3 nn)
            builder.Append(TFCode);                                     //TF Kode
            builder.Append(DistanceStringBuilder(distance.ToString())); //Kørte Km
            builder.Append(getSetting("PROTECTED_KMDReservedNr"));      //KMD reserverede pladser

            if ( ! string.IsNullOrWhiteSpace(Account) && Account.Length == 10)
            {
                builder.Append("   ");                                  //3 spaces
                builder.Append(Account);                                //Account, either administrative or other account chosend when the report was approved
            } 
            else 
            {
                builder.Append("             ");                        //13 whitespaces
            }
            builder.Append(date);                                       //Dato

            return builder.ToString();
        }

        private static string DistanceStringBuilder(string distance)
        {
            var beforeComma = distance;
            var afterComma = "";

            distance = distance.Replace('.', ',');

             if (distance.Contains(","))
            {
                var index = distance.IndexOf(",");

                beforeComma = distance.Substring(0, index);
                afterComma = distance.Substring(index + 1, distance.Length - (index + 1));
                if (afterComma.Length > 2)
                {
                    afterComma = afterComma.Substring(0, 2);
                }
            }

            while (beforeComma.Length < 4)
            {
                beforeComma = "0" + beforeComma;
            }

            while (afterComma.Length < 2)
            {
                afterComma = afterComma + "0";
            }

            return beforeComma + afterComma;
        }

        private string getSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
