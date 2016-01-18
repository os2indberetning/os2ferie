using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.MobileTokenServiceTest
{
    [TestFixture]
    public class MobileTokenServiceTest
    {

        private List<MobileToken> list;
        IGenericRepository<MobileToken> repoMock = NSubstitute.Substitute.For<IGenericRepository<MobileToken>>();
        IGenericRepository<Token> repoTokenMock = NSubstitute.Substitute.For<IGenericRepository<Token>>();

        [SetUp]
        public void SetUp()
        {
            list = new List<MobileToken>();
            repoMock.AsQueryable().ReturnsForAnyArgs(list.AsQueryable());
        }

        [Test]
        public void Create_CalledTwice_TokensNotIdentical()
        {
            
            var token1 = new MobileToken();
            var token2 = new MobileToken();

            repoMock.Insert(new MobileToken())
                .ReturnsForAnyArgs(x => x.Arg<MobileToken>())
                .AndDoes(r => list.Add(r.Arg<MobileToken>()));
            repoTokenMock.Insert(new Token()).ReturnsForAnyArgs(x => x.Arg<Token>());

            var service = new MobileTokenService(repoMock, repoTokenMock);

            var tokenOne = service.Create(token1);
            var tokenTwo = service.Create(token2);

            Assert.That(tokenOne.Token, Is.Not.EqualTo(tokenTwo.Token));
        }
    }
}