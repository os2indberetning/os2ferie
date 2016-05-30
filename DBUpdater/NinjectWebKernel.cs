using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Infrastructure.DmzDataAccess;
using Infrastructure.KMDVacationService;
using Ninject;
using Ninject.Web.Common;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DBUpdater
{
    public static class NinjectWebKernel
    {
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <param name="getInjections"></param>
        /// <returns>The created kernel.</returns>
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                // Install our Ninject-based IDependencyResolver into the Web API config
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<DataContext>().ToSelf().InRequestScope();
            kernel.Bind(typeof (IGenericRepository<>)).To(typeof (GenericRepository<>));
            kernel.Bind(typeof(IGenericDmzRepository<>)).To(typeof(GenericDmzRepository<>));
            kernel.Bind<IPersonService>().To<PersonService>();
            kernel.Bind<IMobileTokenService>().To<MobileTokenService>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<IMailService>().To<MailService>();
            kernel.Bind<ISubstituteService>().To<SubstituteService>();
            kernel.Bind<IAddressCoordinates>().To<AddressCoordinates>();
            kernel.Bind<IRoute<RouteInformation>>().To<BestRoute>();
            kernel.Bind<IReimbursementCalculator>().To<ReimbursementCalculator>();
            kernel.Bind<ILicensePlateService>().To<LicensePlateService>();
            kernel.Bind<IPersonalRouteService>().To<PersonalRouteService>();
            kernel.Bind<IAddressLaunderer>().To<AddressLaundering>();
            kernel.Bind<IOrgUnitService>().To<OrgUnitService>();
            kernel.Bind<ILogger>().To<Core.ApplicationServices.Logger.Logger>();
            kernel.Bind<IAppLoginService>().To<AppLoginService>();
            kernel.Bind<IReportService<Report>>().To<ReportService<Report>>();
            kernel.Bind<IReportService<VacationReport>>().To<VacationReportService>();
            kernel.Bind<IReportService<DriveReport>>().To<DriveReportService>();
            kernel.Bind<IVacationReportService>().To<VacationReportService>();
            kernel.Bind<IDriveReportService>().To<DriveReportService>();
            kernel.Bind<IKMDAbsenceService>().To<KMDAbsenceService>();
            kernel.Bind<IKMDAbsenceReportBuilder>().To<KMDAbsenceReportBuilder>();
            kernel.Bind<IAddressHistoryService>().To<AddressHistoryService>();
            kernel.Bind<IDbUpdaterDataProvider>().To<DataProvider>();
        }
    }
}
