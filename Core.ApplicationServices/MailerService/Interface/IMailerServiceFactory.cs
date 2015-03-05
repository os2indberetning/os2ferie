using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.MailerService
{
    public interface IMailSenderFactory
    {
        IMailSender CreateMailerService();
    }
}
