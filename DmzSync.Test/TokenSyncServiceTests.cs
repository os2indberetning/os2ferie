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
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Impl;
using Infrastructure.DmzSync.Services.Interface;
using NSubstitute;
using NUnit.Framework;

namespace DmzSync.Test
{
    [TestFixture]
    public class TokenSyncServiceTests
    {

        private ISyncService _uut;
        private IGenericRepository<MobileToken> _masterRepoMock; 
        private IGenericRepository<Token> _dmzRepoMock;
        private List<Token> _dmzTokenList;
        private List<MobileToken> _masterTokenList;

        [SetUp]
        public void SetUp()
        {
            _dmzTokenList = new List<Token>();
            _masterTokenList = new List<MobileToken>();

            _dmzRepoMock = NSubstitute.Substitute.For<IGenericRepository<Token>>();
            _masterRepoMock = NSubstitute.Substitute.For<IGenericRepository<MobileToken>>();

            _dmzTokenList.Add(new Token()
            {
                Id = 7,
                GuId = Guid.NewGuid().ToString(),
                Status = 1,
                ProfileId = 1,
                
                TokenString = "1234",
            });
            _dmzTokenList.Add(new Token()
            {
                Id = 8,
                GuId = Guid.NewGuid().ToString(),
                Status = 1,
                ProfileId = 3,
                TokenString = "1234",
            });
            _dmzTokenList.Add(new Token()
            {
                Id = 9,
                GuId = new Guid().ToString(),
                Status = 1,
                ProfileId = 3,
                TokenString = "1234",
            });
            _dmzRepoMock.AsQueryable().ReturnsForAnyArgs(_dmzTokenList.AsQueryable());
            _dmzRepoMock.WhenForAnyArgs(x => x.DeleteRange(_dmzTokenList)).Do(p => _dmzTokenList.Clear());

            _masterTokenList.Add(new MobileToken()
            {
                Id = 1,
                Guid = new Guid(_dmzTokenList.ElementAt(0).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 1,
                Person = new Person()
                {
                    IsActive = true
                },
                Token = "1234"
            });
            _masterTokenList.Add(new MobileToken()
            {
                Id = 2,
                Guid = new Guid(_dmzTokenList.ElementAt(1).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 3,
                Person = new Person()
                {
                    IsActive = true
                },
                Token = "1234"
            });
            _masterTokenList.Add(new MobileToken()
            {
                Id = 4,
                Guid = new Guid(_dmzTokenList.ElementAt(2).GuId),
                Status = MobileTokenStatus.Created,
                PersonId = 3,
                Person = new Person()
                {
                    IsActive = true
                },
                Token = "1234"
            });

            _dmzRepoMock.WhenForAnyArgs(x => x.Insert(new Token())).Do(t => _dmzTokenList.Add(t.Arg<Token>()));

            _masterRepoMock.AsQueryable().ReturnsForAnyArgs(_masterTokenList.AsQueryable());

            for (int i = 0; i < _dmzTokenList.Count; i++)
            {
                Encryptor.EncryptToken(_dmzTokenList.ToArray()[i]);
            }

            _uut = new TokenSyncService(_dmzRepoMock,_masterRepoMock);
        }

        [Test]
        public void ClearDmz_ShouldCallDeleteRange()
        {
            var numberOfReceivedCalls = 0;
            _dmzRepoMock.WhenForAnyArgs(x => x.DeleteRange(_dmzTokenList)).Do(p => numberOfReceivedCalls++);
            _uut.ClearDmz();
            Assert.AreEqual(1, numberOfReceivedCalls);
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
            _uut.ClearDmz();
            _uut.SyncToDmz();
            var res = _dmzRepoMock.AsQueryable();
            Assert.AreEqual(3, res.Count());
        }
    }
}
