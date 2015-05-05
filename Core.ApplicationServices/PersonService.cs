
using System.Collections.Generic;
using System.Linq;
using System.Web.OData.Routing;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Ninject;

namespace Core.ApplicationServices
{
    public class PersonService : IPersonService
    {
        private readonly IGenericRepository<PersonalAddress> _addressRepo;
        private readonly IRoute<RouteInformation> _route;

        public PersonService(IGenericRepository<PersonalAddress> addressRepo, IRoute<RouteInformation> route)
        {
            _addressRepo = addressRepo;
            _route = route;
        }

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

                person.FullName += " [" + person.Initials + "]";
            }            
        }

        public virtual PersonalAddress GetHomeAddress(Person person)
        {
            var hasAlternative = _addressRepo.AsQueryable()
                    .FirstOrDefault(x => x.PersonId == person.Id && x.Type == PersonalAddressType.AlternativeHome);

            if (hasAlternative != null)
            {
                return hasAlternative;
            }

            var home = _addressRepo.AsQueryable()
                    .First(x => x.PersonId == person.Id && x.Type == PersonalAddressType.Home);

            AddCoordinatesToAddressIfNonExisting(home);


            return home;
        }

        private void AddCoordinatesToAddressIfNonExisting(Address a)
        {
            if (string.IsNullOrEmpty(a.Latitude) || a.Latitude.Equals("0"))
            {
                var coordinates = NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>();
                var result = coordinates.GetAddressCoordinates(a);
                a.Latitude = result.Latitude;
                a.Longitude = result.Longitude;
                _addressRepo.Save();

            }
        }
    }
}
