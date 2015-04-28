using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Interfaces;
using NSubstitute;
using NUnit.Framework;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DomainServices.Test
{
    [TestFixture]
    public class CachedAddressLaundererTest
    {
        private CachedAddressLaunderer uut;
        private IAddressLaunderer laundryMock;
        private IGenericRepository<CachedAddress> repoMock;
        private IAddressCoordinates coordinatesMock;
        
        [SetUp]
        public void SetUp()
        {

            coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            repoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<CachedAddress>()
            {
                new CachedAddress()
                {
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "44",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    IsDirty = true
                },
                new CachedAddress()
                {
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93b",
                    ZipCode = 8200,
                    Town = "Aarhus N",
                    IsDirty = false
                }
            }.AsQueryable());

            laundryMock = NSubstitute.Substitute.For<IAddressLaunderer>();
            laundryMock.WhenForAnyArgs(x => x.Launder(new Address())).DoNotCallBase();

            uut = new CachedAddressLaunderer(repoMock, laundryMock, coordinatesMock);    
        }

        [Test]
        public void CleanAddress_Clean_ShouldNotBeCalled()
        {
            var testAddr = new Address()
            {
                StreetName = "Katrinebjergvej",
                StreetNumber = "93b",
                ZipCode = 8200,
                Town = "Aarhus N",
            };

            uut.Launder(testAddr);

            laundryMock.DidNotReceiveWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void DirtyAddress_CleanShouldBeCalled()
        {
            var testAddr = new Address()
            {
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V",
            };

            uut.Launder(testAddr);

            laundryMock.ReceivedWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void NonExistingAddress_Clean_ShouldBeCalled()
        {
            var testAddr = new Address()
            {
                StreetName = "Risdalsvej",
                StreetNumber = "48",
                ZipCode = 8260,
                Town = "Viby J",
            };

            uut.Launder(testAddr);

            repoMock.ReceivedWithAnyArgs().Insert(new CachedAddress());
            laundryMock.ReceivedWithAnyArgs().Launder(new Address());
        }
    }
}
