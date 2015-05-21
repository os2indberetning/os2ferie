
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.Substitutes
{
    public class SubstituteRepositoryMock : GenericRepositoryMock<Substitute>
    {

        public static Person p1 = new Person()
        {
            FirstName = "Jacob",
            LastName = "Jensen",
            CprNumber = "123",
            Mail = "jacob@test.dk",
        };

        public static Person p2 = new Person()
        {
            FirstName = "Morten",
            LastName = "Rasmussen",
            CprNumber = "321",
            Mail = "Morten@test.dk"
        };

        protected override List<Substitute> Seed()
        {
            return new List<Substitute>
            {
                new Substitute
                {
                    Id  = 1,
                    EndDateTimestamp = 1337,
                    StartDateTimestamp = 1336,
                    Leader = p1,
                    Sub = p2,
                    Person = p2
                },
                new Substitute
                {
                    Id  = 2,
                    EndDateTimestamp = 2337,
                    StartDateTimestamp = 2336,
                    Leader = p1,
                    Sub = p2,
                    Person = p2
                },
                new Substitute
                {
                    Id  = 3,
                    EndDateTimestamp = 3337,
                    StartDateTimestamp = 3336,
                    Leader = p1,
                    Sub = p2,
                    Person = p2
                }
            };
        }
    }
}
