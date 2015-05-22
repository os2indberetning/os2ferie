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


namespace Infrastructure.DmzSync
{
    class Program
    {
        static void Main(string[] args)
        {
            new DataContext();

            var personSync = new PersonSyncService(new GenericDmzRepository<Profile>(new DmzContext()),
                new GenericRepository<Person>(new DataContext()),
                NinjectWebKernel.CreateKernel().Get<IPersonService>());

            var tokenSync = new TokenSyncService(new GenericDmzRepository<Token>(new DmzContext()),
                new GenericRepository<MobileToken>(new DataContext()));
            
            Console.WriteLine("TokenSyncFromDmz");
            tokenSync.SyncFromDmz();
            
            Console.WriteLine("TokenClearDmz");
            tokenSync.ClearDmz();
            
            Console.WriteLine("PersonClearDmz");
            personSync.ClearDmz();

            Console.WriteLine("PersonSyncToDmz");
            personSync.SyncToDmz();

            Console.WriteLine("TokenSyncToDmz");
            tokenSync.SyncToDmz();

            Console.WriteLine("Done");



        }
    }
}
