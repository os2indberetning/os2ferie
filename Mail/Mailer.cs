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
