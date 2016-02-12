using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public interface IAppLoginService
    {
        AppLogin PrepareAppLogin(AppLogin appLogin);
        AppLogin SyncToDmz(AppLogin appLogin);
        void RemoveFromDmz(int personId);
    }
}
