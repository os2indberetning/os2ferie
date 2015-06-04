using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Core.ApplicationServices.MailerService.Interface;
using Ninject;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailSender : IMailSender
    {
        private readonly SmtpClient _smtpClient;

        public MailSender()
        {
            _smtpClient = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["PROTECTED_SMTP_HOST"],
                Port = int.Parse(ConfigurationManager.AppSettings["PROTECTED_SMTP_HOST_PORT"]),
                EnableSsl = true,
                Credentials = new NetworkCredential()
                {
                    UserName = ConfigurationManager.AppSettings["PROTECTED_SMTP_USER"],
                    Password = ConfigurationManager.AppSettings["PROTECTED_SMTP_PASSWORD"]
                }

            };

        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="to">Email address of recipient.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email.</param>
        public void SendMail(string to, string subject, string body)
        {
            var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(ConfigurationManager.AppSettings["PROTECTED_MAIL_FROM_ADDRESS"]);
            msg.Body = body;
            msg.Subject = subject;
            try
            {
                _smtpClient.Send(msg);
            }
            catch (Exception)
            {

            }
        }
    }
}
