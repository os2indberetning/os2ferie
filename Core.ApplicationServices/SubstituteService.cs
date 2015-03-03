
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;

namespace Core.ApplicationServices
{
    public class SubstituteService
    {
        public void ScrubCprFromPersons(IQueryable<Substitute> subs)
        {
            foreach (var sub in subs)
            {
                sub.Sub.CprNumber = "";
                sub.Leader.CprNumber = "";

                foreach (var person in sub.Persons)
                {
                    person.CprNumber = "";
                }
            }
        }

        public void AddFullName(IQueryable<Substitute> substitutes)
        {
            var subs = substitutes.ToList();

            foreach (var sub in subs)
            {
                sub.Sub.FullName = sub.Sub.FirstName;

                if (!string.IsNullOrEmpty(sub.Sub.MiddleName))
                {
                    sub.Sub.FullName += " " + sub.Sub.MiddleName;
                }

                sub.Sub.FullName += " " + sub.Sub.LastName;


                sub.Leader.FullName = sub.Leader.FirstName;

                if (!string.IsNullOrEmpty(sub.Leader.MiddleName))
                {
                    sub.Leader.FullName += " " + sub.Leader.MiddleName;
                }

                sub.Leader.FullName += " " + sub.Leader.LastName;

                foreach (var person in sub.Persons)
                {
                    person.FullName = person.FirstName;

                    if (!string.IsNullOrEmpty(person.MiddleName))
                    {
                        person.FullName += " " + person.MiddleName;
                    }

                    person.FullName += " " + person.LastName;

                }

            }

            substitutes = subs.AsQueryable();
        }
    }
}
