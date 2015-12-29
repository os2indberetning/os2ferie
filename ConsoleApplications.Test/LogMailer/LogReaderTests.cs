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
    public class LogReaderTests
    {


        [Test]
        public void Read_CanReadAllLinesInLog()
        {
            var logReader = new LogReader();

            var log = logReader.Read("LogMailer/web.log");

            Assert.AreEqual(6, log.Count());

        }

    }
}
