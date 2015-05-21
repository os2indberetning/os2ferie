using Core.DomainServices.RoutingClasses;
using NUnit.Framework;
using Core.DomainModel;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class AddressLaunderingTests
    {
        #region Laundering tests

        [Test]
        public void LaunderAddress_SplittetStreetName_Good()
        {
            //Arrange
            Address address = new Address { StreetName = "Ny Adelgade", StreetNumber = "10", ZipCode = 1104 };
            AddressLaundering uut = new AddressLaundering();
            //Act
            Address result = uut.LaunderAddress(address);
            //Assert
            Assert.AreEqual(address, result);
        }

        #endregion

        #region Exception tests

        [Test]
        public void LaunderAddress_ThrowException_E700_BadStreetNr()
        {
            //Arrange
            Address address = new Address { StreetName = "Ny Adelgade", StreetNumber = "999999", ZipCode = 1104 };
            AddressLaundering uut = new AddressLaundering();
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => uut.LaunderAddress(address), "Husnummer eksisterer ikke på vejen");
        }

        [Test]
        public void LaunderAddress_ThrowException_E800_BadStreet()
        {
            //Arrange
            Address address = new Address { StreetName = "Ny VejNavn Test Hans", StreetNumber = "10", ZipCode = 1104 };
            AddressLaundering uut = new AddressLaundering();
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => uut.LaunderAddress(address), "Vejnavn findes ikke indenfor postdistriktet");
        }

        [Test]
        public void LaunderAddress_ThrowException_E900_BadZipCode()
        {
            //Arrange
            Address address = new Address { StreetName = "Ny Adelgade", StreetNumber = "10", ZipCode = 99999 };
            AddressLaundering uut = new AddressLaundering();
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => uut.LaunderAddress(address), "Postnummer eksisterer ikke");
        }

        #endregion
    }
}
