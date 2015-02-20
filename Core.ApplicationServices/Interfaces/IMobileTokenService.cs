using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IMobileTokenService
    {
        MobileToken Create(MobileToken token);
        bool Delete(MobileToken token);
    }
}