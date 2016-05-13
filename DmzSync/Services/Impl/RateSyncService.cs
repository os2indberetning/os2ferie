using System;
using System.Linq;
using Core.DomainServices;
using DmzSync.Services.Interface;
using Rate = Core.DmzModel.Rate;

namespace DmzSync.Services.Impl
{
    public class RateSyncService : ISyncService
    {
        private IGenericDmzRepository<Core.DmzModel.Rate> _dmzRateRepo;
        private IGenericRepository<Core.DomainModel.Rate> _masterRateRepo;

        public RateSyncService(IGenericDmzRepository<Rate> dmzRateRepo, IGenericRepository<Core.DomainModel.Rate> masterRateRepo)
        {
            _dmzRateRepo = dmzRateRepo;
            _masterRateRepo = masterRateRepo;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void SyncFromDmz()
        {
            // We are not interested in migrating rates from DMZ to os2.
            throw new NotImplementedException();
        }


        /// <summary>
        /// Syncs all rates from OS2 database to DMZ database.
        /// </summary>
        public void SyncToDmz()
        {
            var i = 0;
            var currentYear = DateTime.Now.Year;
            var rateList = _masterRateRepo.AsQueryable().Where(x => x.Active && x.Year == currentYear).ToList();
            var max = rateList.Count;

            foreach (var masterRate in rateList)
            {
                i++;
                if (i%10 == 0)
                {
                    Console.WriteLine("Syncing rate " + i + " of " + max);
                }

                var rate = new Core.DmzModel.Rate()
                {
                    Id = masterRate.Id,
                    Description = masterRate.Type.Description,
                    Year = masterRate.Year.ToString()
                };

                var dmzRate = _dmzRateRepo.AsQueryable().FirstOrDefault(x => x.Id == rate.Id);

                if (dmzRate == null)
                {
                    _dmzRateRepo.Insert(rate);
                }
                else
                {
                    dmzRate.Description = rate.Description;
                    dmzRate.Year = rate.Year;
                }

            }
             _dmzRateRepo.Save();
        }


        /// <summary>
        /// Clears DMZ database of all rates.
        /// </summary>
        public void ClearDmz()
        {
            throw new NotImplementedException("This service is no longer used");
        }
    }
}
