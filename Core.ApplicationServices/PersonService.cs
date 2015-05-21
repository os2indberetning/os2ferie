
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
        private readonly IAddressCoordinates _coordinates;

        public PersonService(IGenericRepository<PersonalAddress> addressRepo, IRoute<RouteInformation> route, IAddressCoordinates coordinates)
        {
            _addressRepo = addressRepo;
            _route = route;
            _coordinates = coordinates;
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

        public virtual PersonalAddress GetHomeAddress(Person person)
        {
            var alternative = _addressRepo.AsQueryable()
                    .FirstOrDefault(x => x.PersonId == person.Id && x.Type == PersonalAddressType.AlternativeHome);

            if (alternative != null)
            {
                AddCoordinatesToAddressIfNonExisting(alternative);
                return alternative;
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
                    var workAddress = employment.AlternativeWorkAddress ?? new PersonalAddress()
                    {
                        StreetName = employment.OrgUnit.Address.StreetName,
                        StreetNumber = employment.OrgUnit.Address.StreetNumber,
                        ZipCode = employment.OrgUnit.Address.ZipCode,
                        Town = employment.OrgUnit.Address.Town
                    };
                    if (homeAddress != null && workAddress != null)
                    {
                        employment.HomeWorkDistance = _route.GetRoute(new List<Address>() { homeAddress, workAddress }).Length;
                    }
                }
            }
            return person;
        }

        private void AddCoordinatesToAddressIfNonExisting(Address a)
        {
            if (string.IsNullOrEmpty(a.Latitude) || a.Latitude.Equals("0"))
            {
                var result = _coordinates.GetAddressCoordinates(a);
                a.Latitude = result.Latitude;
                a.Longitude = result.Longitude;
                _addressRepo.Save();

            }
        }
    }
}
