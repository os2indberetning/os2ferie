using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.MobileTokens
{
    class MobileTokenRepositoryMock : GenericRepositoryMock<MobileToken>
    {
        protected override List<MobileToken> Seed()
        {
            return new List<MobileToken>
            {
                new MobileToken
                {
                    Id = 1,
                    Token = "token1",
                    Status = MobileTokenStatus.Activated
                },
                new MobileToken
                {
                    Id = 2,
                    Token = "token2",
                    Status = MobileTokenStatus.Created
                },
                new MobileToken
                {
                    Id = 3,
                    Token = "token3",
                    Status = MobileTokenStatus.Deleted
                }
            };
        }
    }
}
