using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.Interfaces;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Interface;
using Employment = Core.DmzModel.Employment;

namespace Infrastructure.DmzSync.Services.Impl
{
    public class PersonSyncService : ISyncService
    {
        private IGenericRepository<Profile> _dmzProfileRepo;
        private IGenericRepository<Person> _masterPersonRepo;
        private readonly IPersonService _personService;

        public PersonSyncService(IGenericRepository<Profile> dmzProfileRepo, IGenericRepository<Person> masterPersonRepo, IPersonService personService)
        {
            _dmzProfileRepo = dmzProfileRepo;
            _masterPersonRepo = masterPersonRepo;
            _personService = personService;
        }

        public void SyncFromDmz()
        {
            // We are not interested in migrating profiles from DMZ to os2.
            throw new NotImplementedException();
        }



        public void SyncToDmz()
        {
            var i = 0;
            var personList = _masterPersonRepo.AsQueryable().ToList();
            var max = personList.Count;

            foreach (var person in personList)
            {
                i++;
                if (i%10 == 0)
                {
                    Console.WriteLine("Syncing person " + i + " of " + max);
                }

                

                var homeAddress = _personService.GetHomeAddress(person);

                _dmzProfileRepo.Insert(new Profile
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    HomeLatitude = homeAddress !=  null ? homeAddress.Latitude : "0",
                    HomeLongitude = homeAddress != null ? homeAddress.Longitude : "0",
                });
            }
             _dmzProfileRepo.Save();
            SyncEmployments();

        }

        private void SyncEmployments()
        {
            var i = 0;
            var personList = _masterPersonRepo.AsQueryable().ToList();
            var max = personList.Count;

            foreach (var person in personList)
            {
                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Syncing employments for person " + i + " of " + max);
                }

                var employments = new List<Employment>();

                // Migrate list of employees as the model is not the same in DMZ and OS2.
                foreach (var employment in person.Employments)
                {
                    employments.Add(new Employment
                    {
                        Id = employment.Id,
                        ProfileId = employment.PersonId,
                        EmploymentPosition = employment.Position + " - " + employment.OrgUnit.LongDescription,
                    });
                }
                var pers = _dmzProfileRepo.AsQueryable().First(x => x.Id == person.Id);
                pers.Employments = employments;
            }
            _dmzProfileRepo.Save();
        }


                        

                



        public void ClearDmz()
        {
            foreach (var profile in _dmzProfileRepo.AsQueryable())
            {
                _dmzProfileRepo.Delete(profile);
            }
            _dmzProfileRepo.Save();
        }

    }

}
