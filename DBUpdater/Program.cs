using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService.Interface;
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
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var service = new UpdateService(ninjectKernel.Get<IGenericRepository<Employment>>(),
                ninjectKernel.Get<IGenericRepository<OrgUnit>>(),
                ninjectKernel.Get<IGenericRepository<Person>>(),
                ninjectKernel.Get<IGenericRepository<CachedAddress>>(),
                ninjectKernel.Get<IGenericRepository<PersonalAddress>>(),
                ninjectKernel.Get<IAddressLaunderer>(),
                ninjectKernel.Get<IAddressCoordinates>(), new DataProvider(),
                ninjectKernel.Get<IMailSender>());

            service.MigrateOrganisations();
            service.MigrateEmployees();
        }



        

    }
}
