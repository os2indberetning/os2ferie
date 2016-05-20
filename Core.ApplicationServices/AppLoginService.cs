using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices.Encryption;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.DomainServices;

namespace Core.ApplicationServices
{
    public class AppLoginService : IAppLoginService
    {
        private readonly IGenericDmzRepository<UserAuth> _dmzUserRepository;

        public AppLoginService(IGenericDmzRepository<UserAuth> dmzUserRepository)
        {
            _dmzUserRepository = dmzUserRepository;
        }


        /// <summary>
        /// Generates salt and hashes password
        /// </summary>
        /// <param name="appLogin"></param>
        /// <returns>AppLogin with salt and hashed password</returns>
        public AppLogin PrepareAppLogin(AppLogin appLogin)
        {
            var result = appLogin;
            var salt = GenerateRandomSalt();
            result.Password = GetHash(salt, appLogin.Password);
            result.Salt = salt;
            result.GuId = Guid.NewGuid().ToString();
            return result;
        }

        /// <summary>
        /// Generates a random password salt using RNGCryptoServiceProvider
        /// </summary>
        /// <returns>string salt</returns>
        private string GenerateRandomSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new Byte[8];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Hashes salt + password using SHA256
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="password"></param>
        /// <returns>string hashed password</returns>
        private static String GetHash(string salt, string password)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(salt + password));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public AppLogin SyncToDmz(AppLogin appLogin)
        {
            var encryptedLogin = Encryptor.EncryptAppLogin(appLogin);
            var dmzLogin = new UserAuth
            {
                UserName = encryptedLogin.UserName,
                GuId = encryptedLogin.GuId,
                Password = encryptedLogin.Password,
                ProfileId = encryptedLogin.PersonId,
                Salt = encryptedLogin.Salt
            };
            _dmzUserRepository.Insert(dmzLogin);
            _dmzUserRepository.Save();

            return encryptedLogin;
        }

        public void RemoveFromDmz(int personId)
        {
            var users = _dmzUserRepository.AsQueryable().Where(x => x.ProfileId == personId).ToList();
            _dmzUserRepository.DeleteRange(users);
            _dmzUserRepository.Save();
        }
    }
}
