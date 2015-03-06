using System.Configuration;
using System.Net;
using System.Net.Mail;
using Core.ApplicationServices.MailerService.Interface;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailSender : IMailSender
    {
        private readonly SmtpClient _smtpClient;

        public MailSender()
        {
            _smtpClient = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["SMTP_HOST"],
                Port = int.Parse(ConfigurationManager.AppSettings["SMTP_HOST_PORT"]),
                Credentials = new NetworkCredential()
                {
                    UserName = ConfigurationManager.AppSettings["SMTP_USER"],
                    Password = ConfigurationManager.AppSettings["SMTP_PASSWORD"]
                }
               
            };
            
        }

        public void SendMail(string to, string subject, string body)
        {
            var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(ConfigurationManager.AppSettings["MAIL_FROM"]);
            msg.Body = body;
            msg.Subject = subject;
            _smtpClient.Send(msg);
        }
    }
}
