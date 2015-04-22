using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Infrastructure.AddressServices.Interfaces
{
    public interface IAddressCoordinates
    {
        Address GetAddressFromCoordinates(Address addressCoord);
        Address GetAddressCoordinates(Address address, bool correctAddresses = false);
    }
}
