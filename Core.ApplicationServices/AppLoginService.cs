using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices.Encryption;
using Infrastructure.DmzDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public class AppLoginService : IAppLoginService
    {
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

            using (SHA256 hash = SHA256Managed.Create())
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
            var _dmzAuthRepo = new GenericDmzRepository<UserAuth>(new DmzContext());

            var encryptedLogin = Encryptor.EncryptAppLogin(appLogin);
            var dmzLogin = new UserAuth
            {
                UserName = encryptedLogin.UserName,
                GuId = encryptedLogin.GuId,
                Password = encryptedLogin.Password,
                ProfileId = encryptedLogin.PersonId,
                Salt = encryptedLogin.Salt
            };
            _dmzAuthRepo.Insert(dmzLogin);
            _dmzAuthRepo.Save();

            return encryptedLogin;
        }

        public void RemoveFromDmz(int personId)
        {
            var _dmzAuthRepo = new GenericDmzRepository<UserAuth>(new DmzContext());

            var rows = _dmzAuthRepo.AsQueryable().Where(x => x.ProfileId == personId).ToList();
            _dmzAuthRepo.DeleteRange(rows);
            _dmzAuthRepo.Save();
        }
    }
}
