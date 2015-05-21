
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.DriveReports
{
    public class DriveReportsRepositoryMock : GenericRepositoryMock<DriveReport>
    {
        private readonly Person _person1 = new Person
        {
            Id = 1,
            CprNumber = "1234567894",
            FirstName = "Fissirul",
            LastName = "Lehmann",
            IsAdmin = true,
            Initials = "FL",
            Mail = "testMail@asd.dk"
        };

        

        readonly Person _person2 = new Person
        {
            Id = 2,
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
                    Person = _person1,
                     Employment = new Employment()
                    {
                        Id = 1,
                        OrgUnitId = 1
                    },
                    FullName = "Fissirul Lehmann [FL]",
                    Status = ReportStatus.Rejected
                },
                new DriveReport
                {
                    Id = 2,
                    Comment = "comment 2",
                    Distance = 3.5f,
                    ClosedDateTimestamp = 4455,
                    Person = _person1,
                     Employment = new Employment()
                    {
                        Id = 1,
                        OrgUnitId = 1
                    },
                    FullName = "Fissirul Lehmann [FL]",
                    Status = ReportStatus.Accepted
                },
                new DriveReport
                {
                    Id = 3,
                    Comment = "comment 3",
                    Distance = 3.6778f,
                    Person = _person1,
                    PersonId = 2302,
                    Employment = new Employment()
                    {
                        Id = 1,
                        OrgUnitId = 1
                    },
                    FullName = "Heidi Søndergaard Huber [hshu]",
                    Status = ReportStatus.Pending
                },
            };
        }

        public override DriveReport Insert(DriveReport entity)
        {
            entity.Person = _person1;
            return base.Insert(entity);
        }
    }
}
