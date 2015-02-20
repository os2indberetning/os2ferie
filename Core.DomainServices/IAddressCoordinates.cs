using Core.DomainModel;

namespace Core.DomainServices
{
    public interface IAddressCoordinates
    {
        Address GetAddressFromCoordinates(Address addressCoord);
        Address GetAddressCoordinates(Address address, bool correctAddresses = false);
    }
}
