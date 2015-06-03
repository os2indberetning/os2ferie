namespace Core.DmzModel
{
    using System.Collections.Generic;

    public partial class Rate
    {
        public Rate()
        {
            DriveReports = new HashSet<DriveReport>();
        }

        public int Id { get; set; }

        public string KmRate { get; set; }

        public string TFCode { get; set; }

        public string Type { get; set; }

        public string Year { get; set; }

        public virtual ICollection<DriveReport> DriveReports { get; set; }
    }
}
