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
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.DataAccess;
using Ninject;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;
using Core.ApplicationServices.Interfaces;
using VacationBalance = Core.DomainModel.VacationBalance;

namespace DBUpdater
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();

            IAddressHistoryService historyService = new AddressHistoryService(ninjectKernel.Get<IGenericRepository<Employment>>(), ninjectKernel.Get<IGenericRepository<AddressHistory>>(), ninjectKernel.Get<IGenericRepository<PersonalAddress>>());
            
            var service = new UpdateService(ninjectKernel.Get<IGenericRepository<Employment>>(),
                ninjectKernel.Get<IGenericRepository<OrgUnit>>(),
                ninjectKernel.Get<IGenericRepository<Person>>(),
                ninjectKernel.Get<IGenericRepository<CachedAddress>>(),
                ninjectKernel.Get<IGenericRepository<PersonalAddress>>(),
                ninjectKernel.Get<IAddressLaunderer>(),
                ninjectKernel.Get<IAddressCoordinates>(), new DataProvider(),
                ninjectKernel.Get<IMailSender>(),
                historyService,
                ninjectKernel.Get<IGenericRepository<DriveReport>>(),
                ninjectKernel.Get<IReportService<DriveReport>>(),
                ninjectKernel.Get<ISubstituteService>(),
                ninjectKernel.Get<IGenericRepository<Substitute>>(),
                ninjectKernel.Get<IGenericRepository<VacationBalance>>());

            //service.MigrateOrganisations();
            //service.MigrateEmployees();
            //historyService.UpdateAddressHistories();
            //historyService.CreateNonExistingHistories();
            //service.UpdateLeadersOnExpiredOrActivatedSubstitutes();
            //service.AddLeadersToReportsThatHaveNone();
            //service.UpdateVacationBalance();



            string iv_begda = "2016-04-25";
            string iv_begda_old = "";
            string iv_begti = "1000";
            string iv_begti_old = "";
            string iv_endda = "2016-04-26";
            string iv_endda_old = "";
            string iv_endti = "1200";
            string iv_endti_old = "";
            string iv_extra_data = "";
            string iv_opera = "DEL";
            string iv_pernr = "00068298";
            string iv_subty = "FE";
            string iv_subty_old = "";

            //indberet ferie
            //ferieInd.Set_Absence_Plus(iv_begda, iv_begda_old, iv_begti, iv_begti_old, iv_endda, iv_endda_old, iv_endti, iv_endti_old, iv_extra_data, iv_opera, iv_pernr, iv_subty, iv_subty_old);
            
            //var empInfo = new Infrastructure.KMDVacationService.EmployeeLookup();

            //empInfo.EmployeeInfo("2015-09-15", "2611640091");
            //empInfo.EmployeeInfo(new DateTime(2015, 9, 15), "2611640091");

            var test = DateTime.UtcNow;

            var lolo = test.ToTimestamp();

            var enen = lolo.ToDateTime();

        }





    }
}
