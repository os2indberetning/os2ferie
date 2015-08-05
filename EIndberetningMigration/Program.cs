using System.Linq;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
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

            var reportService = new MigrateReportsService(ninjectKernel.Get<IGenericRepository<Employment>>(), new GenericRepository<DriveReport>(new TempContext()));
            reportService.MigrateReports(args.ToList());
        }
    }
}
