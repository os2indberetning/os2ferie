
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;

namespace Core.ApplicationServices
{
    public class PersonService : IPersonService
    {
        public IQueryable<Person> ScrubCprFromPersons(IQueryable<Person> queryable)
        {
            var set = queryable.ToList();

            // Add fullname to the resultset
            foreach (var person in set)
            {
                person.CprNumber = "";
            }


            return set.AsQueryable();
        }

        public void AddFullName(IQueryable<Person> persons)
        {
            if (persons == null)
            {
                return;
            }
            foreach (var person in persons)
            {
                person.FullName = person.FirstName;

                if (!string.IsNullOrEmpty(person.MiddleName))
                {
                    person.FullName += " " + person.MiddleName;
                }

                person.FullName += " " + person.LastName;
            }            
        }
    }
}
