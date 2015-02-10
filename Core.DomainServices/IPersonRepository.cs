using Core.DomainModel;

namespace Core.DomainServices
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        void UpdateWorkDistanceOverride(int id, float newValue);
    }
}