
using Infrastructure.AddressServices.Classes;
using NUnit.Framework;

namespace Infrastructure.AddressServices.Tests
{
    [TestFixture]
    public class AddressLaunderingTests
    {
        [Test]
        public void LaunderAddress_SplittetStreetName_Good()
        {
            //Arrange
            Address address = new Address { Street = "Ny Adelgade", StreetNr = "10", ZipCode = "1104" };
            //Act
            Address result = AddressLaundering.LaunderAddress(address);
            //Assert
            Assert.AreEqual(address, result);
        }

        [Test]
        public void LaunderAddress_SplittetStreetName_BadStreetName()
        {
            //Arrange
            Address address = new Address { Street = "Nü Adelgaje", StreetNr = "10", ZipCode = "1104" };
            Address correctAddress = new Address { Street = "Ny Adelgade", StreetNr = "10", ZipCode = "1104" };
            //Act
            Address result = AddressLaundering.LaunderAddress(address);
            //Assert
            Assert.AreEqual(address, result);
        }

        #region Exception tests

        [Test]
        public void LaunderAddress_ThrowException_E700_BadStreetNr()
        {
            //Arrange
            Address address = new Address { Street = "Ny Adelgade", StreetNr = "999999", ZipCode = "1104" };
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => AddressLaundering.LaunderAddress(address), "Husnummer eksisterer ikke på vejen");
        }

        [Test]
        public void LaunderAddress_ThrowException_E800_BadStreet()
        {
            //Arrange
            Address address = new Address { Street = "Ny VejNavn Test Hans", StreetNr = "10", ZipCode = "1104" };
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => AddressLaundering.LaunderAddress(address), "Vejnavn findes ikke indenfor postdistriktet");
        }

        [Test]
        public void LaunderAddress_ThrowException_E900_BadZipCode()
        {
            //Arrange
            Address address = new Address { Street = "Ny Adelgade", StreetNr = "10", ZipCode = "99999" };
            //Act

            //Assert
            Assert.Throws(typeof(AddressLaunderingException),
                () => AddressLaundering.LaunderAddress(address), "Postnummer eksisterer ikke");
        }

        #endregion
    }
}
