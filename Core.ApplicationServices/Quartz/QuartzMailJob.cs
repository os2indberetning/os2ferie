using Core.ApplicationServices.MailerService.Interface;
using Ninject;
using OS2Indberetning;
using Quartz;

namespace Core.ApplicationServices.Quartz
{
    public class QuartzMailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var mailService = NinjectWebKernel.CreateKernel().Get<IMailService>();
            mailService.SendMails();
        }
    }
}
