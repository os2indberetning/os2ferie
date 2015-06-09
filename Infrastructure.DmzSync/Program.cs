using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.DmzDataAccess;
using Infrastructure.DataAccess;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Impl;
using Ninject;
using DriveReport = Core.DmzModel.DriveReport;
using Rate = Core.DomainModel.Rate;


namespace Infrastructure.DmzSync
{
    class Program
    {
        static void Main(string[] args)
        {
            // hacks because of error with Entity Framework.
            // This forces the dmzconnection to use MySql.
            new DataContext();

            var personSync = new PersonSyncService(new GenericDmzRepository<Profile>(new DmzContext()),
                new GenericRepository<Person>(new DataContext()),
                NinjectWebKernel.CreateKernel().Get<IPersonService>());

            var tokenSync = new TokenSyncService(new GenericDmzRepository<Token>(new DmzContext()),
                new GenericRepository<MobileToken>(new DataContext()));

            var driveSync = new DriveReportSyncService(new GenericDmzRepository<DriveReport>(new DmzContext()),
               new GenericRepository<Core.DomainModel.DriveReport>(new DataContext()),new GenericRepository<Rate>(new DataContext()),new GenericRepository<LicensePlate>(new DataContext()), NinjectWebKernel.CreateKernel().Get<IDriveReportService>(), NinjectWebKernel.CreateKernel().Get<IRoute<RouteInformation>>(), NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>());

            var rateSync = new RateSyncService(new GenericDmzRepository<Core.DmzModel.Rate>(new DmzContext()),
                new GenericRepository<Rate>(new DataContext()));

            Console.WriteLine("TokenSyncFromDmz");
            tokenSync.SyncFromDmz();

            Console.WriteLine("DriveReportsSyncFromDmz");
            driveSync.SyncFromDmz();

            Console.WriteLine("TokenClearDmz");
            tokenSync.ClearDmz();

            Console.WriteLine("DriveReportClearDmz");
            driveSync.ClearDmz();
            
            Console.WriteLine("PersonClearDmz");
            personSync.ClearDmz();
            
            Console.WriteLine("RateClearDmz");
            rateSync.ClearDmz();

            Console.WriteLine("PersonSyncToDmz");
            personSync.SyncToDmz();

            Console.WriteLine("TokenSyncToDmz");
            tokenSync.SyncToDmz();

            Console.WriteLine("RateSyncToDmz");
            rateSync.SyncToDmz();

            Console.WriteLine("Done");



        }
    }
}
