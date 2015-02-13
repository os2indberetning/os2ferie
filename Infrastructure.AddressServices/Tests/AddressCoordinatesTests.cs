using Infrastructure.AddressServices.Classes;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class AddressCoordinatesTests
    {
        [Test]
        public void GetCoordinates_GoodCoordinates()
        {
            //Arrange
            Address address = new Address { StreetName = "Katrinebjergvej", StreetNumber = "90", ZipCode = "8200" };
            Coordinates correctCoord = new Coordinates
            {
                Longitude = "10.1906",
                Latitude = "56.1735",
                Type = Coordinates.CoordinatesType.Origin
            };

            //Act
            Coordinates result = AddressCoordinates.GetCoordinates(address, Coordinates.CoordinatesType.Origin);

            //Assert
            Assert.IsTrue(correctCoord.Equals(result));
        }

        [Test]
        public void GetCoordinates_BadAddress_ThrowException()
        {
            //Arrange
            Address address = new Address { StreetName = "Bjergvej Alle Troll", StreetNumber = "90", ZipCode = "8200" };
            //Act

            //Assert
            Assert.Throws(typeof(AddressCoordinatesException),
                () => AddressCoordinates.GetCoordinates(address, Coordinates.CoordinatesType.Origin), "Errors in address, see inner exception.");
        }

        [Test]
        public void GetAddressCoordinatesSecond_GoodCoordinates()
        {
            //Arrange
            Address address = new Address { StreetName = "Katrinebjergvej", StreetNumber = "90", ZipCode = "8200" };
            Address correctCoord = new Address
            {
                StreetName = "Katrinebjergvej",
                StreetNumber = "90",
                ZipCode = "8200",
                Longitude = "10.1906",
                Latitude = "56.1735",

            };

            //Act
            Address result = AddressCoordinates.GetAddressCoordinates(address);

            //Assert
            Assert.IsTrue(correctCoord.Latitude == result.Latitude && correctCoord.Longitude == result.Longitude);

        }

    }
}
