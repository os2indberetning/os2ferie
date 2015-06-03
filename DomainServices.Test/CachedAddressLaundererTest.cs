using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
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
        private CachedAddressLaunderer _uut;
        private IAddressLaunderer _laundryMock;
        private IGenericRepository<CachedAddress> _repoMock;
        private IAddressCoordinates _coordinatesMock;
        
        [SetUp]
        public void SetUp()
        {

            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _repoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            _repoMock.AsQueryable().ReturnsForAnyArgs(new List<CachedAddress>()
            {
                new CachedAddress()
                {
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "44",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    IsDirty = true,
                    DirtyString = "Jens Baggesens Vej 44, 8210 Aarhus V"
                },
                new CachedAddress()
                {
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "93b",
                    ZipCode = 8200,
                    Town = "Aarhus N",
                    IsDirty = false,
                    DirtyString = "Katrinebjergvej 93b, 8200 Aarhus N"
                }
            }.AsQueryable());

            _laundryMock = NSubstitute.Substitute.For<IAddressLaunderer>();
            _laundryMock.WhenForAnyArgs(x => x.Launder(new Address())).DoNotCallBase();



            _uut = new CachedAddressLaunderer(_repoMock, _laundryMock, _coordinatesMock);    
        }

        [Test]
        public void CleanAddress_Clean_ShouldNotBeCalled()
        {

            _laundryMock.Launder(new Address()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>());
            var testAddr = new Address()
            {
                StreetName = "Katrinebjergvej",
                StreetNumber = "93b",
                ZipCode = 8200,
                Town = "Aarhus N",
            };

            _uut.Launder(testAddr);

            _laundryMock.DidNotReceiveWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void DirtyAddress_CleanShouldBeCalled()
        {

            _laundryMock.Launder(new Address()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>());
            var testAddr = new Address()
            {
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V",
            };

            _uut.Launder(testAddr);

            _laundryMock.ReceivedWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void NonExistingAddress_Clean_ShouldBeCalled()
        {

            _laundryMock.Launder(new Address()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>());
            var testAddr = new Address()
            {
                StreetName = "Risdalsvej",
                StreetNumber = "48",
                ZipCode = 8260,
                Town = "Viby J",
            };

            _uut.Launder(testAddr);

            _repoMock.ReceivedWithAnyArgs().Insert(new CachedAddress());
            _laundryMock.ReceivedWithAnyArgs().Launder(new Address());
        }

    
    }
}
