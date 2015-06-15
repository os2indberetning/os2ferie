using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;

using NUnit.Framework;
using Substitute = NSubstitute.Substitute;


namespace ApplicationServices.Test.MailerServiceTest
{

    [TestFixture]
    public class GetLeadersWithPendingReportsEmails
    {
        private SubRepoMock subRepoMock;
        private DriveRepoMock driveRepoMock;
        private MailService uut;

        [SetUp]
        public void SetUp()
        {
            subRepoMock = new SubRepoMock();
            subRepoMock.ReSeed();
            driveRepoMock = new DriveRepoMock();
            driveRepoMock.ReSeed();

            var senderMock = Substitute.For<IMailSender>();

            uut = new MailService(driveRepoMock, subRepoMock, senderMock, null);
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnSubAndLeaderMail()
        {
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p1,
                Sub = DriveRepoMock.p2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1

            });

            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.Member("Preben@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldNotReturnSubMail()
        {
            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.No.Member("Preben@g.dk"));
            Assert.That(res, Has.Member("Lars@g.dk"));
        }


        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturn2SubMails()
        {
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p1,
                Sub = DriveRepoMock.p2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1
            });
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p3,
                Sub = DriveRepoMock.p4,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p3
            });

            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.No.Member("Lars@g.dk"));
            Assert.That(res, Has.No.Member("Ivan@g.dk"));
            Assert.That(res, Has.Member("Frank@g.dk"));
            Assert.That(res, Has.Member("Preben@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnDifferentSubAndLeaderMail()
        {
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p3,
                Sub = DriveRepoMock.p4,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1
            });

            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.Member("Frank@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnEmpty_WhenNoUsersWantToReceiveMail()
        {
            DriveRepoMock.p1.RecieveMail = false;
            DriveRepoMock.p2.RecieveMail = false;
            DriveRepoMock.p3.RecieveMail = false;
            DriveRepoMock.p4.RecieveMail = false;
            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.IsEmpty(res);
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnEmpty_WhenNoSubsAreSet_AndNoLeadersWantToReceiveMails()
        {
            DriveRepoMock.p1.RecieveMail = false;
            DriveRepoMock.p2.RecieveMail = true;
            DriveRepoMock.p3.RecieveMail = false;
            DriveRepoMock.p4.RecieveMail = true;
            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.IsEmpty(res);
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnEmpty_WhenAllReportsAreRejected()
        {

            DriveRepoMock.dr1.Status = ReportStatus.Rejected;
            DriveRepoMock.dr2.Status = ReportStatus.Rejected;
            DriveRepoMock.dr3.Status = ReportStatus.Rejected;
            DriveRepoMock.dr4.Status = ReportStatus.Rejected;

            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.IsEmpty(res);
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturnEmpty_WhenAllReportsAreAccepted()
        {

            DriveRepoMock.dr1.Status = ReportStatus.Accepted;
            DriveRepoMock.dr2.Status = ReportStatus.Accepted;
            DriveRepoMock.dr3.Status = ReportStatus.Accepted;
            DriveRepoMock.dr4.Status = ReportStatus.Accepted;

            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.IsEmpty(res);
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturn_OnlyOneMail_WhenTheSameUserIsLeaderForMoreReports()
        {
            DriveRepoMock.e1.Person = DriveRepoMock.p1;
            DriveRepoMock.e2.Person = DriveRepoMock.p1;
            DriveRepoMock.e3.Person = DriveRepoMock.p1;
            DriveRepoMock.e4.Person = DriveRepoMock.p1;
            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.AreEqual(1, res.Count());
            Assert.That(res,Has.Member("Lars@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldReturn_OnlyOneSubMail_WhenTheSameUserIsLeaderForMoreReports()
        {
            DriveRepoMock.e1.Person = DriveRepoMock.p1;
            DriveRepoMock.e2.Person = DriveRepoMock.p1;
            DriveRepoMock.e3.Person = DriveRepoMock.p1;
            DriveRepoMock.e4.Person = DriveRepoMock.p1;

            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p1,
                Sub = DriveRepoMock.p2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1
            });

            var res = uut.GetLeadersWithPendingReportsMails();
            Assert.AreEqual(1, res.Count());
            Assert.That(res, Has.Member("Preben@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldNotReturn_SubMails_WhenStartDateIsAfterToday()
        {
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p1,
                Sub = DriveRepoMock.p2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1
            });
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p3,
                Sub = DriveRepoMock.p4,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p3
            });

            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.Member("Lars@g.dk"));
            Assert.That(res, Has.Member("Ivan@g.dk"));
            Assert.That(res, Has.No.Member("Frank@g.dk"));
            Assert.That(res, Has.No.Member("Preben@g.dk"));
        }

        [Test]
        public void MailService_GetLeadersWithPendingReportsMails_ShouldNotReturn_SubMails_WhenEndDateIsBeforeToday()
        {
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p1,
                Sub = DriveRepoMock.p2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p1
            });
            subRepoMock.Substitutes.Add(new Core.DomainModel.Substitute()
            {
                Id = 1,
                Leader = DriveRepoMock.p3,
                Sub = DriveRepoMock.p4,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.AddDays(-2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Person = DriveRepoMock.p3
            });

            var res = uut.GetLeadersWithPendingReportsMails();

            Assert.That(res, Has.Member("Lars@g.dk"));
            Assert.That(res, Has.Member("Ivan@g.dk"));
            Assert.That(res, Has.No.Member("Frank@g.dk"));
            Assert.That(res, Has.No.Member("Preben@g.dk"));
        }
    }
}