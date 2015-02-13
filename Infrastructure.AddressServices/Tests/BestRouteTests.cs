using System.Collections.Generic;
using Core.DomainServices;
using Infrastructure.AddressServices.Classes;
using Infrastructure.AddressServices.Routing;
using NUnit.Framework;
using Address = Core.DomainModel.Address;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class BestRouteTests
    {
        #region Setup
        private RouteInformation _result = new RouteInformation();

        [TestFixtureSetUp]
        public void BestRoute_CheckIfBoundariesObeyed_Setup()
        {
            //Arrange
            List<Address> addresses = new List<Address>()
            {
                new Address()
                {
                    StreetName = "Dalvej",
                    StreetNumber = "2",
                    ZipCode = 8450
                },
                new Address()
                {
                    StreetName = "Åskrænten",
                    StreetNumber = "5",
                    ZipCode = 8250
                }
            };
            IRoute<RouteInformation> bestRoute = new BestRoute();
            //Act
            _result = bestRoute.GetRoute(addresses);
        }

        #endregion

        #region Best route tests

        [Test]
        public void BestRoute_CheckIfDurationIs1570()
        {
            Assert.IsTrue(_result.Duration == 1569);
        }

        [Test]
        public void BestRoute_CheckIfDistanceIs30889()
        {
            Assert.IsTrue(_result.Length == 30886);
        }

        #endregion
    }
}
