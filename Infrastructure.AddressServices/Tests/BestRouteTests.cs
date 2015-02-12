using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.AddressServices.Classes;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class BestRouteTests
    {
        #region Setup
        private RouteInformation result = new RouteInformation();

        [TestFixtureSetUp]
        public void BestRoute_CheckIfBoundariesObeyed_Setup()
        {
            //Arrange
            List<Address> addresses = new List<Address>()
            {
                new Address()
                {
                    Street = "Dalvej",
                    StreetNr = "2",
                    ZipCode = "8450",
                    Type = Coordinates.CoordinatesType.Origin
                },
                new Address()
                {
                    Street = "Åskrænten",
                    StreetNr = "5",
                    ZipCode = "8250",
                    Type = Coordinates.CoordinatesType.Destination
                }
            };
            IRoute bestRoute = new BestRoute();
            //Act
            result = bestRoute.GetRoute(addresses);
        }

        #endregion

        [Test]
        public void BestRoute_CheckIfDurationIs1570()
        {
            Assert.IsTrue(result.Duration == 1569);
        }

        [Test]
        public void BestRoute_CheckIfDistanceIs30889()
        {
            Assert.IsTrue(result.Length == 30886);
        }

    }
}
