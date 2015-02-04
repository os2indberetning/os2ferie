
using System;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public class DriveReport : Report
    {
        public float Distance { get; set; }
        public float AmountToReimburse { get; set; }
        public string Purpose { get; set; }
        public float KmRate { get; set; }
        public DateTime DriveDate { get; set; }
        public bool FourKmRule { get; set; }
        public bool StartsAtHome { get; set; }
        public bool EndsAtHome { get; set; }
        public String Licenseplate { get; set; }

        public virtual ICollection<DriveReportPoint> DriveReportPoints { get; set; }
    }
}
