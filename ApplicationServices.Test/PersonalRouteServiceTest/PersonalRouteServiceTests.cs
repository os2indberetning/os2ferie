using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.PersonalRouteServiceTest
{
    [TestFixture]
    class PersonalRouteServiceTests
    {
        private IAddressCoordinates _coordinatesMock;
        private IGenericRepository<PersonalRoute> _routeRepoMock;
        private PersonalRouteService _uut;
        private Person _testPerson;
        private int _idCounter;
        [SetUp]
        public void SetUp()
        {
            _idCounter = 0;
            var routeList = new List<PersonalRoute>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _coordinatesMock.GetAddressCoordinates(new Address()).ReturnsForAnyArgs(new Point
            {
                Latitude = "1",
                Longitude = "1"
            });


            _routeRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalRoute>>();
            _routeRepoMock.Insert(new PersonalRoute()).ReturnsForAnyArgs(x => x.Arg<PersonalRoute>()).AndDoes(x => routeList.Add(x.Arg<PersonalRoute>())).AndDoes(x => x.Arg<PersonalRoute>().Id = _idCounter).AndDoes(x => _idCounter++);
            _routeRepoMock.AsQueryable().ReturnsForAnyArgs(routeList.AsQueryable());

            _uut = new PersonalRouteService(_coordinatesMock, _routeRepoMock);
            _testPerson = new Person
            {
                Id = 1
            };
        }

        [Test]
        public void Create_ShouldCall_GetAddressCoordinates()
        {
            var testRoute = new PersonalRoute
            {
                Person = _testPerson,
                PersonId = _testPerson.Id,
                Points = new List<Point>
                {
                    new Point
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    },
                    new Point
                    {
                        StreetName = "Jens Baggesens Vej",
                        StreetNumber = "44",
                        ZipCode = 8210,
                        Town = "Aarhus V"
                    }
                }
            };

            _uut.Create(testRoute);
            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void Create_ShouldSet_CorrectPreviousAndNextId()
        {
            var testRoute = new PersonalRoute
            {
                Person = _testPerson,
                PersonId = _testPerson.Id,
                Points = new List<Point>
                {
                    new Point
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    },
                    new Point
                    {
                        StreetName = "Jens Baggesens Vej",
                        StreetNumber = "44",
                        ZipCode = 8210,
                        Town = "Aarhus V"
                    },
                    new Point
                    {
                        StreetName = "Jens Baggesens Vej",
                        StreetNumber = "46",
                        ZipCode = 8210,
                        Town = "Aarhus V"
                    }
                }
            };

            var res = _uut.Create(testRoute);
            Assert.AreEqual(res.Points.ElementAt(1).Id, res.Points.ElementAt(0).NextPointId);
            Assert.AreEqual(res.Points.ElementAt(0).Id, res.Points.ElementAt(1).PreviousPointId);
            Assert.AreEqual(res.Points.ElementAt(1).Id, res.Points.ElementAt(2).PreviousPointId);
            Assert.AreEqual(res.Points.ElementAt(2).Id, res.Points.ElementAt(1).NextPointId);
            Assert.AreEqual(0, res.Points.ElementAt(0).PreviousPointId);
            Assert.AreEqual(0, res.Points.ElementAt(2).NextPointId);
        }
    }
}
