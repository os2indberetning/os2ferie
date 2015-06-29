using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.PersonService
{
    [TestFixture]
    public class PersonServiceTest
    {
        private IQueryable<Person> _persons;
        private IRoute<RouteInformation> _routeMock;
        private IAddressCoordinates _coordinatesMock;
        private IGenericRepository<PersonalAddress> _addressRepoMock;
        private IPersonService _uut;
            
        [SetUp]
        public void SetUp()
        {
            _persons = new List<Person>{
                new Person
                {
                    Id = 1,
                    FirstName = "Morten",
                    LastName = "Rasmussen",
                    CprNumber = "1234567890",
                    Initials = "MR"
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Morten",
                    LastName = "Jørgensen",
                    CprNumber = "0987654321",
                    Initials = "MJ"
                },
                new Person
                {
                    Id = 3,
                    FirstName = "Jacob",
                    LastName = "Jensen",
                    CprNumber = "456456456",
                    Initials = "JOJ"
                }
            }.AsQueryable();

            _routeMock = NSubstitute.Substitute.For<IRoute<RouteInformation>>();
            _routeMock.GetRoute(DriveReportTransportType.Car, new List<Address>()).ReturnsForAnyArgs(new RouteInformation());
            _addressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _coordinatesMock.GetAddressCoordinates(new Address()).ReturnsForAnyArgs(new Address
            {
                Latitude = "1",
                Longitude = "1"
            });
            _uut = new Core.ApplicationServices.PersonService(_addressRepoMock, _routeMock, _coordinatesMock);
        }



        [Test]
        public void ScrubCprsShouldRemoveCprNumbers()
        {

            var uut = new Core.ApplicationServices.PersonService(_addressRepoMock, _routeMock, _coordinatesMock);

            foreach (var person in _persons)
            {
                Assert.AreNotEqual("", person.CprNumber, "Person should have a CPR number before scrubbing");
            }
            _persons = uut.ScrubCprFromPersons(_persons);
            foreach (var person in _persons)
            {
                Assert.AreEqual("", person.CprNumber, "Person should not have a CPR number before scrubbing");
            }
        }

        [Test]
        public void GetHomeAddress_NoAlternative_ShouldReturnActualHomeAddress()
        {
            var testPerson = new Person
            {
                Id = 1
            };

            _addressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress
                {
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    Person = testPerson,
                    Latitude = "1",
                    Longitude = "2",
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93B",
                    ZipCode = 8200,
                    Town = "Aarhus N"
                }
            }.AsQueryable());

            var uut = new Core.ApplicationServices.PersonService(_addressRepoMock, _routeMock, _coordinatesMock);
            var res = uut.GetHomeAddress(testPerson);
            Assert.AreEqual(PersonalAddressType.Home, res.Type);
            Assert.AreEqual("Katrinebjergvej", res.StreetName);
            Assert.AreEqual("93B", res.StreetNumber);
            Assert.AreEqual(8200, res.ZipCode);
            Assert.AreEqual("Aarhus N", res.Town);
            _coordinatesMock.DidNotReceiveWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void GetHomeAddress_WithAlternative_ShouldReturnAlternativeHomeAddress()
        {
            var testPerson = new Person
            {
                Id = 1
            };

            _addressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress
                {
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    Person = testPerson,
                    Latitude = "1",
                    Longitude = "2",
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93B",
                    ZipCode = 8200,
                    Town = "Aarhus N"
                },
                new PersonalAddress
                {
                    Type = PersonalAddressType.AlternativeHome,
                    PersonId = 1,
                    Person = testPerson,
                    Latitude = "1",
                    Longitude = "2",
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "44",
                    ZipCode = 8210,
                    Town = "Aarhus V"
                }
            }.AsQueryable());

            var uut = new Core.ApplicationServices.PersonService(_addressRepoMock, _routeMock, _coordinatesMock);
            var res = uut.GetHomeAddress(testPerson);
            Assert.AreEqual(PersonalAddressType.AlternativeHome, res.Type);
            Assert.AreEqual("Jens Baggesens Vej", res.StreetName);
            Assert.AreEqual("44", res.StreetNumber);
            Assert.AreEqual(8210, res.ZipCode);
            Assert.AreEqual("Aarhus V", res.Town);
            _coordinatesMock.DidNotReceiveWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void GetHomeAddress_WithAlternative_WithNoCoords_ShouldReturnAlternativeHomeAddress_AndCallCoordinates()
        {
            var testPerson = new Person
            {
                Id = 1
            };

            _addressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress
                {
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    Person = testPerson,
                    Latitude = "1",
                    Longitude = "2",
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93B",
                    ZipCode = 8200,
                    Town = "Aarhus N"
                },
                new PersonalAddress
                {
                    Type = PersonalAddressType.AlternativeHome,
                    PersonId = 1,
                    Person = testPerson,
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "44",
                    ZipCode = 8210,
                    Town = "Aarhus V"
                }
            }.AsQueryable());

            var uut = new Core.ApplicationServices.PersonService(_addressRepoMock, _routeMock, _coordinatesMock);
            var res = uut.GetHomeAddress(testPerson);
            Assert.AreEqual(PersonalAddressType.AlternativeHome, res.Type);
            Assert.AreEqual("Jens Baggesens Vej", res.StreetName);
            Assert.AreEqual("44", res.StreetNumber);
            Assert.AreEqual(8210, res.ZipCode);
            Assert.AreEqual("Aarhus V", res.Town);
            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void GetHomeAddress_WithActual_WithNoCoords_ShouldReturnActualHomeAddress_AndCallCoordinates()
        {
            var testPerson = new Person
            {
                Id = 1
            };

            _addressRepoMock.AsQueryable().ReturnsForAnyArgs(new List<PersonalAddress>
            {
                new PersonalAddress
                {
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    Person = testPerson,
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93B",
                    ZipCode = 8200,
                    Town = "Aarhus N"
                }
            }.AsQueryable());


            var res = _uut.GetHomeAddress(testPerson);
            Assert.AreEqual(PersonalAddressType.Home, res.Type);
            Assert.AreEqual("Katrinebjergvej", res.StreetName);
            Assert.AreEqual("93B", res.StreetNumber);
            Assert.AreEqual(8200, res.ZipCode);
            Assert.AreEqual("Aarhus N", res.Town);
            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void PersonWith_WorkDistanceOverrideSet_ShouldReturn_WorkDistanceOverride()
        {
            var testPerson = new Person
            {
                PersonalAddresses = new List<PersonalAddress>
                {
                    new PersonalAddress
                    {
                        Type = PersonalAddressType.Home
                    },
                },
                Employments = new List<Employment>
                {
                    new Employment
                    {
                        WorkDistanceOverride = 1
                    }
                }
            };

            _uut.AddHomeWorkDistanceToEmployments(testPerson);
            Assert.AreEqual(1, testPerson.Employments.ElementAt(0).HomeWorkDistance);
        }

        [Test]
        public void PersonWithout_WorkDistanceOverrideSet_ActualWorkAddress_ShouldCall_GetRoute()
        {
            var testPerson = new Person
            {
                PersonalAddresses = new List<PersonalAddress>
                {
                    new PersonalAddress
                    {
                        Type = PersonalAddressType.Home
                    },
                },
                Employments = new List<Employment>
                {
                    new Employment
                    {
                        OrgUnit = new OrgUnit()
                        {
                            Address = new WorkAddress()
                            {
                                StreetName = "Katrinebjergvej",
                                StreetNumber = "93B",
                                ZipCode = 8200,
                                Town = "Aarhus N"
                            }
                        }
                    }
                }
            };

            _uut.AddHomeWorkDistanceToEmployments(testPerson);
            _routeMock.ReceivedWithAnyArgs().GetRoute(DriveReportTransportType.Car, new List<Address>());
        }

        [Test]
        public void PersonWithout_WorkDistanceOverrideSet_AlternativeWorkAddress_ShouldCall_GetRoute()
        {
            var testPerson = new Person
            {
                PersonalAddresses = new List<PersonalAddress>
                {
                    new PersonalAddress
                    {
                        Type = PersonalAddressType.Home
                    },
                },
                Employments = new List<Employment>
                {
                    new Employment
                    {
                        OrgUnit = new OrgUnit()
                        {
                            Address = new WorkAddress()
                            {
                                StreetName = "Katrinebjergvej",
                                StreetNumber = "93B",
                                ZipCode = 8200,
                                Town = "Aarhus N"
                            }
                        },
                        AlternativeWorkAddress = new PersonalAddress
                        {
                            Type = PersonalAddressType.AlternativeWork
                        }
                    }
                }
            };

            _uut.AddHomeWorkDistanceToEmployments(testPerson);
            _routeMock.ReceivedWithAnyArgs().GetRoute(DriveReportTransportType.Car, new List<Address>());
        }
    }
}
