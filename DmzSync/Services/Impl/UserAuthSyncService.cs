using System;
using System.Linq;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Encryption;
using DmzSync.Services.Interface;

namespace DmzSync.Services.Impl
{
    public class UserAuthSyncService : ISyncService
    {
        private readonly IGenericRepository<AppLogin> _appLoginRepo;
        private readonly IGenericRepository<UserAuth> _dmzAuthRepo;

        public UserAuthSyncService(IGenericRepository<AppLogin> appLoginRepo, IGenericRepository<UserAuth> dmzAuthRepo)
        {
            _appLoginRepo = appLoginRepo;
            _dmzAuthRepo = dmzAuthRepo;
        }

        /// <summary>
        /// Syncs all AppLogin from DMZ database to OS2 database.
        /// </summary>
        public void SyncFromDmz()
        {
            throw new NotImplementedException();
        }


        // Dont run this method before syncing people.
        /// <summary>
        /// Syncs all MobileTokens from OS2 database to DMZ database.
        /// Do not run this before having synced people.
        /// </summary>
        public void SyncToDmz()
        {
            var i = 0;
            var logins = _appLoginRepo.AsQueryable().ToList();
            var max = logins.Count;

            foreach (var login in logins)
            {

                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Syncing UserAuth " + i + " of " + max);
                }

                var encryptedLogin = Encryptor.EncryptAppLogin(login);

                var dmzLogin = new UserAuth
                {
                    UserName = encryptedLogin.UserName,
                    GuId = encryptedLogin.GuId,
                    Password = encryptedLogin.Password,
                    ProfileId = encryptedLogin.PersonId,
                    Salt = encryptedLogin.Salt
                };

                _dmzAuthRepo.Insert(dmzLogin);
                
            }

            _dmzAuthRepo.Save();
        }

        /// <summary>
        /// Clears all AppLogins in DMZ database.
        /// </summary>
        public void ClearDmz()
        {
            throw new NotImplementedException();
        }

    }

}
