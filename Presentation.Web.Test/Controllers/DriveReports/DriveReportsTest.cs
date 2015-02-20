using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Presentation.Web.Test.Controllers.DriveReports
{
    [TestClass]
    public class DriveReportsTest : BaseControllerTest<DriveReport>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<DriveReport>),
                    typeof (DriveReportsRepositoryMock))
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
                Fullname = "Morten Tester"
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
                        'Id': 4,
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
        }
    }
}
