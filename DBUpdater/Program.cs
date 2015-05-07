using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.DataAccess;
using Ninject;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DBUpdater
{
    static class Program
    {
        static void Main(string[] args)
        {
            var service = new UpdateService(NinjectWebKernel.CreateKernel().Get<IGenericRepository<Employment>>(),
                NinjectWebKernel.CreateKernel().Get<IGenericRepository<OrgUnit>>(),
                NinjectWebKernel.CreateKernel().Get<IGenericRepository<Person>>(),
                NinjectWebKernel.CreateKernel().Get<IGenericRepository<CachedAddress>>(),
                NinjectWebKernel.CreateKernel().Get<IGenericRepository<PersonalAddress>>(),
                NinjectWebKernel.CreateKernel().Get<IAddressLaunderer>(),
                NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>(), new SyddjursDataProvider(),
                NinjectWebKernel.CreateKernel().Get<IGenericRepository<WorkAddress>>());

            //service.MigrateOrganisations();
            service.MigrateEmployees();
        }



        

    }
}