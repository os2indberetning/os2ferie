using System;
using System.Configuration;
using System.Text;
using Core.DomainModel;

namespace FileGenerator
{
    public class FileRecord
    {
        public string CprNr { get; set; }
        public DateTime Date { get; set; }
        public double ReimbursementDistance { get; set; }
        public string TFCode { get; set; }
        public int EmploymentId { get; set; }
        public int EmploymentTypeId { get; set; }

        public FileRecord(DriveReport report, string ownerCpr)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var reportDate = dtDateTime.AddSeconds(report.DriveDateTimestamp).ToLocalTime();
            var date = new DateTime(reportDate.Year, reportDate.Month, 1);

            CprNr = ownerCpr;
            Date = date;
            EmploymentId = report.Employment.EmploymentId;
            EmploymentTypeId = report.Employment.EmploymentType;
            ReimbursementDistance = report.Distance;
            TFCode = report.TFCode;
        }

        public new string ToString()
        {
            var date = Date.GetDateTimeFormats()[1]; //Get correct time format
            date = date.Replace("-", "");
            var distance = ReimbursementDistance;

            var builder = new StringBuilder();

            builder.Append(getSetting("KMDStaticNr"));               //KMD statisk identifier
            builder.Append(getSetting("CommuneNr"));                 //Syddjurs' KommuneNr.
            builder.Append(EmploymentId);                        //Ansættelsesform (0,1,3)
            builder.Append(CprNr);                              //CPR Nr.
            builder.Append(EmploymentTypeId);                    //Ekstra ciffer (0,1,2,3 nn)
            builder.Append(TFCode);                              //TF Kode
            builder.Append(DistanceStringBuilder(distance.ToString())); //Kørte Km
            builder.Append(getSetting("KMDReservedNr"));             //KMD reserverede pladser
            builder.Append("             ");                            //13 whitespaces
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
