using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace EIndberetningMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var service = new MigrateService(ninjectKernel.Get<IGenericRepository<PersonalAddress>>(), ninjectKernel.Get<IGenericRepository<Person>>(),new DataProvider(), ninjectKernel.Get<IAddressCoordinates>());
            
            
            
            service.MigratePersonalAddresses(args.ToList());
            var a = service.errorCount;
        }
    }
}
