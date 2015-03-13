
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;

namespace Core.ApplicationServices
{
    public class SubstituteService : ISubstituteService
    {
        public void ScrubCprFromPersons(IQueryable<Substitute> subs)
        {
            foreach (var sub in subs)
            {
                sub.Sub.CprNumber = "";
                sub.Leader.CprNumber = "";


                sub.Person.CprNumber = "";

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
                sub.Person.FullName = sub.Person.FirstName;

                if (!string.IsNullOrEmpty(sub.Person.MiddleName))
                {
                    sub.Person.FullName += " " + sub.Person.MiddleName;
                }

                sub.Person.FullName += " " + sub.Person.LastName;

            }

            substitutes = subs.AsQueryable();
        }
    }
}
