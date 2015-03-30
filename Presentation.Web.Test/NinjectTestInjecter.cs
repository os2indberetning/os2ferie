using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Core.ApplicationServices;
using Ninject;
using Ninject.Web.Common;
using OS2Indberetning;

namespace Presentation.Web.Test
{
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

                NinjectWebKernel.RegisterServices(kernel);

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
            foreach (var injectedServicePair in _injectedServices)
            {
                kernel.Rebind(injectedServicePair.Key).To(injectedServicePair.Value);
            }
        }
    }
}
