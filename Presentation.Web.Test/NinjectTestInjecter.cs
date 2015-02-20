using System.Collections.Generic;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace OS2Indberetning.App_Start
{
    using System;
    using System.Web;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectTestInjector
    {
        private static List<KeyValuePair<Type, Type>> _injectedServices;  

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        public static IKernel CreateKernel(List<KeyValuePair<Type, Type>> injectedServices)
        {
            _injectedServices = injectedServices;
            var kernel = new StandardKernel();
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
            kernel.Bind<DataContext>().ToSelf();
            foreach (var injectedServicePair in _injectedServices)
            {
                kernel.Bind(injectedServicePair.Key).To(injectedServicePair.Value);
            }
        }
    }
}
