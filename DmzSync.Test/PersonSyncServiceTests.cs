using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DmzSync.Services.Impl;
using Infrastructure.DmzSync.Services.Interface;
using NSubstitute;
using NUnit.Framework;

namespace DmzSync.Test
{
    [TestFixture]
    public class PersonSyncServiceTests
    {

        private ISyncService _uut;
        private IGenericRepository<Profile> _masterRepoMock; 
        private IGenericRepository<Person> _dmzRepoMock;
        private List<Token> _dmzTokenList = new List<Profile>();
        private List<MobileToken> _masterTokenList = new List<MobileToken>();

        [SetUp]
        public void SetUp()
        {
            _dmzRepoMock = NSubstitute.Substitute.For<IGenericRepository<Token>>();
            _masterRepoMock = NSubstitute.Substitute.For<IGenericRepository<MobileToken>>();

            _dmzTokenList.Add(new Token()
            {
                Id = 1,
                GuId = new Guid().ToString(),
                Status = 1,
                ProfileId = 1,
                TokenString = "1234",
            });
            _dmzTokenList.Add(new Token()
            {
                Id = 2,
                GuId = new Guid().ToString(),
                Status = 1,
                ProfileId = 3,
                TokenString = "1234",
            });
            _dmzTokenList.Add(new Token()
            {
                Id = 4,
                GuId = new Guid().ToString(),
                Status = 1,
                ProfileId = 3,
                TokenString = "1234",
            });
            _dmzRepoMock.AsQueryable().ReturnsForAnyArgs(_dmzTokenList.AsQueryable());
            _dmzRepoMock.WhenForAnyArgs(x => x.Delete(new Token())).Do(p => _dmzTokenList.Remove(p.Arg<Token>()));

            _masterTokenList.Add(new MobileToken()
            {
                Id = 1,
                Guid = new Guid(_dmzTokenList.ElementAt(0).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 1,
                Token = "1234"
            });
            _masterTokenList.Add(new MobileToken()
            {
                Id = 2,
                Guid = new Guid(_dmzTokenList.ElementAt(1).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 3,
                Token = "1234"
            });
            _masterTokenList.Add(new MobileToken()
            {
                Id = 4,
                Guid = new Guid(_dmzTokenList.ElementAt(2).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 3,
                Token = "1234"
            });

            _dmzRepoMock.WhenForAnyArgs(x => x.Insert(new Token())).Do(t => _dmzTokenList.Add(t.Arg<Token>()));

            _masterRepoMock.AsQueryable().ReturnsForAnyArgs(_masterTokenList.AsQueryable());


            _uut = new TokenSyncService(_dmzRepoMock,_masterRepoMock);
        }

        [Test]
        public void ClearDmz_ShouldCallDelete_OnceForEachToken()
        {
            var numberOfReceivedCalls = 0;
            _dmzRepoMock.WhenForAnyArgs(x => x.Delete(new Token())).Do(p => numberOfReceivedCalls++);
            _uut.ClearDmz();
            Assert.AreEqual(3, numberOfReceivedCalls);
        }

        [Test]
        public void SyncFromDmz_ShouldUpdateTokenStatus()
        {
            _uut.SyncFromDmz();
            Assert.AreEqual(MobileTokenStatus.Activated, _masterRepoMock.AsQueryable().ElementAt(0).Status);
        }

        [Test]
        public void SyncToDmz_ShouldCreateTokensInDmz()
        {
            _dmzTokenList.Clear();
            _uut.SyncToDmz();
            Assert.AreEqual(3, _dmzRepoMock.AsQueryable().Count());
        }
    }
}
