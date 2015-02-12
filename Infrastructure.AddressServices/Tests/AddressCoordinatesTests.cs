using Infrastructure.AddressServices.Classes;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class AddressCoordinatesTests
    {
        [Test]
        public void GetAddressCoordinates_GoodCoordinates()
        {
            //Arrange
            Address address = new Address { Street = "Katrinebjergvej", StreetNr = "90", ZipCode = "8200" };
            Coordinates correctCoord = new Coordinates
            {
                Longitude = "10.1906",
                Latitude = "56.1735",
                Type = Coordinates.CoordinatesType.Origin
            };

            //Act
            Coordinates result = AddressCoordinates.GetAddressCoordinates(address, Coordinates.CoordinatesType.Origin);

            //Assert
            Assert.IsTrue(correctCoord.Equals(result));
        }

        [Test]
        public void GetAddressCoordinates_BadAddress_ThrowException()
        {
            //Arrange
            Address address = new Address { Street = "bjergvej alle troll", StreetNr = "90", ZipCode = "8200" };
            //Act

            //Assert
            Assert.Throws(typeof (AddressCoordinatesException),
                () => AddressCoordinates.GetAddressCoordinates(address, Coordinates.CoordinatesType.Origin), "Errors in address, see inner exception.");
        }

    }
}
