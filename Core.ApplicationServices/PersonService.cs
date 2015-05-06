
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.OData.Routing;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Microsoft.Ajax.Utilities;
using Ninject;

namespace Core.ApplicationServices
{
    public class PersonService : IPersonService
    {
        private readonly IGenericRepository<PersonalAddress> _addressRepo;
        private readonly IRoute<RouteInformation> _route;
        private readonly IGenericRepository<Employment> _emplRepo;

        public PersonService(IGenericRepository<PersonalAddress> addressRepo, IRoute<RouteInformation> route, IGenericRepository<Employment> emplRepo)
        {
            _addressRepo = addressRepo;
            _route = route;
            _emplRepo = emplRepo;
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

        public Person AddHomeWorkDistanceToEmployments(Person person)
        {
            // Get employments for person
            // Get alternative home address.
            var homeAddress = person.PersonalAddresses.AsQueryable().FirstOrDefault(x => x.Type == PersonalAddressType.AlternativeHome);
            // Get primary home address if alternative doesnt exist.
            homeAddress = homeAddress ?? person.PersonalAddresses.AsQueryable().FirstOrDefault(x => x.Type == PersonalAddressType.Home);
            foreach (var employment in person.Employments)
            {
                if (employment.WorkDistanceOverride > 0)
                {
                    employment.HomeWorkDistance = employment.WorkDistanceOverride;
                }
                else
                {
                    var workAddress = employment.AlternativeWorkAddress;
                    workAddress = workAddress ?? employment.OrgUnit.Address;
                    if (homeAddress != null && workAddress != null)
                    {
                        employment.HomeWorkDistance = _route.GetRoute(new List<Address>() { homeAddress, workAddress }).Length;
                    }
                }
            }
            return person;
        }

        public IQueryable<Person> AddHomeWorkDistanceToEmployments(IQueryable<Person> people)
        {
            foreach (var person in people.ToList())
            {
                AddHomeWorkDistanceToEmployments(person);
            }
            return people;
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
