using System.Linq;
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
            var personalAddressesServices = new MigratePersonalAddressesService(ninjectKernel.Get<IGenericRepository<PersonalAddress>>(), ninjectKernel.Get<IGenericRepository<Person>>(), ninjectKernel.Get<IAddressCoordinates>());
            //personalAddressesServices.MigratePersonalAddresses(args.ToList());

            var reportService = new MigrateReportsService(ninjectKernel.Get<IGenericRepository<Employment>>(), ninjectKernel.Get<IGenericRepository<DriveReport>>());
            reportService.MigrateReports(args.ToList());
        }
    }
}
