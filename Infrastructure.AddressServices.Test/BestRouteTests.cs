using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
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
            _result = bestRoute.GetRoute(DriveReportTransportType.Car,addresses);
        }

        #endregion

        #region Best route tests

        [Test]
        public void BestRoute_CheckIfDurationIs1595()
        {
            Assert.That(_result.Duration, Is.EqualTo(1595));
            //Assert.IsTrue(result.Duration == 1595);
        }

        [Test]
        public void BestRoute_CheckIfDistanceIs30882()
        {
            Assert.That(_result.Length, Is.EqualTo(30.88).Within(1));
            //Assert.IsTrue(result.Length == 30886);
        }

        #endregion
    }
}
