using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Presentation.Web.Test.Controllers.MobileTokens
{
    [TestClass]
    public class MobileTokenTest : BaseControllerTest<MobileToken>
    {

        protected override void AsssertEqualEntities(MobileToken m1, MobileToken m2)
        {
            Assert.AreEqual(m1.Id, m2.Id, "ID of two mobile tokens does not match");
            Assert.AreEqual(m1.Token, m2.Token, "Token of two mobile tokens does not match");
            Assert.AreEqual(m1.Status, m2.Status, "Status of two mobile tokens does not match");
        }


        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<MobileToken>),
                    typeof (MobileTokenRepositoryMock))
            };
        }

        protected override MobileToken GetReferenceEntity1()
        {
            return new MobileToken
        {
            Id = 1,
            Token = "token1",
            Status = MobileTokenStatus.Activated
        };
        }

        protected override MobileToken GetReferenceEntity2()
        {
            return new MobileToken
                    {
                        Id = 2,
                        Token = "token2",
                        Status = MobileTokenStatus.Created
                    };
        }

        protected override MobileToken GetReferenceEntity3()
        {
            return new MobileToken
                    {
                        Id = 3,
                        Token = "token3",
                        Status = MobileTokenStatus.Deleted
                    };
        }

        protected override MobileToken GetPostReferenceEntity()
        {
            return new MobileToken
                    {
                        Id = 4,
                        Token = "posted token",
                        Status = MobileTokenStatus.Activated
                    };
        }
        
        protected override MobileToken GetPatchReferenceEntity()
        {
            return new MobileToken
            {
                Id = 3,
                Token = "patched token",
                Status = MobileTokenStatus.Activated
            };
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id' : 4,
                        'Token' : 'posted token',
                        'Status' : 'Activated'
                    }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Token' : 'patched token',
                        'Status' : 'Activated'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/MobileToken";
        }

        protected override void ReSeed()
        {
            new MobileTokenRepositoryMock().ReSeed();
        }
    }
}
