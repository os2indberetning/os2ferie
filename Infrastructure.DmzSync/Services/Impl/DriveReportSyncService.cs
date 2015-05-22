using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.Interfaces;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Interface;
using Employment = Core.DmzModel.Employment;

namespace Infrastructure.DmzSync.Services.Impl
{
    public class DriveReportSyncService : ISyncService
    {
        private IGenericRepository<Profile> _dmzDriveReportRepo;
        private IGenericRepository<Person> _masterDriveReportRepo;

        public DriveReportSyncService(IGenericRepository<Profile> dmzDriveReportRepo, IGenericRepository<Person> masterDriveReportRepo)
        {
            _dmzDriveReportRepo = dmzDriveReportRepo;
            _masterDriveReportRepo = masterDriveReportRepo;
        }

        public void SyncFromDmz()
        {
            // We are not interested in migrating profiles from DMZ to os2.
            throw new NotImplementedException();
        }

        public void SyncToDmz()
        {
            throw new NotImplementedException();
        }

        public void ClearDmz()
        {
            throw new NotImplementedException();
        }

    }

}
