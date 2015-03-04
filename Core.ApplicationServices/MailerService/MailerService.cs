using System.Net.Mail;

namespace Core.ApplicationServices.MailerService
{
    public class MailerService : IMailerService
    {
        public void SendMail(string to, string from, string subject, string body, ISmtpClient smtpClient)
        {
            var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(from);
            msg.Body = body;
            msg.Subject = subject;
            smtpClient.Send(msg);
        }
    }
}
