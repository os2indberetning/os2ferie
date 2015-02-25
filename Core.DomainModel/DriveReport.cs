
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace Core.DomainModel
{

    public enum KilometerAllowance
    {
        Calculated,
        Read,
        CalculatedWithoutExtraDistance
    }

    public class DriveReport : Report
    {
        public double Distance { get; set; }
        public double AmountToReimburse { get; set; }
        public string Purpose { get; set; }
        public double KmRate { get; set; }
        public long DriveDateTimestamp { get; set; }
        public bool FourKmRule { get; set; }
        public bool StartsAtHome { get; set; }
        public bool EndsAtHome { get; set; }
        public string Licenseplate { get; set; }
        public string Fullname { get; set; }
        public string AccountNumber { get; set; }
        public KilometerAllowance KilometerAllowance { get; set; }

        public ICollection<DriveReportPoint> DriveReportPoints { get; set; }
    }
}
