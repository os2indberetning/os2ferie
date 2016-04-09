using System.Linq;
using Mail.LogMailer;
using NUnit.Framework;

namespace Mail.Test.LogMailer
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
