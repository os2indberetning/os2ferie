
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Ninject;
using Ninject.Web.Common;

namespace OS2Indberetning
{
    using System;

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
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<DataContext>().ToSelf().InRequestScope();
            kernel.Bind(typeof (IGenericRepository<>)).To(typeof (GenericRepository<>));
            kernel.Bind<IPersonService>().To<PersonService>();
            kernel.Bind<IMobileTokenService>().To<MobileTokenService>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind(typeof(IMailService)).To(typeof(MailService));
        }        
    }
}
