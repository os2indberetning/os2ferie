using System.Collections.Generic;
using Infrastructure.AddressServices.Classes;
using Infrastructure.AddressServices.Routing;
using NUnit.Framework;
using Address = Core.DomainModel.Address;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class SeptimaRouteTests
    {
        #region Test setups

        private List<RouteInformation> _singleRoute = new List<RouteInformation>();
        private List<RouteInformation> _viaRoute = new List<RouteInformation>();

        [TestFixtureSetUp]
        public void GetRoute_FoundRoute()
        {
            //Arrange
            List<Coordinates> testCoords = new List<Coordinates>
            {
                new Coordinates()
                {
                    Longitude = "9.8607",
                    Latitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Origin
                },
                new Coordinates()
                {
                    Longitude = "10.4446",
                    Latitude = "56.2921",
                    Type = Coordinates.CoordinatesType.Destination
                }
            };
            SeptimaRouter uut = new SeptimaRouter();

            _singleRoute = uut.GetRoute(testCoords) as List<RouteInformation>;
        }

        [TestFixtureSetUp]
        public void GetRoute_ViaPoint_FoundRoute()
        {
            //Arrange
            List<Coordinates> testCoords = new List<Coordinates>
            {
                new Coordinates()
                {
                    Longitude = "9.8607",
                    Latitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Origin
                },
                new Coordinates()
                {
                    Longitude = "10.4446",
                    Latitude = "56.2921",
                    Type = Coordinates.CoordinatesType.Destination
                },
                new Coordinates()
                {
                    Longitude = "10.1906",
                    Latitude = "56.1735",
                    Type = Coordinates.CoordinatesType.Via
                }
            };
            SeptimaRouter uut = new SeptimaRouter();

            _viaRoute = uut.GetRoute(testCoords) as List<RouteInformation>;
        }

#endregion

        #region Single route tests

        [Test]
        public void CheckRoute_ResultNotEmpty()
        {
            Assert.IsNotEmpty(_singleRoute);
        }

        [Test]
        public void CheckRoute_FirsResultHasGeoPoints()
        {
            Assert.IsTrue(_singleRoute[0].GeoPoints.Length > 0);
        }

        [Test]
        public void CheckRoute_FirstResultLengthIs45215()
        {
            Assert.IsTrue(45215 == _singleRoute[0].Length);
        }

        [Test]
        public void CheckRoute_FirstResultDurationIs2281()
        {
            Assert.IsTrue(2281 == _singleRoute[0].Duration);
        }

        [Test]
        public void CheckRoute_FirstResultStartStreetIsDalvej()
        {
            Assert.AreEqual(_singleRoute[0].StartStreet, "Dalvej");
        }

        [Test]
        public void CheckRoute_FirstResultEndStreetIsKlydevej()
        {
            Assert.AreEqual(_singleRoute[0].EndStreet, "Klydevej");
        }

        [Test]
        public void CheckRoute_ResultHasAlternativeRoutes()
        {
            Assert.IsTrue(_singleRoute.Count > 1);
        }

        #endregion

        #region Via route tests

        [Test]
        public void CheckRoute_ViaRoute_ResultNotEmpty()
        {
            Assert.IsNotEmpty(_viaRoute);
        }

        [Test]
        public void CheckRoute_ViaRoute_FirsResultHasGeoPoints()
        {
            Assert.IsTrue(_viaRoute[0].GeoPoints.Length > 0);
        }

        [Test]
        public void CheckRoute_ViaRoute_FirstResultLengthIs52910()
        {
            Assert.IsTrue(52910 + 10 >= _viaRoute[0].Length && 52910-10 <= _viaRoute[0].Length); //10 meters +/-
        }

        [Test]
        public void CheckRoute_ViaRoute_FirstResultDurationIs2901()
        {
            Assert.IsTrue(2901 == _viaRoute[0].Duration);
            Assert.IsTrue(2901 + 5 >= _viaRoute[0].Duration && 2901 - 5 <= _viaRoute[0].Duration); //5 seconds +/-
        }

        [Test]
        public void CheckRoute_ViaRoute_FirstResultStartStreetIsDalvej()
        {
            Assert.AreEqual(_viaRoute[0].StartStreet, "Dalvej");
        }

        [Test]
        public void CheckRoute_ViaRoute_FirstResultEndStreetIsKlydevej()
        {
            Assert.AreEqual(_viaRoute[0].EndStreet, "Klydevej");
        }

        #endregion

        #region Exception tests

        [Test]
        public void GetRoute_NoRouteException()
        {
            //Arrange
            List<Coordinates> testCoords = new List<Coordinates>
            {
                new Coordinates()
                {
                    Latitude = "9.8607",
                    Longitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Origin
                },
                new Coordinates()
                {
                    Latitude = "9.8607",
                    Longitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Destination
                }
                
            };
            SeptimaRouter uut = new SeptimaRouter();

            Assert.Throws(typeof(RouteInformationException),
                () => uut.GetRoute(testCoords), "No route found.");
        }

        [Test]
        public void GetRoute_NoOriginOrDestinationEntry()
        {
            //Arrange
            List<Coordinates> testCoords = new List<Coordinates>
            {
                new Coordinates()
                {
                    Latitude = "10.1906",
                    Longitude = "56.1735",
                    Type = Coordinates.CoordinatesType.Via
                },
                new Coordinates()
                {
                    Latitude = "9.8607",
                    Longitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Via
                }
                
            };
            SeptimaRouter uut = new SeptimaRouter();

            Assert.Throws(typeof(RouteInformationException),
                () => uut.GetRoute(testCoords), "Coordinate of type Origin and/or Destination missing.");
        }

        [Test]
        public void GetRoute_DoubleOrigin()
        {
            //Arrange
            List<Coordinates> testCoords = new List<Coordinates>
            {
                new Coordinates()
                {
                    Latitude = "10.1906",
                    Longitude = "56.1735",
                    Type = Coordinates.CoordinatesType.Origin
                },
                new Coordinates()
                {
                    Latitude = "9.8607",
                    Longitude = "56.2564",
                    Type = Coordinates.CoordinatesType.Origin
                }
                
            };
            SeptimaRouter uut = new SeptimaRouter();

            Assert.Throws(typeof(RouteInformationException),
                () => uut.GetRoute(testCoords), "Mutltiple coordinates with type Origin and/or Destination.");
        }

        #endregion
    }
}
