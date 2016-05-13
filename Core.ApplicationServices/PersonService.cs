using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Core.ApplicationServices.Logger;

namespace Core.ApplicationServices
{
    public class PersonService : IPersonService
    {
        private readonly IGenericRepository<PersonalAddress> _addressRepo;
        private readonly IRoute<RouteInformation> _route;
        private readonly IAddressCoordinates _coordinates;
        private readonly ILogger _logger;

        public PersonService(IGenericRepository<PersonalAddress> addressRepo, IRoute<RouteInformation> route, IAddressCoordinates coordinates, ILogger logger)
        {
            _addressRepo = addressRepo;
            _route = route;
            _coordinates = coordinates;
            _logger = logger;
        }

        /// <summary>
        /// Removes CPR-number from all People in queryable.
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns>List of People with CPR-number removed.</returns>
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

        /// <summary>
        /// Returns AlternativeHome Address for person if one exists.
        /// Otherwise the real home address is returned.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
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
                    .FirstOrDefault(x => x.PersonId == person.Id && x.Type == PersonalAddressType.Home);

            if (home != null) {
                AddCoordinatesToAddressIfNonExisting(home);
            }

            return home;
        }

        /// <summary>
        /// Calculates and sets HomeWorkDistance to each employment belonging to person.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
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
                        try
                        {
                            employment.HomeWorkDistance = _route.GetRoute(DriveReportTransportType.Car, new List<Address>() { homeAddress, workAddress }).Length;
                        }
                        catch (AddressCoordinatesException e)
                        {
                            // Catch the exception to write the error to the log file.
                            _logger.Log(person.FullName + " kan ikke logge på, da der er fejl i vedkommendes arbejds- eller hjemmeadresse.", "web", 1);
                            // Rethrow the exception to allow to front end to display the error aswell.
                            throw e;
                        }
                    }
                }
            }
            return person;
        }

        /// <summary>
        /// Performs a coordinate lookup if Address a does not have coordinates.
        /// </summary>
        /// <param name="a"></param>
        private void AddCoordinatesToAddressIfNonExisting(Address a)
        {
            try
            {
                if (string.IsNullOrEmpty(a.Latitude) || a.Latitude.Equals("0"))
                {
                    var result = _coordinates.GetAddressCoordinates(a);
                    a.Latitude = result.Latitude;
                    a.Longitude = result.Longitude;
                    _addressRepo.Save();
                }
            }
            catch (AddressCoordinatesException ade)
            {

            }
        }
    }
}
