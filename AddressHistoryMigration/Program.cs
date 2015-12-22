using Core.ApplicationServices;

namespace AddressHistoryMigration
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var updateService = new Service();
            updateService.TransferFromTempToActual();
        }





    }
}
