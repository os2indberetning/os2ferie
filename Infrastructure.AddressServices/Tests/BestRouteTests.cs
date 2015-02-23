using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using NUnit.Framework;
using Address = Core.DomainModel.Address;

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
