
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

    public enum DriveReportTransportType
    {
        Car,
        Bike
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
        public string LicensePlate { get; set; }
        public string FullName { get; set; }
        public string AccountNumber { get; set; }
        public string TFCode { get; set; }
        public KilometerAllowance KilometerAllowance { get; set; }
        public bool IsFromApp { get; set; }
        public string UserComment { get; set; }
        public string RouteGeometry { get; set; }
        public bool? IsExtraDistance { get; set; }
        public bool? IsOldMigratedReport { get; set; }


        public virtual ICollection<DriveReportPoint> DriveReportPoints { get; set; }
    }
}
