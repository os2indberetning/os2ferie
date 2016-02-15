using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModel
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CprNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public bool RecieveMail { get; set; }
        public double DistanceFromHomeToBorder { get; set; }
        public string Initials { get; set; }
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSubstitute { get; set; }
        public bool IsActive { get; set; }
        public bool HasAppPassword { get; set; }

        public virtual ICollection<PersonalAddress> PersonalAddresses { get; set; }
        public virtual ICollection<PersonalRoute> PersonalRoutes { get; set; }
        public virtual ICollection<LicensePlate> LicensePlates { get; set; }
        public virtual ICollection<MobileToken> MobileTokens { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Employment> Employments { get; set; }

        public virtual ICollection<Substitute> Substitutes { get; set; }
        public virtual ICollection<Substitute> SubstituteFor { get; set; }
        public virtual ICollection<Substitute> SubstituteLeaders { get; set; }
    }
}
