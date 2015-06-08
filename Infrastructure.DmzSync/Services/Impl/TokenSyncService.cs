using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Interface;

namespace Infrastructure.DmzSync.Services.Impl
{
    public class TokenSyncService : ISyncService
    {
        private IGenericRepository<Token> _dmzTokenRepo;
        private IGenericRepository<MobileToken> _masterTokenRepo;

        public TokenSyncService(IGenericRepository<Token> dmzTokenRepo, IGenericRepository<MobileToken> masterTokenRepo)
        {
            _dmzTokenRepo = dmzTokenRepo;
            _masterTokenRepo = masterTokenRepo;
        }

        /// <summary>
        /// Syncs all MobileTokens from DMZ database to OS2 database.
        /// </summary>
        public void SyncFromDmz()
        {
            var i = 0;
            var tokens = _dmzTokenRepo.AsQueryable().ToList();
            var max = tokens.Count;

            foreach (var token in tokens)
            {
                i++;
                Console.WriteLine("Syncing token " + i + " of " + max + " from DMZ.");
                _masterTokenRepo.AsQueryable().First(x => x.Guid.Equals(new Guid(token.GuId))).Status = (MobileTokenStatus) token.Status;
                _masterTokenRepo.Save();
            }
        }


        // Dont run this method before syncing people.
        /// <summary>
        /// Syncs all MobileTokens from OS2 database to DMZ database.
        /// Do not run this before having synced people.
        /// </summary>
        public void SyncToDmz()
        {
            var i = 0;
            var tokens = _masterTokenRepo.AsQueryable().ToList();
            var max = tokens.Count;

            foreach (var token in tokens)
            {
                i++;
                Console.WriteLine("Syncing token " + i + " of " + max + " to DMZ.");
                _dmzTokenRepo.Insert(new Token
                {
                    Id = token.Id,
                    Status = (int)token.Status,
                    GuId = token.Guid.ToString(),
                    TokenString = token.Token,
                    ProfileId = token.PersonId,
                });
            }
            _dmzTokenRepo.Save();
        }

        /// <summary>
        /// Clears all MobileTokens in DMZ database.
        /// </summary>
        public void ClearDmz()
        {
            var list = _dmzTokenRepo.AsQueryable().ToList();
            foreach (var token in list)
            {
                  _dmzTokenRepo.Delete(token);
            }
            _dmzTokenRepo.Save();
        }

    }

}
