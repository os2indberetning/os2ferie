using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Core.ApplicationServices.MailerService.Interface;
using Ninject;
using Core.ApplicationServices.Logger;

namespace Core.ApplicationServices.MailerService.Impl
{
    public class MailSender : IMailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger _logger;

        public MailSender(ILogger logger)
        {
            _logger = logger;
            _smtpClient = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["PROTECTED_SMTP_HOST"],
                Port = int.Parse(ConfigurationManager.AppSettings["PROTECTED_SMTP_HOST_PORT"]),
                EnableSsl = false,
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
            if (String.IsNullOrWhiteSpace(to))
            {
                _logger.Log("Email adressen er tom", "mail");
                return;
            }
            var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(ConfigurationManager.AppSettings["PROTECTED_MAIL_FROM_ADDRESS"]);
            msg.Body = body;
            msg.Subject = subject;
            try
            {
                _smtpClient.Send(msg);
            }
            catch (Exception e )
            {
                _logger.Log("Fejl under afsendelse af mail", "mail");
            }
        }
    }
}
