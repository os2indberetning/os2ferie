using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailSenderFactory : IMailSenderFactory
    {
        public IMailSender CreateMailerService()
        {
            return new MailSender();
        }
    }
}
