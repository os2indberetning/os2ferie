using System;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using DmzSync.Services.Impl;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Ninject;
using DriveReport = Core.DmzModel.DriveReport;
using Rate = Core.DomainModel.Rate;

namespace DmzSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = NinjectWebKernel.CreateKernel().Get<ILogger>();

            // hacks because of error with Entity Framework.
            // This forces the dmzconnection to use MySql.
            new DataContext();

            var personSync = new PersonSyncService(new GenericDmzRepository<Profile>(new DmzContext()),
                new GenericRepository<Person>(new DataContext()),
                NinjectWebKernel.CreateKernel().Get<IPersonService>());

            var driveSync = new DriveReportSyncService(new GenericDmzRepository<DriveReport>(new DmzContext()),
               new GenericRepository<Core.DomainModel.DriveReport>(new DataContext()), new GenericRepository<Rate>(new DataContext()), new GenericRepository<LicensePlate>(new DataContext()), NinjectWebKernel.CreateKernel().Get<IDriveReportService>(), NinjectWebKernel.CreateKernel().Get<IRoute<RouteInformation>>(), NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>(), NinjectWebKernel.CreateKernel().Get<IGenericRepository<Core.DomainModel.Employment>>(), NinjectWebKernel.CreateKernel().Get<ILogger>());

            var rateSync = new RateSyncService(new GenericDmzRepository<Core.DmzModel.Rate>(new DmzContext()),
                new GenericRepository<Rate>(new DataContext()));

            try
            {
                Console.WriteLine("DriveReportsSyncFromDmz");
                driveSync.SyncFromDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Fejl under synkronisering af indberetninger fra DMZ. Mobilindberetninger er ikke synkroniserede.", "dmz", ex, 1);
                throw;
            }

            try
            {
                Console.WriteLine("PersonSyncToDmz");
                personSync.SyncToDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Fejl under synkronisering af medarbejdere til DMZ. Mobil-app er ikke opdateret med nyeste medarbejderdata.", "dmz", ex, 1);
                throw;
            }

            try
            {
                Console.WriteLine("RateSyncToDmz");
                rateSync.SyncToDmz();
            }
            catch (Exception ex)
            {
                logger.Log("Fejl under synkronisering af takster til DMZ. Mobil-app er ikke opdateret med nyeste rater.", "dmz", ex, 1);
                throw;
            }

            Console.WriteLine("Done");
        }
    }
}
