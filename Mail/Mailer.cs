using System;
using System.Linq;
using System.Threading;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace Mail
{
    public class Mailer
    {
        public static void Main(string[] args)
        {
            var service = NinjectWebKernel.CreateKernel().Get<ConsoleMailerService>();
            service.RunMailService();
        }

        
    }
}
