using System.Linq;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IPersonService
    {
        IQueryable<Person> ScrubCprFromPersons(IQueryable<Person> queryable);
        void AddFullName(IQueryable<Person> persons);
        PersonalAddress GetHomeAddress(Person person);
    }
}
