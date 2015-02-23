using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.MobileTokens
{
    class MobileTokenRepositoryMock : GenericRepositoryMock<MobileToken>
    {
        protected override List<MobileToken> Seed()
        {
            var person1 = new Person
            {
                Id = 1
            };
            var person2 = new Person
            {
                Id = 2
            };

            return new List<MobileToken>
            {
                new MobileToken
                {
                    Id = 1,
                    Token = "token1",
                    Status = MobileTokenStatus.Activated,
                    Person = person1,
                    PersonId = 1
                },
                new MobileToken
                {
                    Id = 2,
                    Token = "token2",
                    Status = MobileTokenStatus.Created,
                    Person = person1,
                    PersonId = 1
                },
                new MobileToken
                {
                    Id = 3,
                    Token = "token3",
                    Status = MobileTokenStatus.Created,
                    Person = person2,
                    PersonId = 2
                }
            };
        }
    }
}
