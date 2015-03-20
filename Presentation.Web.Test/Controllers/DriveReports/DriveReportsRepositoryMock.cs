
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.DriveReports
{
    public class DriveReportsRepositoryMock : GenericRepositoryMock<DriveReport>
    {
        readonly Person _person = new Person
        {
            FirstName = "Morten",
            LastName = "Tester",
            Initials = "MT",
            Mail = "testMail@asd.dk"
        };

        readonly Person _person2 = new Person
        {
            FirstName = "Morten",
            LastName = "Tester",
            Initials = "MT",
            Mail = "AndenTestMail@asd.dk"
        };



        protected override List<DriveReport> Seed()
        {
      
            return new List<DriveReport>
            {
                new DriveReport
                {
                    Id = 1,
                    Comment = "comment 1",
                    Distance = 3.4f,
                    ClosedDateTimestamp = 4444,
                    Person = _person2
                },
                new DriveReport
                {
                    Id = 2,
                    Comment = "comment 2",
                    Distance = 3.5f,
                    ClosedDateTimestamp = 4455,
                    Person = _person2
                },
                new DriveReport
                {
                    Id = 3,
                    Comment = "comment 3",
                    Distance = 3.6778f,
                    ClosedDateTimestamp = 7777,
                    Person = _person
                }
            };
        }
        
        public override DriveReport Insert(DriveReport entity)
        {
            entity.Person = _person;
            return base.Insert(entity);
        }
    }
}
