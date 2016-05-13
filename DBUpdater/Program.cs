using Ninject;

namespace DBUpdater
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var historyService = ninjectKernel.Get<IAddressHistoryService>();
            var service = ninjectKernel.Get<UpdateService>();

            service.MigrateOrganisations();
            service.MigrateEmployees();
            historyService.UpdateAddressHistories();
            historyService.CreateNonExistingHistories();
            service.UpdateLeadersOnExpiredOrActivatedSubstitutes();
            service.AddLeadersToReportsThatHaveNone();
            service.UpdateVacationBalance();
        }
    }
}
