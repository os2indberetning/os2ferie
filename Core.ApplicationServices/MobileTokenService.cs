using System;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
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

        /// <summary>
        /// Creates a MobileToken and inserts it into the database.
        /// </summary>
        /// <param name="token">MobileToken to be created.</param>
        /// <returns>The created MobileToken.</returns>
        public MobileToken Create(MobileToken token)
        {
            var randomToken = GenerateToken();

            //Check if tokens exists
            var exists = true;

            while (exists)
            {
                if (_repo.AsQueryable().Any(x => x.Token.Equals(randomToken)))
                {
                    randomToken = GenerateToken();
                }
                else
                {
                    exists = false;
                }
            }

            token.Token = randomToken;
            token.Guid = Guid.NewGuid();
            token.Status = MobileTokenStatus.Created;

            if (token.Status == MobileTokenStatus.Created)
            {
                token.StatusToPresent = "Oprettet";
            }

            if (token.Status == MobileTokenStatus.Activated)
            {
                token.StatusToPresent = "Aktiveret";
            }            

            var createdToken = _repo.Insert(token);

            _repo.Save();

            return createdToken;
        }

        /// <summary>
        /// Deletes the MobileToken
        /// </summary>
        /// <param name="token">MobileToken to be deleted.</param>
        /// <returns>True if successfull, otherwise false.</returns>
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

        /// <summary>
        /// Gets all MobileTokens with status other than Deleted belonging to the user identified by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MobileTokens</returns>
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

        /// <summary>
        /// Generates a mobile token.
        /// </summary>
        /// <returns>A string representing the generated MobileToken</returns>
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