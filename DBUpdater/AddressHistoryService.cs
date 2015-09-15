using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;

namespace DBUpdater
{
    public class AddressHistoryService : IAddressHistoryService
    {
        private readonly IGenericRepository<Employment> _emplRepo;
        private readonly IGenericRepository<AddressHistory> _addressHistoryRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private List<WorkAddress> _workAddresses;
        private List<PersonalAddress> _homeAddresses;
        private HashSet<int> _changedHistories; 

        public AddressHistoryService(IGenericRepository<Employment> emplRepo, IGenericRepository<AddressHistory> addressHistoryRepo, IGenericRepository<PersonalAddress> personalAddressRepo)
        {
            _emplRepo = emplRepo;
            _addressHistoryRepo = addressHistoryRepo;
            _personalAddressRepo = personalAddressRepo;
            _changedHistories = new HashSet<int>();
            _homeAddresses = new List<PersonalAddress>();
            _workAddresses = new List<WorkAddress>();
        }

        public void AddHomeAddress(PersonalAddress homeAddress)
        {
            _homeAddresses.Add(homeAddress);
        }

        public void AddWorkAddress(WorkAddress workAddress)
        {
            _workAddresses.Add(workAddress);
        }

        public void CreateNonExistingHistories()
        {
            Console.WriteLine("Creating non-existing address histories.");
            var i = 0;
            var empls = _emplRepo.AsQueryable().ToList();
            foreach (var employment in empls)
            {
                i++;
                if (i%10 == 0)
                {
                    Console.WriteLine("Checking employment " + i + " of " + empls.Count);
                }
                if (!_addressHistoryRepo.AsQueryable().Any(x => x.EmploymentId == employment.Id && x.EndTimestamp == 0))
                {
                    var homeAddress =
                        _personalAddressRepo.AsQueryable()
                            .FirstOrDefault(x => x.PersonId == employment.PersonId && x.Type == PersonalAddressType.Home);

                    if (homeAddress == null || employment.OrgUnit.Address == null)
                    {
                        continue;
                    }

                    _addressHistoryRepo.Insert(new AddressHistory
                    {
                        EmploymentId = employment.Id,
                        EndTimestamp = 0,
                        WorkAddressId = employment.OrgUnit.AddressId,
                        HomeAddressId = homeAddress.Id,
                        StartTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                    });
                }
            }
            _addressHistoryRepo.Save();
        }

        public void UpdateAddressHistories()
        {
            foreach (var homeAddress in _homeAddresses)
            {
                var activeHistory = _addressHistoryRepo.AsQueryable().FirstOrDefault(x => x.EndTimestamp == 0 && x.Employment.PersonId == homeAddress.PersonId);
                if (activeHistory != null)
                {
                    _changedHistories.Add(activeHistory.EmploymentId);
                }
            }

            foreach (var workAddress in _workAddresses)
            {
                var activeHistory = _addressHistoryRepo.AsQueryable().FirstOrDefault(x => x.EndTimestamp == 0 && x.Employment.OrgUnitId == workAddress.OrgUnitId);
                if (activeHistory != null)
                {
                    _changedHistories.Add(activeHistory.EmploymentId);
                }
            }

            foreach (var changedHistory in _changedHistories)
            {
                // Iterate all active histories where addresses have changed
                // Set the EndTimestamp to now on these histories.
                _addressHistoryRepo.AsQueryable().Single(x => x.EmploymentId == changedHistory && x.EndTimestamp == 0).EndTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            _addressHistoryRepo.Save();


        }


    }
}
