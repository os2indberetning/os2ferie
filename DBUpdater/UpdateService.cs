using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices.Interfaces;

namespace DBUpdater
{
    public class UpdateService
    {
        private readonly CachedAddressLaunderer _cachedLaunderer; 
        public UpdateService(CachedAddressLaunderer cachedLaunderer)
        {
            _cachedLaunderer = cachedLaunderer; 
        }

        public Address GetCleanAddress(Address input)
        {
            return _cachedLaunderer.Launder(input);
        }
    }
}
