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
                    AktivFra = (Int32) (DateTime.UtcNow.Subtract(ah.AktivFra)).TotalSeconds,
                    AktivTil = (Int32) (DateTime.UtcNow.Subtract(ah.AktivTil)).TotalSeconds,
                    ArbejdsAdresse = ah.ArbejdsAdresse,
                    ArbejdsBy = ah.ArbejdsBy,
                    ArbejdsPostNr = ah.ArbejdsPostNr,
                    HjemmeAdresse = ah.HjemmeAdresse,
                    HjemmeBy = ah.HjemmeBy,
                    HjemmeLand = ah.HjemmeLand,
                    HjemmePostNr = ah.HjemmePostNr,
                    MaNr = ah.MaNr,
                    Navn = ah.Navn,
                    IsDirty = false
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
                    tempHistory.IsDirty = true;
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
                    tempHistory.IsDirty = true;
                }
                repo.Insert(tempHistory);
            }
            repo.Save();
            return null;
        } 


    }
}
