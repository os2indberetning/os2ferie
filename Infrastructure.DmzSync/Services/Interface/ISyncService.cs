using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DmzSync.Services.Interface
{
    public interface ISyncService
    {
        void SyncFromDmz();
        void SyncToDmz();
        void ClearDmz();
    }
}
