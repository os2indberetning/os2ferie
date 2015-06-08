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

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SyncFromDmz()
        {
            // We are not interested in migrating profiles from DMZ to os2.
            throw new NotImplementedException();
        }


        /// <summary>
        /// Syncs all People from OS2 database to DMZ database.
        /// </summary>
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

                var profile = new Profile
                    {
                        Id = person.Id,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        HomeLatitude = homeAddress != null ? homeAddress.Latitude : "0",
                        HomeLongitude = homeAddress != null ? homeAddress.Longitude : "0",
                        Initials = person.Initials,
                        FullName = person.FullName,
                    };
                profile = Encryptor.EncryptProfile(profile);
                _dmzProfileRepo.Insert(profile); 
            }
             _dmzProfileRepo.Save();
            SyncEmployments();

        }

        /// <summary>
        /// Syncs employments for all People in OS2 database to DMZ database.
        /// </summary>
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
                foreach (var masterEmployment in person.Employments)
                {
                    var dmzEmployment = new Employment 
                    {
                        Id = masterEmployment.Id,
                        ProfileId = masterEmployment.PersonId,
                        EmploymentPosition = masterEmployment.Position + " - " + masterEmployment.OrgUnit.LongDescription,
                    };
                    dmzEmployment = Encryptor.EncryptEmployment(dmzEmployment);
                    employments.Add(dmzEmployment);
                }
                var pers = _dmzProfileRepo.AsQueryable().First(x => x.Id == person.Id);
                pers.Employments = employments;
            }
            _dmzProfileRepo.Save();
        }

        /// <summary>
        /// Clears DMZ database of all people.
        /// </summary>
        public void ClearDmz()
        {
            var i = 0;
            var personList = _dmzProfileRepo.AsQueryable().ToList();
            var max = personList.Count;

            foreach (var profile in personList)
            {
                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Clearing person " + i + " of " + max);
                }
                _dmzProfileRepo.Delete(profile);
            }
            _dmzProfileRepo.Save();
        }

    }

}
