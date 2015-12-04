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
        public void UpdateAddressHistories_WithOneActiveHistory_NoChangesInAddress_ShouldNotDeactivate()
        {
            var homeAddress = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress
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
                    Employment = emplRepoMock.AsQueryable().First(),
                    WorkAddress = workAddress,
                    HomeAddress = homeAddress,
                   
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               homeAddress
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithOneActiveHistory_ChangeInWorkAddress_ShouldDeactivate()
        {
            var homeAddress = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address =  new WorkAddress
                       {
                            StreetName = "NewTestStreetWork",
                            StreetNumber = "1",
                            ZipCode = 1234,
                            Town = "TestTown",
                            Latitude = "1234",
                            Longitude = "1234" 
                       }

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
                    Employment = emplRepoMock.AsQueryable().First(),
                    WorkAddress = workAddress,
                    HomeAddress = homeAddress,
                   
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               homeAddress
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithOneActiveHistory_ChangesInHomeAddress_ShouldDeactivate()
        {
            var homeAddress = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress
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
                    Employment = emplRepoMock.AsQueryable().First(),
                    WorkAddress = workAddress,
                    HomeAddress = homeAddress,
                   
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               new PersonalAddress
               {
                        StreetName = "NewTestStreetHome",
                        StreetNumber = "1",
                        ZipCode = 1234,
                        Town = "TestTown",
                        Latitude = "1234",
                        Longitude = "1234",
                        Type = PersonalAddressType.Home,
                        PersonId = 1
               }
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithOneActiveHistory_ChangesInBothAddresses_ShouldDeactivate()
        {
            var homeAddress = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address =  new WorkAddress
                       {
                            StreetName = "NewTestStreetWork",
                            StreetNumber = "1",
                            ZipCode = 1234,
                            Town = "TestTown",
                            Latitude = "1234",
                            Longitude = "1234" 
                       }
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
                    Employment = emplRepoMock.AsQueryable().First(),
                    WorkAddress = workAddress,
                    HomeAddress = homeAddress,
                   
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               new PersonalAddress
               {
                        StreetName = "NewTestStreetHome",
                        StreetNumber = "1",
                        ZipCode = 1234,
                        Town = "TestTown",
                        Latitude = "1234",
                        Longitude = "1234",
                        Type = PersonalAddressType.Home,
                        PersonId = 1
               }
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithTwoActiveHistories_NoChanges_ShouldNotDeactivate()
        {
            var homeAddress1 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress1 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var homeAddress2 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 2
            };

            var workAddress2 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress2
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
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 1),
                    WorkAddress = workAddress1,
                    HomeAddress = homeAddress1,
                },
                 new AddressHistory()
                {
                    EmploymentId = 2,
                    EndTimestamp = 0,
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 2),
                    WorkAddress = workAddress2,
                    HomeAddress = homeAddress2,
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               homeAddress1, homeAddress2
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithTwoActiveHistories_ChangesInOneWorkAddress_ShouldDeactivateOneHistory()
        {
            var homeAddress1 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress1 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var homeAddress2 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 2
            };

            var workAddress2 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                       Address = new WorkAddress
                       {
                            StreetName = "NewTestStreetWork",
                            StreetNumber = "1",
                            ZipCode = 1234,
                            Town = "TestTown",
                            Latitude = "1234",
                            Longitude = "1234"
                       }
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
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 1),
                    WorkAddress = workAddress1,
                    HomeAddress = homeAddress1,
                },
                 new AddressHistory()
                {
                    EmploymentId = 2,
                    EndTimestamp = 0,
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 2),
                    WorkAddress = workAddress2,
                    HomeAddress = homeAddress2,
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               homeAddress1, homeAddress2
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreNotEqual(0, historyList.ElementAt(1).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithTwoActiveHistories_ChangeInOneHomeAddress_ShouldDeactivateOneHistory()
        {
            var homeAddress1 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress1 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var homeAddress2 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 2
            };

            var workAddress2 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress2
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
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 1),
                    WorkAddress = workAddress1,
                    HomeAddress = homeAddress1,
                },
                 new AddressHistory()
                {
                    EmploymentId = 2,
                    EndTimestamp = 0,
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 2),
                    WorkAddress = workAddress2,
                    HomeAddress = homeAddress2,
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               new PersonalAddress
               {
                  StreetName = "NewTestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1   
               }, homeAddress2
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreEqual(0, historyList.ElementAt(1).EndTimestamp);

        }

        [Test]
        public void UpdateAddressHistories_WithTwoActiveHistories_ChangesInOneHomeAddressOneWorkAddress_ShouldDeactivateBothHistories()
        {
            var homeAddress1 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1
            };

            var workAddress1 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var homeAddress2 = new PersonalAddress
            {
                StreetName = "TestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 2
            };

            var workAddress2 = new WorkAddress
            {
                StreetName = "TestStreetWork",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234"
            };

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>
            {
                new Employment()
                {
                    Id = 1,
                    PersonId = 1,
                    OrgUnit = new OrgUnit()
                    {
                       Address = workAddress1
                    }
                },
                new Employment()
                {
                    Id = 2,
                    PersonId = 2,
                    OrgUnit = new OrgUnit()
                    {
                       Address = new WorkAddress
                       {
                            StreetName = "NewTestStreetWork",
                            StreetNumber = "1",
                            ZipCode = 1234,
                            Town = "TestTown",
                            Latitude = "1234",
                            Longitude = "1234"
                       }
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
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 1),
                    WorkAddress = workAddress1,
                    HomeAddress = homeAddress1,
                },
                 new AddressHistory()
                {
                    EmploymentId = 2,
                    EndTimestamp = 0,
                    Employment = emplRepoMock.AsQueryable().Single(x => x.Id == 2),
                    WorkAddress = workAddress2,
                    HomeAddress = homeAddress2,
                }
            };

            var personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            personalAddressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
               new PersonalAddress
               {
                  StreetName = "NewTestStreetHome",
                StreetNumber = "1",
                ZipCode = 1234,
                Town = "TestTown",
                Latitude = "1234",
                Longitude = "1234",
                Type = PersonalAddressType.Home,
                PersonId = 1   
               }, homeAddress2
            }.AsQueryable());

            addressHistoryRepoMock.AsQueryable().ReturnsForAnyArgs(historyList.AsQueryable());

            var uut = new AddressHistoryService(emplRepoMock, addressHistoryRepoMock, personalAddressRepoMock);

            uut.UpdateAddressHistories();
            Assert.AreNotEqual(0, historyList.ElementAt(0).EndTimestamp);
            Assert.AreNotEqual(0, historyList.ElementAt(1).EndTimestamp);

        }

    }
}
