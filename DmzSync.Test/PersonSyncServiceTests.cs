using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DmzSync.Encryption;
using Infrastructure.DmzSync.Services.Impl;
using Infrastructure.DmzSync.Services.Interface;
using NSubstitute;
using NUnit.Framework;
using Employment = Core.DomainModel.Employment;

namespace DmzSync.Test
{
    [TestFixture]
    public class PersonSyncServiceTests
    {

        private ISyncService _uut;
        private IGenericRepository<Person> _masterRepoMock;
        private IGenericRepository<Profile> _dmzRepoMock;
        private List<Profile> _dmzProfileList = new List<Profile>();
        private List<Person> _masterPersonList = new List<Person>();
        private IPersonService _personServiceMock;

        [SetUp]
        public void SetUp()
        {
            _dmzRepoMock = NSubstitute.Substitute.For<IGenericRepository<Profile>>();
            _masterRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _personServiceMock = NSubstitute.Substitute.For<IPersonService>();


            _dmzRepoMock.WhenForAnyArgs(x => x.Delete(new Profile())).Do(p => _dmzProfileList.Remove(p.Arg<Profile>()));

            _personServiceMock.GetHomeAddress(new Person()).ReturnsForAnyArgs(new PersonalAddress()
            {
                Latitude = "1",
                Longitude = "2"
            });

            _dmzRepoMock.WhenForAnyArgs(x => x.Insert(new Profile())).Do(t => _dmzProfileList.Add(t.Arg<Profile>()));

            _dmzProfileList = new List<Profile>();
            _masterPersonList = new List<Person>()
            {
                new Person()
                {
                    Id = 1,
                    IsActive = true,
                    FirstName = "Test",
                    LastName = "Testesen",
                    Initials = "TT",
                    FullName = "Test Testesen [TT]",
                    Employments = new List<Employment>()
                    {
                        new Employment()
                        {
                            Id = 1,
                            PersonId = 1,
                            Position = "Tester",
                            OrgUnit = new OrgUnit()
                            {
                                LongDescription = "IT Minds"
                            }
                        }
                    }
                },
                new Person()
                {
                    Id = 2,
                    FirstName = "Lars",
                    IsActive = true,
                    LastName = "Testesen",
                    Initials = "LT",
                    FullName = "Lars Testesen [LT]",
                    Employments = new List<Employment>()
                    {
                        new Employment()
                        {
                            Id = 1,
                            PersonId = 2,
                            Position = "Tester2",
                            OrgUnit = new OrgUnit()
                            {
                                LongDescription = "IT Minds"
                            }
                        }
                    }
                },
                new Person()
                {
                    Id = 3,
                   IsActive = true,
                    FirstName = "Preben",
                    LastName = "Testesen",
                    Initials = "PT",
                    FullName = "Preben Testesen [PT]",
                    Employments = new List<Employment>()
                    {
                        new Employment()
                        {
                            Id = 1,
                            PersonId = 3,
                            Position = "Tester3",
                            OrgUnit = new OrgUnit()
                            {
                                LongDescription = "IT Minds"
                            }
                        }
                    }
                }
            };


            _masterRepoMock.AsQueryable().ReturnsForAnyArgs(_masterPersonList.AsQueryable());
            _dmzRepoMock.AsQueryable().ReturnsForAnyArgs(_dmzProfileList.AsQueryable());

            _uut = new PersonSyncService(_dmzRepoMock, _masterRepoMock, _personServiceMock);
        }

        [Test]
        public void ClearDmz_ShouldCallDeleteRange()
        {
            _dmzProfileList.Add(new Profile());
            _dmzProfileList.Add(new Profile());
            _dmzProfileList.Add(new Profile());
            var numberOfReceivedCalls = 0;
            _dmzRepoMock.WhenForAnyArgs(x => x.DeleteRange(_dmzProfileList)).Do(p => numberOfReceivedCalls++);
            _uut.ClearDmz();
            Assert.AreEqual(1, numberOfReceivedCalls);
        }

        [Test]
        public void SyncFromDmz_ShouldThrow_NotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => _uut.SyncFromDmz());
        }

        [Test]
        public void SyncToDmz_ShouldCreateProfilesInDmz()
        {
            Assert.AreEqual(0, _dmzProfileList.Count);
            _uut.SyncToDmz();
            Assert.AreEqual(3, _dmzProfileList.Count);
        }

        [Test]
        public void SyncToDmz_ShouldSetEmploymentsCorrectly()
        {
            _uut.SyncToDmz();
            Assert.AreEqual(StringCipher.Encrypt("Tester - IT Minds", Encryptor.EncryptKey), _dmzProfileList.ElementAt(0).Employments.ElementAt(0).EmploymentPosition);
            Assert.AreEqual(StringCipher.Encrypt("Tester2 - IT Minds", Encryptor.EncryptKey), _dmzProfileList.ElementAt(1).Employments.ElementAt(0).EmploymentPosition);
            Assert.AreEqual(StringCipher.Encrypt("Tester3 - IT Minds", Encryptor.EncryptKey), _dmzProfileList.ElementAt(2).Employments.ElementAt(0).EmploymentPosition);
        }

        [Test]
        public void SyncToDmz_ShouldSetFullNameCorrectly()
        {
            _uut.SyncToDmz();
            Assert.AreEqual(StringCipher.Encrypt("Test Testesen [TT]", Encryptor.EncryptKey), _dmzProfileList.ElementAt(0).FullName);
            Assert.AreEqual(StringCipher.Encrypt("Lars Testesen [LT]", Encryptor.EncryptKey), _dmzProfileList.ElementAt(1).FullName);
            Assert.AreEqual(StringCipher.Encrypt("Preben Testesen [PT]", Encryptor.EncryptKey), _dmzProfileList.ElementAt(2).FullName);

        }

    }
}
