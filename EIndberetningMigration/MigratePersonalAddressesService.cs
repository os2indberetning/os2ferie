using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using PersonalAddress = EIndberetningMigration.Models.EIndPersonalAddress;

namespace EIndberetningMigration
{
    public class MigratePersonalAddressesService
    {
        private readonly IGenericRepository<Core.DomainModel.PersonalAddress> _personalAddressRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly DataProvider _dataProvider;
        private readonly IAddressCoordinates _coordinates;

        public int Unwashed = 0;
        public int UnknownPeople = 0;

        public MigratePersonalAddressesService(IGenericRepository<Core.DomainModel.PersonalAddress> personalAddressRepo, IGenericRepository<Person> personRepo, IAddressCoordinates coordinates)
        {
            _personalAddressRepo = personalAddressRepo;
            _personRepo = personRepo;
            _coordinates = coordinates;
        }

        public void MigratePersonalAddresses(ICollection<string> initialsToMigrate)
        {
            if (initialsToMigrate.Count > 0)
            {
                // If initials are given then only migrate addresses belonging to those initials
                foreach (var initials in initialsToMigrate)
                {
                    var person = _personRepo.AsQueryable().SingleOrDefault(x => x.Initials == initials);
                    if (person == null)
                    {
                        continue;
                    }
                    var addresses =
                        DataProvider.GetPersonalAddressesAsQueryable().Where(x => x.CprNumber == person.CprNumber);
                    HandleMigratePersonalAddresses(addresses);
                }
            }
            else
            {
                // If no initials are given then migrate all personal addresses.
                var addresses = DataProvider.GetPersonalAddressesAsQueryable();
                HandleMigratePersonalAddresses(addresses);
            }

            _personalAddressRepo.Save();
        }

        private void HandleMigratePersonalAddresses(IQueryable<PersonalAddress> addresses)
        {
            var i = 1;
            foreach (var address in addresses)
            {
                var person = _personRepo.AsQueryable().FirstOrDefault(x => x.CprNumber == address.CprNumber);

                if (person == null)
                {
                    // Skip iteration if person does not exist.
                    UnknownPeople++;
                    continue;
                }

                Console.WriteLine("Migrating personal address " + i + " of " + addresses.Count() + " for " + person.Initials);
                i++;

                if (_personalAddressRepo.AsQueryable().Any(x => x.Description == address.Description && x.PersonId == person.Id))
                {
                    // Skip this address if one already exists with the same description.
                    continue;
                }
                Address coordinates;
                try
                {
                    // Get Latitude and Longitude of address.
                    coordinates = _coordinates.GetAddressCoordinates(new Address()
                    {
                        StreetName = address.StreetName,
                        StreetNumber = address.StreetNumber,
                        ZipCode = address.ZipCode
                    });
                }
                catch (AddressCoordinatesException e)
                {
                    //Address laundering will throw an exception if no address could be found.
                    //In that case we just ignore that address.
                    Unwashed++;
                    continue;
                }
                // Insert address to DB.
                _personalAddressRepo.Insert(new Core.DomainModel.PersonalAddress()
                {
                    Description = address.Description,
                    PersonId = person.Id,
                    StreetName = address.StreetName,
                    StreetNumber = address.StreetNumber,
                    ZipCode = address.ZipCode,
                    Town = coordinates.Town,
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude,
                    Type = PersonalAddressType.Standard
                });
            }
        }
    }
}
