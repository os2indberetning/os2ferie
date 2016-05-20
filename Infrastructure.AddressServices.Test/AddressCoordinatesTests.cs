using Core.DomainModel;
using Core.DomainServices.RoutingClasses;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Test
{
    [TestFixture]
    public class AddressCoordinatesTests
    {
        #region Coordinate tests

        [Test]
        public void GetCoordinates_GoodCoordinates()
        {
            //Arrange
            Address address = new Address { StreetName = "Katrinebjergvej", StreetNumber = "90", ZipCode = 8200 };
            AddressCoordinates uut = new AddressCoordinates();
            Coordinates correctCoord = new Coordinates
            {
                Longitude = "10.1906",
                Latitude = "56.1735",
                Type = Coordinates.CoordinatesType.Origin
            };

            //Act
            Coordinates result = uut.GetCoordinates(address, Coordinates.CoordinatesType.Origin);

            //Assert
            Assert.IsTrue(correctCoord.Equals(result));
        }

        [Test]
        public void GetAddressCoordinatesSecond_GoodCoordinates()
        {
            //Arrange
            Address address = new Address { StreetName = "Katrinebjergvej", StreetNumber = "90", ZipCode = 8200 };
            AddressCoordinates uut = new AddressCoordinates();
            Address correctCoord = new Address
            {
                StreetName = "Katrinebjergvej",
                StreetNumber = "90",
                ZipCode = 8200,
                Longitude = "10.1906964757185",
                Latitude = "56.1735754688399",

            };

            //Act
            Address result = uut.GetAddressCoordinates(address);

            //Assert
            Assert.IsTrue(correctCoord.Latitude == result.Latitude && correctCoord.Longitude == result.Longitude);
        }

        [Test]
        public void GetAddressFromCoordinates_GoodCoords()
        {
            //Arrange
            Address address = new Address { Longitude = "12.58514", Latitude = "55.68323" };
            AddressCoordinates uut = new AddressCoordinates();
            Address correctAddress = new Address
            {
                StreetName = "Landgreven",
                StreetNumber = "10",
                ZipCode = 1301,
                Town = "København K",
                Longitude = "12.58514",
                Latitude = "55.68323"

            };

            //Act
            Address result = uut.GetAddressFromCoordinates(address);

            //Assert
            Assert.AreEqual(correctAddress.StreetName, result.StreetName);
            Assert.AreEqual(correctAddress.StreetNumber, result.StreetNumber);
            Assert.AreEqual(correctAddress.ZipCode, result.ZipCode);
            Assert.AreEqual(correctAddress.Town, result.Town);
        }

        #endregion

        #region Exception tests

        [Test]
        public void GetCoordinates_BadAddress_ThrowException()
        {
            //Arrange
            Address address = new Address { StreetName = "Bjergvej Alle Troll", StreetNumber = "90", ZipCode = 8200 };
            AddressCoordinates uut = new AddressCoordinates();
            //Act

            //Assert
            Assert.Throws(typeof(AddressCoordinatesException),
                () => uut.GetCoordinates(address, Coordinates.CoordinatesType.Origin), "Errors in address, see inner exception.");
        }

        [Test]
        public void GetAddressFromCoordinates_BadCoords_ThrowException()
        {
            //Arrange
            Address address = new Address { Longitude = "999.00", Latitude = "999.00" };
            AddressCoordinates uut = new AddressCoordinates();

            //Act

            //Assert
            Assert.Throws(typeof (AddressCoordinatesException), () => uut.GetAddressFromCoordinates(address),
                "Server error, coordinates invalid");
        }

        #endregion
    }
}
