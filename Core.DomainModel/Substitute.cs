using System;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public class Substitute
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual Person Leader { get; set; }
        public virtual Person Sub { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
        public virtual OrgUnit OrgUnit{ get; set; }
    }
}
