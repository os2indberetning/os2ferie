using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Ninject.Activation;

namespace AddressHistoryMigration
{
    public class UpdateService
    {
        private readonly DataProvider _provider;
        private readonly IGenericRepository<Employment> _employmentRepo;
        private readonly IGenericRepository<AddressHistory> _historyRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private readonly IGenericRepository<WorkAddress> _workAddressRepo;
        private readonly IAddressCoordinates _coords;

        public UpdateService(DataProvider provider, IGenericRepository<Employment> employmentRepo, IGenericRepository<AddressHistory> historyRepo, IGenericRepository<PersonalAddress> personalAddressRepo, IGenericRepository<WorkAddress> workAddressRepo, IAddressCoordinates coords)
        {
            _provider = provider;
            _employmentRepo = employmentRepo;
            _historyRepo = historyRepo;
            _personalAddressRepo = personalAddressRepo;
            _workAddressRepo = workAddressRepo;
            _coords = coords;
        }

        public void MigrateHistories()
        {
            var historiesToMigrate = _provider.GetAddressHistoriesAsQueryable();
            var failedWorkAddresses = new List<string>();
            var failedHomeAddresses = new List<string>();

            var i = 0;
            var max = historiesToMigrate.Count();
            foreach (var ah in historiesToMigrate)
            {
                if (i%10 == 0)
                {
                    Console.WriteLine("Migrating AddressHistory " + i + " of " + max + ".");
                }
                i++;

                var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var employments = _employmentRepo.AsQueryable().Where(x => x.EmploymentId == ah.MaNr);
                var employment = new Employment();
                if (employments.Count() == 1)
                {
                    employment = employments.First();
                }
                else if (employments.Count() > 1)
                {
                    employment = employments.Single(x => x.EndDateTimestamp == 0 || x.EndDateTimestamp > currentDateTimestamp);
                }


                var homeAddressLookup = new Address();
                var homeAddress = new PersonalAddress();
                try
                {
                    homeAddressLookup = _coords.GetAddressCoordinates(new Address()
                    {
                        StreetName = SplitAddressOnNumber(ah.HjemmeAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(ah.HjemmeAdresse)[1],
                        ZipCode = ah.HjemmePostNr,
                        Town = ah.HjemmeBy
                    });

                    homeAddress = new PersonalAddress()
                    {
                        Type = PersonalAddressType.OldHome,
                        PersonId = employment.PersonId,
                        StreetNumber = homeAddressLookup.StreetNumber,
                        StreetName = homeAddressLookup.StreetName,
                        Town = homeAddressLookup.Town,
                        ZipCode = homeAddressLookup.ZipCode,
                        Latitude = homeAddressLookup.Latitude,
                        Longitude = homeAddressLookup.Longitude
                    };

                    homeAddress = _personalAddressRepo.Insert(homeAddress);
                    _personalAddressRepo.Save();
                }
                catch (Exception)
                {
                    failedHomeAddresses.Add(ah.HjemmeAdresse);
                }

                var workAddressLookup = new Address();
                var workAddress = new WorkAddress();

                try
                {
                    workAddressLookup = _coords.GetAddressCoordinates(new Address()
                    {
                        StreetName = SplitAddressOnNumber(ah.ArbejdsAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(ah.ArbejdsAdresse)[1],
                        ZipCode = ah.ArbejdsPostNr,
                        Town = ah.ArbejdsBy
                    });

                    workAddress = new WorkAddress()
                    {
                        StreetNumber = workAddressLookup.StreetNumber,
                        StreetName = workAddressLookup.StreetName,
                        Town = workAddressLookup.Town,
                        ZipCode = workAddressLookup.ZipCode,
                        Latitude = workAddressLookup.Latitude,
                        Longitude = workAddressLookup.Longitude
                    };


                    workAddress = _workAddressRepo.Insert(workAddress);

                    _workAddressRepo.Save();
                }
                catch (Exception)
                {
                    failedWorkAddresses.Add(ah.ArbejdsAdresse);
                }

                




                var history = new Core.DomainModel.AddressHistory()
                {
                    EmploymentId = employment.Id,
                    StartTimestamp = (Int32) (ah.AktivFra.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    EndTimestamp = (Int32) (ah.AktivTil.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    HomeAddressId = homeAddress.Id,
                    WorkAddressId = workAddress.Id
                };

                _historyRepo.Insert(history);
            }
            _historyRepo.Save();
        }

        /// <summary>
        /// Splits an address represented as "StreetName StreetNumber" into a list of "StreetName" , "StreetNumber"
        /// </summary>
        /// <param name="address">String to split</param>
        /// <returns>List of StreetName and StreetNumber</returns>
        public List<String> SplitAddressOnNumber(string address)
        {
            var result = new List<string>();
            var index = address.IndexOfAny("0123456789".ToCharArray());
            if (index == -1)
            {
                result.Add(address);
            }
            else
            {
                result.Add(address.Substring(0, index - 1));
                result.Add(address.Substring(index, address.Length - index));
            }
            return result;
        }

    }
}
