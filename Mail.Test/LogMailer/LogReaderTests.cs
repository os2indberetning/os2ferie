using System.Linq;
using Mail.LogMailer;
using NUnit.Framework;
using System;
using System.IO;

namespace Mail.Test.LogMailer
{
    [TestFixture]
    public class LogReaderTests
    {


        [Test]
        public void Read_CanReadAllLinesInLog()
        {
            var logReader = new LogReader();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"LogMailer\test-web.txt");
            var log = logReader.Read(path);

            Assert.AreEqual(6, log.Count());

        }

    }
}
