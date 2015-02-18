using System;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using IMobileTokenService = Core.ApplicationServices.Interfaces.IMobileTokenService;

namespace Core.ApplicationServices
{
    public class MobileTokenService : IMobileTokenService
    {
        private readonly IGenericRepository<MobileToken> _repo;

        public MobileTokenService(IGenericRepository<MobileToken> repo)
        {
            _repo = repo;
        }

        public MobileToken Create(MobileToken token)
        {
            var randomToken = GenerateToken();

            //Check if tokens exists
            var exists = true;

            while (exists)
            {
                if (_repo.AsQueryable().Any(x => x.Token == randomToken))
                {
                    randomToken = GenerateToken();
                }
                else
                {
                    exists = false;
                }
            }

            if (token.Status == MobileTokenStatus.Created)
            {
                token.StatusToPresent = "Oprettet";
            }

            if (token.Status == MobileTokenStatus.Activated)
            {
                token.StatusToPresent = "Aktiveret";
            }

            token.Token = randomToken;
            token.Guid = Guid.NewGuid();
            token.Status = MobileTokenStatus.Created;

            var createdToken = _repo.Insert(token);

            _repo.Save();

            return createdToken;
        }

        public bool Delete(MobileToken token)
        {
            var id = token.Id;

            _repo.Delete(token);

            _repo.Save();

            if (_repo.AsQueryable().Any(x => x.Id == id))
            {
                return false;
            }

            return true;
        }

        public IQueryable<MobileToken> GetByPersonId(int id)
        {
            var result = _repo.AsQueryable().Where(x => x.PersonId == id && x.Status != MobileTokenStatus.Deleted);

            foreach (var token in result)
            {
                if (token.Status == MobileTokenStatus.Created)
                {
                    token.StatusToPresent = "Oprettet";
                }

                if (token.Status == MobileTokenStatus.Activated)
                {
                    token.StatusToPresent = "Aktiveret";
                }
            }

            return result;
        }

        private string GenerateToken()
        {
            var token = "";

            var generator = new Random();

            for (int i = 0; i < 10; i++)
            {
                token += generator.Next(0, 10);
            }

            return token;
        }
    }
}