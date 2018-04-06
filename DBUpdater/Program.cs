using Ninject;
using Core.ApplicationServices.Logger;
using System;
using Core.DomainServices.Interfaces;

namespace DBUpdater
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();
            var historyService = ninjectKernel.Get<IAddressHistoryService>();
            var service = ninjectKernel.Get<UpdateService>();
            var logger = ninjectKernel.Get<ILogger>();
            var customSettings = ninjectKernel.Get<ICustomSettings>();

            logger.Log("DBUpdater er start", "dbupdater");
            try
            {
                service.MigrateOrganisations();
                service.MigrateEmployees();
                historyService.UpdateAddressHistories();
                historyService.CreateNonExistingHistories();
                service.UpdateLeadersOnExpiredOrActivatedSubstitutes();
                service.AddLeadersToReportsThatHaveNone();
                service.UpdateVacationBalance();
                logger.Log("DBUpdater kørte uden fejl", "dbupdater");
            }
            catch(Exception ex)
            {
                logger.Log("Fejl:", "dbupdater", ex);
            }


        }
    }
}
