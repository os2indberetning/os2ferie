using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.ApplicationServices.MailerService.Interface;

namespace Mail.LogMailer
{
    public class LogMailer : ILogMailer
    {
        private readonly ILogParser _logParser;
        private readonly ILogReader _logReader;
        private readonly IMailSender _mailSender;

        public LogMailer(ILogParser logParser, ILogReader logReader, IMailSender mailSender)
        {
            _logParser = logParser;
            _logReader = logReader;
            _mailSender = mailSender;
        }

        public void Send()
        {

            var configvalue = ConfigurationManager.AppSettings["PROTECTED_DailyErrorLogMail"];

            configvalue = Regex.Replace(configvalue, @"\s+", "");

            var receivers = configvalue.Split(',');

            var webLines = _logReader.Read("C:\\logs\\os2eindberetning\\web.log");
            var dmzLines = _logReader.Read("C:\\logs\\os2eindberetning\\dmz.log");
            var mailLines = _logReader.Read("C:\\logs\\os2eindberetning\\mail.log");

            var webMessage = String.Join(Environment.NewLine, _logParser.Messages(webLines, DateTime.Now.AddDays(-1)));
            var dmzMessage = String.Join(Environment.NewLine, _logParser.Messages(dmzLines, DateTime.Now.AddDays(-1)));
            var mailMessage = String.Join(Environment.NewLine, _logParser.Messages(mailLines, DateTime.Now.AddDays(-1)));

            var newLine = System.Environment.NewLine;

            var result = "";

            // Only add each header if there are log messages in that category.
            if (webMessage.Any())
            {
                result += "Web:" + newLine + newLine + webMessage + newLine + newLine;
            }
            if (dmzMessage.Any())
            {
                result += "DMZ: " + newLine + newLine + dmzMessage + newLine + newLine;
            }
            if (mailMessage.Any())
            {
                result += "Mail: " + newLine + newLine + mailMessage;
            }

            if (result == "")
            {
                result = "Ingen fejl registreret";
            }

            foreach (var receiver in receivers)
            {
                _mailSender.SendMail(receiver, "Log", result);
            }
            
        
        }

    }
}
