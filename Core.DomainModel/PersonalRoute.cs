
using System.Collections.Generic;

namespace Core.DomainModel
{
    public class PersonalRoute
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Point> Points { get; set; } 
    }
}
