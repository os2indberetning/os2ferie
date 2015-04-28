using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices.Interfaces;

namespace Core.DomainServices
{
    public class CachedAddressLaunderer : IAddressLaunderer
    {
        private IGenericRepository<CachedAddress> _repo;
        private readonly IAddressLaunderer _actualLaunderer;
        private readonly IAddressCoordinates _coordinates;

        public CachedAddressLaunderer(IGenericRepository<CachedAddress> repo, IAddressLaunderer actualLaunderer, IAddressCoordinates coordinates)
        {
            _repo = repo;
            _actualLaunderer = actualLaunderer;
            _coordinates = coordinates;
        }

        public Address Launder(Address inputAddress)
        {
            var cachedAddress = _repo.AsQueryable()
                .FirstOrDefault(
                    addr =>
                        addr.StreetName == inputAddress.StreetName && addr.StreetNumber == inputAddress.StreetNumber &&
                        addr.ZipCode == inputAddress.ZipCode && addr.Town == inputAddress.Town);

            if (cachedAddress != null && !cachedAddress.IsDirty)
            {
                return cachedAddress;
            }

            if (cachedAddress == null)
            {
                cachedAddress = new CachedAddress(inputAddress);
                _repo.Insert(cachedAddress);
            }

            _actualLaunderer.Launder(cachedAddress);

            if (cachedAddress.Latitude == null || cachedAddress.Latitude.Equals("0"))
            {
                _coordinates.GetAddressCoordinates(cachedAddress);
            }

            cachedAddress.IsDirty = false;
            _repo.Save();

            return cachedAddress;
        }
    }
}
