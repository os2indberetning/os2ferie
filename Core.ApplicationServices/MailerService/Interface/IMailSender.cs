using System.Net.Mail;

namespace Core.ApplicationServices.MailerService.Interface
{
    public interface IMailSender
    {
        void SendMail(string to, string subject, string body);
    }
}
