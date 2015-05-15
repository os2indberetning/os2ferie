using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Microsoft.Owin.Security;
using NUnit.Framework;
using Presentation.Web.Test.Controllers.Models;
using Presentation.Web.Test.Controllers.Persons;
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
                    new KeyValuePair<Type, Type>(typeof (IGenericRepository<Person>), typeof (PersonRepositoryMock)),
                    new KeyValuePair<Type, Type>(typeof (IGenericRepository<Employment>), typeof (EmploymentRepositoryMock)),
                    new KeyValuePair<Type, Type>(typeof (IGenericRepository<OrgUnit>), typeof (OrgUnitRepositoryMock)),
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
                Employment = new Employment()
                {
                    Id = 1,
                    OrgUnitId = 1
                },
                FullName = "Fissirul Lehmann [FL]",
                Status = ReportStatus.Rejected
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
                Employment = new Employment()
                {
                    Id = 1,
                    OrgUnitId = 1
                },
                FullName = "Fissirul Lehmann [FL]",
                Status = ReportStatus.Accepted
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
                FullName = "Fissirul Lehmann [FL]",
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
                FullName = "Morten Tester [MT]"
            };
        }

        protected override DriveReport GetPatchReferenceEntity()
        {
            return new DriveReport
            {
                Id = 3,
                Comment = "comment patched",
                Distance = 3.6778f,
                PersonId = 1,
                Employment = new Employment()
                {
                    Id = 1,
                    OrgUnitId = 1
                },
                FullName = "Fissirul Lehmann [FL]",
                Status = ReportStatus.Pending
            };
        }

        protected override void AsssertEqualEntities(DriveReport entity1, DriveReport entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two drive reports should be the same");
            Assert.AreEqual(entity1.Comment, entity2.Comment, "Comment of the two drive reports should be the same");
            Assert.AreEqual(entity1.DriveReportPoints, entity2.DriveReportPoints, "DriveReportPoints of the two drive reports should be the same");
            Assert.AreEqual(entity1.Comment, entity2.Comment, "Comment of the two drive reports should be the same");
            Assert.AreEqual(entity1.FullName, entity2.FullName, "Full name should be the same on both reports");
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
            new OrgUnitRepositoryMock().ReSeed();
            new EmploymentRepositoryMock().ReSeed();
            MailSenderMock.SendHasBeenCalled = false;
            MailSenderMock.Body = "";
            MailSenderMock.Subject = "";
            MailSenderMock.To = "";
        }

        [Test]
        protected override async Task PostShouldInsertAnEntity()
        {
            //Is tested via drive report service
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
