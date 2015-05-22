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

        public void SyncFromDmz()
        {
            var i = 0;
            var tokens = _dmzTokenRepo.AsQueryable().ToList();
            var max = tokens.Count;

            var temp = _dmzTokenRepo.AsQueryable();
            foreach (var token in tokens)
            {
                i++;
                Console.WriteLine("Syncing token " + i + " of " + max + " from DMZ.");
                _masterTokenRepo.Patch(new MobileToken
                {
                    Id = token.Id,
                    Status = (MobileTokenStatus)token.Status,
                });
            }
            _masterTokenRepo.Save();
        }


        // Dont run this method before syncing people.
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

        public void ClearDmz()
        {
            foreach (var token in _dmzTokenRepo.AsQueryable())
            {
                  _dmzTokenRepo.Delete(token);
            }
            _dmzTokenRepo.Save();
        }

    }

}
