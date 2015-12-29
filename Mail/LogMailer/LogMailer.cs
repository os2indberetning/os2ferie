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

            var lines = _logReader.Read("C:\\logs\\os2eindberetning\\web.log");

            var message = String.Join(Environment.NewLine, _logParser.Messages(lines, DateTime.Now.AddDays(-1)));

            foreach (var receiver in receivers)
            {
                _mailSender.SendMail(receiver, "Log", message);
            }
            
        
        }

    }
}
