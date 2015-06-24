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
            var tokens = _dmzTokenRepo.AsQueryable().ToList();
            var max = tokens.Count;

            for (var i = 0; i < max; i++)
            { 
                Console.WriteLine("Syncing token " + i + " of " + max + " from DMZ.");
                var token = tokens[i];
                token = Encryptor.DecryptToken(token);
                if (_masterTokenRepo.AsQueryable().Any(x => x.Guid.Equals(new Guid(token.GuId))))
                {
                    _masterTokenRepo.AsQueryable().First(x => x.Guid.Equals(new Guid(token.GuId))).Status = (MobileTokenStatus)token.Status;
                    _masterTokenRepo.Save();
                }
            }
        }


        // Dont run this method before syncing people.
        /// <summary>
        /// Syncs all MobileTokens from OS2 database to DMZ database.
        /// Do not run this before having synced people.
        /// </summary>
        public void SyncToDmz()
        {
            var tokens = _masterTokenRepo.AsQueryable().ToList();
            var max = tokens.Count;

            for (var i = 0; i < max; i++)
            { //Encrypt token before insert
                Console.WriteLine("Syncing token " + i + " of " + max + " to DMZ.");
                var masterToken = tokens[i];
                var dmzToken = new Token
                {
                    Id = masterToken.Id,
                    Status = (int)masterToken.Status,
                    GuId = masterToken.Guid.ToString(),
                    TokenString = masterToken.Token,
                    ProfileId = masterToken.PersonId,
                };
                dmzToken = Encryptor.EncryptToken(dmzToken);
                _dmzTokenRepo.Insert(dmzToken);
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
