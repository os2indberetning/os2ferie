using System;

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
        public String Description { get; set; }
        public String Year { get; set; }

        public virtual ICollection<DriveReport> DriveReports { get; set; }
    }
}
