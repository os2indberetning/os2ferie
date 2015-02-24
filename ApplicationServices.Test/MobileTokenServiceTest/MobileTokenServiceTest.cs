using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.MobileTokenServiceTest
{
    [TestFixture]
    public class MobileTokenServiceTest
    {
        [Test]
        public void Create_CalledTwice_TokensNotIdentical()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<MobileToken>>();

            var token1 = new MobileToken();
            var token2 = new MobileToken();

            repoMock.AsQueryable().Returns(info => new List<MobileToken>().AsQueryable());
            repoMock.Insert(token1).Returns(token1);
            repoMock.Insert(token2).Returns(token2);
            repoMock.When(repository => repository.Save()).Do(info => { });

            var service = new MobileTokenService(repoMock);

            var tokenOne = service.Create(token1);
            var tokenTwo = service.Create(token2);

            Assert.That(tokenOne.Token, Is.Not.EqualTo(tokenTwo.Token));
        }
    }
}