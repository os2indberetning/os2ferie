using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressHistoryMigration.Models;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices;
using Ninject;
using AddressHistory = Core.DomainModel.AddressHistory;

namespace AddressHistoryMigration
{
    public class Service
    {
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


        public void TryReClean()
        {
            var tempRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<TempAddressHistory>>();
            var coords = new AddressCoordinates();
            var i = 0;
            var rows = tempRepo.AsQueryable().Where(x => x.HomeIsDirty || x.WorkIsDirty).ToList();
            foreach (var history in rows)
            {
                i++;
                Console.WriteLine(i + " of " + rows.Count);
                history.WorkIsDirty = false;
                history.HomeIsDirty = false;

                try
                {
                    coords.GetAddressCoordinates(new Address
                    {
                        StreetName = SplitAddressOnNumber(history.HjemmeAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(history.HjemmeAdresse)[1],
                        ZipCode = history.HjemmePostNr,
                        Town = history.HjemmeBy
                    });

                }
                catch (Exception e)
                {
                    history.HomeIsDirty = true;
                }

                try
                {
                    coords.GetAddressCoordinates(new Address
                    {
                        StreetName = SplitAddressOnNumber(history.ArbejdsAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(history.ArbejdsAdresse)[1],
                        ZipCode = history.ArbejdsPostNr,
                        Town = history.ArbejdsBy
                    });

                }
                catch (Exception e)
                {
                    history.WorkIsDirty = true;
                }
            }
            tempRepo.Save();
        }

        public void TransferFromTempToActual()
        {
            var tempRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<TempAddressHistory>>();
            var actualRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<AddressHistory>>();
            var workAddressRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<WorkAddress>>();
            var personalAddressRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<PersonalAddress>>();
            var coords = new AddressCoordinates();
            var emplRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Employment>>();

            var i = 0;

            var rows = tempRepo.AsQueryable().Where(x => !x.WorkIsDirty && !x.HomeIsDirty).ToList();

            foreach (var tempHistory in rows)
            {
                i++;
                Console.WriteLine(i + " of " + rows.Count);
                var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var empls = emplRepo.AsQueryable().Where(x => x.EmploymentId == tempHistory.MaNr).ToList();
                if (empls.Count == 0)
                {
                    continue; ;
                }
                var empl = new Employment();
                if (empls.Count == 1)
                {
                    // If only one empl with manr exists just use that
                    empl = empls.First();
                }
                else
                {
                    // If more than one exists then select the active one.
                    // If an active one does not exist then select the one that ended most recently
                    empl = empls.FirstOrDefault(x => x.EndDateTimestamp == 0) ??
                           empls.OrderByDescending(x => x.EndDateTimestamp).First();
                }

                var workTemp = coords.GetAddressCoordinates(new WorkAddress()
                {
                    StreetName = SplitAddressOnNumber(tempHistory.ArbejdsAdresse)[0],
                    StreetNumber = SplitAddressOnNumber(tempHistory.ArbejdsAdresse)[1],
                    ZipCode = tempHistory.ArbejdsPostNr,
                    Town = tempHistory.ArbejdsBy,
                });

                var workAddress = new WorkAddress
                {
                    StreetName = workTemp.StreetName,
                    StreetNumber = workTemp.StreetNumber,
                    ZipCode = workTemp.ZipCode,
                    Town = workTemp.Town,
                    Latitude = workTemp.Latitude,
                    Longitude = workTemp.Longitude
                };

                var homeTemp = coords.GetAddressCoordinates(new PersonalAddress
                {
                    StreetName = SplitAddressOnNumber(tempHistory.HjemmeAdresse)[0],
                    StreetNumber = SplitAddressOnNumber(tempHistory.HjemmeAdresse)[1],
                    ZipCode = tempHistory.HjemmePostNr,
                    Town = tempHistory.HjemmeBy,
                });

                var homeAddress = new PersonalAddress()
                {
                    StreetName = homeTemp.StreetName,
                    StreetNumber = homeTemp.StreetNumber,
                    ZipCode = homeTemp.ZipCode,
                    Town = homeTemp.Town,
                    Latitude = homeTemp.Latitude,
                    Longitude = homeTemp.Longitude,
                    PersonId = empl.PersonId,
                    Type = PersonalAddressType.OldHome,
                };

                workAddressRepo.Insert(workAddress);
                personalAddressRepo.Insert(homeAddress);
                workAddressRepo.Save();
                personalAddressRepo.Save();

                var addressHistory = new Core.DomainModel.AddressHistory
                {
                    WorkAddressId = workAddress.Id,
                    HomeAddressId = homeAddress.Id,
                    StartTimestamp = tempHistory.AktivFra,
                    EndTimestamp = tempHistory.AktivTil,
                    EmploymentId = empl.Id,        
                    IsMigrated = true,
                };

                actualRepo.Insert(addressHistory);

            }
            actualRepo.Save();
        }

        public HashSet<string> GetUncleanableAddresses()
        {
            var provider = new DataProvider();
            var coords = new AddressCoordinates();
            var repo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<TempAddressHistory>>();
            Console.WriteLine("Loading histories");
            var histories = provider.GetAddressHistoriesAsQueryable();
            var i = 0;
            var max = histories.Count();
            foreach (var ah in histories)
            {
                i++;
                Console.WriteLine("Attempting clean of address " + i + " of " + max);

                var tempHistory = new TempAddressHistory
                {

                    AktivFra = (Int32)(ah.AktivFra.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    AktivTil = (Int32)(ah.AktivTil.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    ArbejdsAdresse = ah.ArbejdsAdresse,
                    ArbejdsBy = ah.ArbejdsBy,
                    ArbejdsPostNr = ah.ArbejdsPostNr,
                    HjemmeAdresse = ah.HjemmeAdresse,
                    HjemmeBy = ah.HjemmeBy,
                    HjemmeLand = ah.HjemmeLand,
                    HjemmePostNr = ah.HjemmePostNr,
                    MaNr = ah.MaNr,
                    Navn = ah.Navn,
                    HomeIsDirty = false,
                    WorkIsDirty = false,
                };

                var home = new Address();

                try
                {
                    home = new Address
                    {
                        StreetName = SplitAddressOnNumber(ah.HjemmeAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(ah.HjemmeAdresse)[1],
                        ZipCode = ah.HjemmePostNr,
                        Town = ah.HjemmeBy
                    };
                    coords.GetAddressCoordinates(home);

                }
                catch (Exception e)
                {
                    tempHistory.HomeIsDirty = true;
                }

                var work = new Address();

                try
                {

                    work = new Address
                    {
                        StreetName = SplitAddressOnNumber(ah.ArbejdsAdresse)[0],
                        StreetNumber = SplitAddressOnNumber(ah.ArbejdsAdresse)[1],
                        ZipCode = ah.ArbejdsPostNr,
                        Town = ah.ArbejdsBy
                    };
                    coords.GetAddressCoordinates(work);

                }
                catch (Exception e)
                {
                    tempHistory.WorkIsDirty = true;
                }
                repo.Insert(tempHistory);
            }
            repo.Save();
            return null;
        } 


    }
}
