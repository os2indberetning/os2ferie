using AddressHistoryMigration;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace DBUpdater
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var updateService = new UpdateService(new DataProvider(), ninjectKernel.Get<IGenericRepository<Employment>>(), ninjectKernel.Get<IGenericRepository<AddressHistory>>(), ninjectKernel.Get<IGenericRepository<PersonalAddress>>(), ninjectKernel.Get<IGenericRepository<WorkAddress>>(), ninjectKernel.Get<IAddressCoordinates>());
            updateService.MigrateHistories();
        }





    }
}
