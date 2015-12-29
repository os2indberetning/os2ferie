using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mail.LogMailer;
using NUnit.Framework;

namespace ConsoleApplications.Test.LogMailer
{
    [TestFixture]
    public class LogParserTests
    {

        [Test]
        public void Messages_CanParseSingleLine()
        {
            var logParser = new LogParser();
            var log = new List<string>
            {
                "28/12/2015 15:21:14 : Exception doing post of type Core.DomainModel.Substitute"
            };

            var date = new DateTime(2015, 12, 28, 0, 0, 0);
            var messages = logParser.Messages(log, date);
            Assert.AreEqual("Exception doing post of type Core.DomainModel.Substitute", messages.First());
        }

        [Test]
        public void Messages_IgnoresOldEntries()
        {
            var logParser = new LogParser();
            var log = new List<string>
            {
                "22/12/2015 15:21:14 : Exception doing post of type Core.DomainModel.Substitute"
            };

            var date = new DateTime(2015, 12, 28, 0, 0, 0);
            var messages = logParser.Messages(log, date);
            Assert.AreEqual(messages.Count(), 0);
        }

    }
}
