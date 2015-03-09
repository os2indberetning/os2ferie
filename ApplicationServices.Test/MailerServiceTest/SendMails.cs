using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using NSubstitute;
using NUnit.Framework;
using Quartz;

namespace ApplicationServices.Test.MailerServiceTest
{
    [TestFixture]
    public class SendMails
    {
        private MailService uut;
        private DriveRepoMock driveRepoMock;
        private SubRepoMock subRepoMock;

        [SetUp]
        public void SetUp()
        {
            driveRepoMock = new DriveRepoMock();
            driveRepoMock.ReSeed();
            subRepoMock = new SubRepoMock();
            subRepoMock.ReSeed();
        }

        [Test]
        public void SendMails_ShouldCall_GetLeadersWithPendingReportsMail()
        {
            var senderMock = Substitute.For<IMailSender>();

            uut = new MailService(driveRepoMock, subRepoMock, senderMock);
            uut.SendMails();
            senderMock.ReceivedWithAnyArgs().SendMail("", "", "");
        }
    }
}