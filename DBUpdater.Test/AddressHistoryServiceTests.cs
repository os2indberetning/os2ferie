using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace DBUpdater.Test
{
    public class AddressHistoryServiceTests
    {
        [Test]
        public void CreateNoneExistingHistories_CalledWithOneNoneExistingEmpl_ShouldAddOneHistory()
        {

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address =  new WorkAddress()
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>();
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            Assert.IsEmpty(historyList);
            uut.CreateNonExistingHistories();
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
        }

        [Test]
        public void CreateNoneExistingHistories_CalledWithTwoNoneExistingEmpls_ShouldAddTwoHistories()
        {

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address = new WorkAddress()
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 2,
                        Address = new WorkAddress()
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>();
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                },
                new PersonalAddress()
                {
                    PersonId = 2,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            Assert.IsEmpty(historyList);
            uut.CreateNonExistingHistories();
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(1).EmploymentId);
        }

        [Test]
        public void CreateNoneExistingHistories_CalledWithOneNoneExistingAndOneExisting_ShouldOneTwoHistory()
        {

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address = new WorkAddress()
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 2
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>();
            historyList.Add(new AddressHistory()
            {
                EmploymentId = 2,
                EndTimestamp = 0,
            });
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                },
                new PersonalAddress()
                {
                    PersonId = 2,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.CreateNonExistingHistories();
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(2, historyList.Count);
        }

        [Test]
        public void CreateNoneExistingHistories_CalledWithOneNoneExistingButNoHomeAddress_ShouldAddNothing()
        {

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 2
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>();
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress> { }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            Assert.AreEqual(0, historyList.Count);
            uut.CreateNonExistingHistories();
            Assert.AreEqual(0, historyList.Count);
        }

        [Test]
        public void CreateNoneExistingHistories_CalledWithEmplThatHasInactiveHistory_ShouldAddOneHistory()
        {

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address = new WorkAddress()
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    EndTimestamp = 12,
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.CreateNonExistingHistories();
            Assert.AreEqual(12, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(1).EmploymentId);
        }

        [Test]
        public void UpdateAddressHistories_WithNoChanges_ShouldNotAlterAnyHistories()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    EndTimestamp = 0,
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            uut.UpdateAddressHistories();
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
        }

        [Test]
        public void UpdateAddressHistories_WithOneChangeInPersonalAddress_ShouldAlterOldHistory_AndAddNewOne()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address = new WorkAddress()
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        PersonId = 1
                    },
                    HomeAddressId = 1,
                    EndTimestamp = 0,
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            var personalAddressList = new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.OldHome,
                    Id = 1
                },
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home,
                    Id = 2
                }
            };
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(personalAddressList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.Count);

            uut.AddHomeAddress(new PersonalAddress()
            {
                PersonId = 1,
                Id = 2
            });

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).HomeAddressId);

            Assert.AreEqual(1, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(1).HomeAddressId);
            Assert.AreEqual(2, historyList.Count);
        }

        [Test]
        public void UpdateAddressHistories_WithOneChangeInWorkAddress_ShouldAlterOldHistory_AndAddNewOne()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnitId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 12,
                        Address = new WorkAddress(),
                        Id = 1
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    EndTimestamp = 0,
                    WorkAddressId = 1
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            uut.AddWorkAddress(new WorkAddress()
            {
                OrgUnitId = 1,
                Id = 12
            });

            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);
            Assert.AreEqual(1, historyList.Count);

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();

            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);

            Assert.AreEqual(1, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(12, historyList.ElementAt(1).WorkAddressId);
            Assert.AreEqual(2, historyList.Count);
        }

        [Test]
        public void UpdateAddressHistories_WithOneChangeInPersonalAddress_AndLotsOfOldHistories_ShouldAlterOnlyActiveHistory_AndAddNewHistory()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnitId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 12,
                        Address = new WorkAddress(),
                        Id = 1
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 30,
                    WorkAddressId = 1
                },
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 30,
                    WorkAddressId = 1
                },
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 30,
                    WorkAddressId = 1
                },new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 30,
                    WorkAddressId = 1
                },
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 30,
                    WorkAddressId = 1
                },
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    StartTimestamp = 20,
                    EndTimestamp = 0,
                    WorkAddressId = 1
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.OldHome,
                    Id = 1
                },
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home,
                    Id = 2
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            uut.AddWorkAddress(new WorkAddress()
            {
                OrgUnitId = 1,
                Id = 12
            });

            uut.AddHomeAddress(new PersonalAddress()
            {
                PersonId = 1,
                Id = 2
            });

            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(30, historyList.ElementAt(i).EndTimestamp);
            }

            Assert.AreEqual(6, historyList.Count);
            Assert.AreEqual(0, historyList.ElementAt(5).EndTimestamp);

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();

            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(30, historyList.ElementAt(i).EndTimestamp);
            }

            Assert.AreNotEqual(0, historyList.ElementAt(5).EndTimestamp);
            Assert.AreEqual(0, historyList.ElementAt(6).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(6).HomeAddressId);
        }

        [Test]
        public void UpdateAddressHistories_WithTwoChangesInPersonalAddress_ShouldAlterOldHistory_AndAddNewOneForEachEmpl()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 1,
                        Address = new WorkAddress()
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 2,
                        Address = new WorkAddress()
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        PersonId = 1
                    },
                    HomeAddressId = 1,
                    EndTimestamp = 0,
                },
                new AddressHistory()
                {
                    EmploymentId = 2,
                    Employment = new Employment()
                    {
                        PersonId = 2
                    },
                    HomeAddressId = 3,
                    EndTimestamp = 0,
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            var personalAddressList = new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.OldHome,
                    Id = 1
                },
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home,
                    Id = 2
                },
                new PersonalAddress()
                {
                    PersonId = 2,
                    Type = PersonalAddressType.OldHome,
                    Id = 3
                },
                new PersonalAddress()
                {
                    PersonId = 2,
                    Type = PersonalAddressType.Home,
                    Id = 4
                }
            };
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(personalAddressList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(2, historyList.Count);

            uut.AddHomeAddress(new PersonalAddress()
            {
                PersonId = 1,
                Id = 2
            });

            uut.AddHomeAddress(new PersonalAddress()
            {
                PersonId = 2,
                Id = 4
            });

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();
            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).HomeAddressId);

            Assert.AreEqual(2, historyList.ElementAt(1).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(3, historyList.ElementAt(1).HomeAddressId);

            Assert.AreEqual(1, historyList.ElementAt(2).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(2).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(2).HomeAddressId);

            Assert.AreEqual(2, historyList.ElementAt(3).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(3).EndTimestamp);
            Assert.AreEqual(4, historyList.ElementAt(3).HomeAddressId);

            Assert.AreEqual(4, historyList.Count);
        }

        [Test]
        public void UpdateAddressHistories_WithTwoChangesInWorkAddresses_ShouldAlterOldHistory_AndAddNewOneForEachEmpl()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnitId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 12,
                        Address = new WorkAddress(),
                        Id = 1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnitId = 2,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 13,
                        Address = new WorkAddress(),
                        Id = 2
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1
                    },
                    EndTimestamp = 0,
                    WorkAddressId = 1
                },
                new AddressHistory()
                {
                    EmploymentId = 2,
                    Employment = new Employment()
                    {
                        OrgUnitId = 2
                    },
                    EndTimestamp = 0,
                    WorkAddressId = 2
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                },
                 new PersonalAddress()
                {
                    PersonId = 2,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            uut.AddWorkAddress(new WorkAddress()
            {
                OrgUnitId = 1,
                Id = 12
            });
            uut.AddWorkAddress(new WorkAddress()
            {
                OrgUnitId = 2,
                Id = 13
            });

            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);

            Assert.AreEqual(2, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(1).WorkAddressId);

            Assert.AreEqual(2, historyList.Count);

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();

            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);

            Assert.AreEqual(2, historyList.ElementAt(1).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(2, historyList.ElementAt(1).WorkAddressId);

            Assert.AreEqual(1, historyList.ElementAt(2).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(2).EndTimestamp);
            Assert.AreEqual(12, historyList.ElementAt(2).WorkAddressId);

            Assert.AreEqual(2, historyList.ElementAt(3).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(3).EndTimestamp);
            Assert.AreEqual(13, historyList.ElementAt(3).WorkAddressId);


            Assert.AreEqual(4, historyList.Count);
        }

        [Test]
        public void UpdateAddressHistories_WithChangeInPersonalAndWorkAddress_ShouldAlterOldHistory_AndAddNewOne()
        {
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnitId = 1,
                    OrgUnit = new OrgUnit()
                    {
                        AddressId = 13,
                        Address = new WorkAddress(),
                        Id = 1
                    }
                }
            }.AsQueryable());

            var addressHistoryRepoMock = NSubstitute.Substitute.For<IGenericRepository<AddressHistory>>();
            var historyList = new List<AddressHistory>
            {
                new AddressHistory()
                {
                    EmploymentId = 1,
                    Employment = new Employment()
                    {
                        OrgUnitId = 1,
                        PersonId = 1
                    },
                    EndTimestamp = 0,
                    WorkAddressId = 1,
                    HomeAddressId = 10
                }
            };
            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());
            addressHistoryRepoMock.Insert(new AddressHistory())
                .ReturnsForAnyArgs(x => x.Arg<AddressHistory>())
                .AndDoes(x => historyList.Add(x.Arg<AddressHistory>()));

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress()
                {
                    PersonId = 1,
                    Id = 10,
                    Type = PersonalAddressType.OldHome
                },
                 new PersonalAddress()
                {
                    Id = 20,
                    PersonId = 1,
                    Type = PersonalAddressType.Home
                }
            }.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);
            uut.AddHomeAddress(new PersonalAddress()
            {
                PersonId = 1,
                Id = 2
            });
            uut.AddWorkAddress(new WorkAddress()
            {
                OrgUnitId = 1,
                Id = 13
            });

            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);
            Assert.AreEqual(10, historyList.ElementAt(0).HomeAddressId);

            Assert.AreEqual(1, historyList.Count);

            uut.UpdateAddressHistories();
            uut.CreateNonExistingHistories();


            Assert.AreEqual(1, historyList.ElementAt(0).EmploymentId);
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(1, historyList.ElementAt(0).WorkAddressId);
            Assert.AreEqual(10, historyList.ElementAt(0).HomeAddressId);

            Assert.AreEqual(1, historyList.ElementAt(1).EmploymentId);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);
            Assert.AreEqual(13, historyList.ElementAt(1).WorkAddressId);
            Assert.AreEqual(20, historyList.ElementAt(1).HomeAddressId);
            Assert.AreEqual(2, historyList.Count);
        }


    }
}
