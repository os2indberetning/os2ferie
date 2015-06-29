using System.Collections.Generic;
using Core.DomainModel;
using Infrastructure.AddressServices.Routing;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void DistanceBetweenTwoAddresses_ReturnsFourteenPointSixKm()
        {
            var addresses = new List<Address>();

            var first = new Address()
            {
                StreetName = "Hans Bojes Alle",
                StreetNumber = "15",
                ZipCode = 8960,
                Town = "Randers SØ"
            };

            var second = new Address()
            {
                StreetName = "Vesselbjergvej",
                StreetNumber = "8",
                ZipCode = 8370,
                Town = "Hadsten"
            };

            var firstWithCoordinates = new AddressCoordinates().GetAddressCoordinates(first);
            var secondWithCoordinates = new AddressCoordinates().GetAddressCoordinates(second);

            addresses.Add(firstWithCoordinates);
            addresses.Add(secondWithCoordinates);

            var route = new BestRoute().GetRoute(DriveReportTransportType.Car,addresses);

            double result = (double)route.Length;

            Assert.That(result, Is.EqualTo(14.43).Within(2));
        }

        [Test]
        public void DistanceBetweenTwoAddresses_WithViaPoint_ReturnsThirtyOnePointFourKm()
        {
            var addresses = new List<Address>();

            var first = new Address()
            {
                StreetName = "Skovvej",
                StreetNumber = "20",
                ZipCode = 8382,
                Town = "Hinnerup"
            };

            var second = new Address()
            {
                StreetName = "Postvej",
                StreetNumber = "69",
                ZipCode = 8382,
                Town = "Hinnerup"
            };

            var third = new Address()
            {
                StreetName = "Danstrupvej",
                StreetNumber = "4",
                ZipCode = 8860,
                Town = "Ulstrup"
            };

            var firstWithCoordinates = new AddressCoordinates().GetAddressCoordinates(first);
            var secondWithCoordinates = new AddressCoordinates().GetAddressCoordinates(second);
            var thirdWithCoordinates = new AddressCoordinates().GetAddressCoordinates(third);

            addresses.Add(firstWithCoordinates);
            addresses.Add(secondWithCoordinates);
            addresses.Add(thirdWithCoordinates);

            var route = new BestRoute().GetRoute(DriveReportTransportType.Car,addresses);

            double result = (double)route.Length;

            Assert.That(result, Is.EqualTo(31.4).Within(2));
        }
    }
}