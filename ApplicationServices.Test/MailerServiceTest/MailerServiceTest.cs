using System.Net.Mail;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService;

using NSubstitute;
using NUnit.Framework;


namespace ApplicationServices.Test.MailerServiceTest
{

    [TestFixture]
    public class MailerServiceTest
    {
        private IMailerService uut;
        private ISmtpClient smtp;

        [SetUp]
        public void SetUp()
        {
            uut = new MailerService();
            smtp = Substitute.For<ISmtpClient>();
        }

        [Test]
        public void SendMail_ShouldCall_SmtpClient_Send()
        {
            uut.SendMail("To@to.com","From@from.com","Subject","Body",smtp);
            smtp.ReceivedWithAnyArgs().Send(new MailMessage());
        }

    }
}