using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using PersonalAddress = EIndberetningMigration.Models.PersonalAddress;

namespace EIndberetningMigration
{
    public class MigrateService
    {
        private readonly IGenericRepository<Core.DomainModel.PersonalAddress> _personalAddressRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly DataProvider _dataProvider;
        private readonly IAddressCoordinates _coordinates;

        public MigrateService(IGenericRepository<Core.DomainModel.PersonalAddress> personalAddressRepo, IGenericRepository<Person> personRepo, DataProvider dataProvider, IAddressCoordinates coordinates)
        {
            _personalAddressRepo = personalAddressRepo;
            _personRepo = personRepo;
            _dataProvider = dataProvider;
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
                        return;
                    }
                    var addresses =
                        _dataProvider.GetPersonalAddressesAsQueryable().Where(x => x.CprNumber == person.CprNumber);
                    HandleMigratePersonalAddresses(addresses);
                }
            }
            else
            {
                // If no initials are given then migrate all personal addresses.
                var addresses = _dataProvider.GetPersonalAddressesAsQueryable();
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
                    continue;
                }

                Console.WriteLine("Migrating personal address " + i + " of " + addresses.Count() + " for " + person.Initials);
                i++;

                if (_personalAddressRepo.AsQueryable().Any(x => x.Description == address.Description && x.PersonId == person.Id))
                {
                    // Skip this address if one already exists with the same description.
                    continue;
                }

                try
                {
                    // Get Latitude and Longitude of address.
                    var coordinates = _coordinates.GetAddressCoordinates(new Address()
                    {
                        StreetName = address.StreetName,
                        StreetNumber = address.StreetNumber,
                        ZipCode = address.ZipCode
                    });

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
                catch (Exception e)
                {

                }


            }
        }
    }
}
