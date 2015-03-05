using System.Reflection;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace OS2Indberetning
{
    using System;
    using System.Web;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
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
        }        
    }
}
