namespace Core.DmzModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class Profile
    {
        public Profile()
        {
            DriveReports = new HashSet<DriveReport>();
            Employments = new HashSet<Employment>();
            Tokens = new HashSet<Token>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Initials { get; set; }

        public string FullName { get; set; }

        public string HomeLatitude { get; set; }

        public string HomeLongitude { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<DriveReport> DriveReports { get; set; }

        public virtual ICollection<Employment> Employments { get; set; }

        public virtual ICollection<Token> Tokens { get; set; }
    }
}
