using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Microsoft.Owin.Security;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.DriveReports
{
    [TestFixture]
    public class DriveReportsTest : BaseControllerTest<DriveReport>
    {

   
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<DriveReport>),typeof (DriveReportsRepositoryMock)),
                    new KeyValuePair<Type, Type>(typeof (IMailSender), typeof (MailSenderMock)),
                    new KeyValuePair<Type, Type>(typeof(IGenericRepository<LicensePlate>), typeof(GenericRepository<LicensePlate>))
            };
        }

        protected override DriveReport GetReferenceEntity1()
        {
            return new DriveReport
            {
                Id = 1,
                Comment = "comment 1",
                Distance = 3.4f,
                ClosedDateTimestamp = 4444,
                Fullname = "Morten Tester"
            };
        }

        protected override DriveReport GetReferenceEntity2()
        {
            return new DriveReport
            {
                Id = 2,
                Comment = "comment 2",
                Distance = 3.5f,
                ClosedDateTimestamp = 4455,
                Fullname = "Morten Tester"
            };
        }

        protected override DriveReport GetReferenceEntity3()
        {
            return new DriveReport
            {
                Id = 3,
                Comment = "comment 3",
                Distance = 3.6778f,
                ClosedDateTimestamp = 7777,
                Fullname = "Morten Tester",
            };
        }

        protected override DriveReport GetPostReferenceEntity()
        {
            return new DriveReport
            {
                Id = 4,
                Comment = "comment posted",
                Distance = 3.6778f,
                ClosedDateTimestamp = 7777,
                Fullname = "Morten Tester"
            };
        }

        protected override DriveReport GetPatchReferenceEntity()
        {
            return new DriveReport
            {
                Id = 3,
                Comment = "comment patched",
                Distance = 3.6778f,
                ClosedDateTimestamp = 666,
                Fullname = "Morten Tester"
            };
        }

        protected override void AsssertEqualEntities(DriveReport entity1, DriveReport entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two drive reports should be the same");
            Assert.AreEqual(entity1.Comment, entity2.Comment, "Comment of the two drive reports should be the same");
            Assert.AreEqual(entity1.DriveReportPoints, entity2.DriveReportPoints, "DriveReportPoints of the two drive reports should be the same");
            Assert.AreEqual(entity1.Comment, entity2.Comment, "Comment of the two drive reports should be the same");
            Assert.AreEqual(entity1.Fullname, entity2.Fullname, "Full name should be the same on both reports");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'PersonId': 4,
                        'Comment' : 'comment posted',
                        'Distance' : 3.6778,
                        'ClosedDateTimestamp' : 7777  
            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Comment' : 'comment patched',
                        'ClosedDateTimestamp' : 666
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/DriveReports";
        }

        protected override void ReSeed()
        {
            new DriveReportsRepositoryMock().ReSeed();
            MailSenderMock.SendHasBeenCalled = false;
            MailSenderMock.Body = "";
            MailSenderMock.Subject = "";
            MailSenderMock.To = "";
        }

        [Test]
        public override async Task PostShouldInsertAnEntity()
        {
            //Is tested via drive report service
        }

        
        [Test]
        public async void DriveReport_PatchWithStatus_Rejected_ShouldSendMail()
        {
            const string bodyContent = @"{
                        'Status' : 'Rejected',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual(true, MailSenderMock.SendHasBeenCalled);
        }

        [Test]
        public async void DriveReport_PatchWithStatus_Rejected_ShouldSendMailToCorrectRecipient()
        {
            const string bodyContent = @"{
                        'Status' : 'Rejected',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual("testMail@asd.dk", MailSenderMock.To);
        }

        [Test]
        public async void DriveReport_PatchWithStatus_Rejected_ShouldSendMailWithCorrectSubject()
        {
            const string bodyContent = @"{
                        'Status' : 'Rejected',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual("Afvist indberetning", MailSenderMock.Subject);
        }

        [Test]
        public async void DriveReport_PatchWithStatus_Rejected_ShouldSendMailWithCorrectBody()
        {
            const string bodyContent = @"{
                        'Status' : 'Rejected',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual("Din indberetning er blevet afvist med kommentaren: \n \n" + "TestComment", MailSenderMock.Body);
        }

        [Test]
        public async void DriveReport_PatchWithStatus_Accepted_ShouldNotSendMail()
        {
            const string bodyContent = @"{
                        'Status' : 'Accepted',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);
        }

        [Test]
        public async void DriveReport_PatchWithStatus_Pending_ShouldNotSendMail()
        {
            const string bodyContent = @"{
                        'Status' : 'Pending',
                        'Comment': 'TestComment'
                    }";

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(bodyContent))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");

            Assert.AreEqual(false, MailSenderMock.SendHasBeenCalled);
        }
    }

    public class MailSenderMock : IMailSender
    {
        public static bool SendHasBeenCalled { get; set; }
        public static string To { get; set; }
        public static string Subject { get; set; }
        public static string Body { get; set; }

        public MailSenderMock()
        {
            SendHasBeenCalled = false;
            To = "";
            Subject = "";
            Body = "";
        }

        public void SendMail(string to, string subject, string body)
        {
            SendHasBeenCalled = true;
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
