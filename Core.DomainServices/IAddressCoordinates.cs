namespace Core.DomainServices
{
    public interface IAddressCoordinates<T>
    {
        T GetAddressCoordinates(T address);
    }
}
