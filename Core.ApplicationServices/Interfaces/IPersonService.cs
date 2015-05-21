using System.Linq;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IPersonService
    {
        IQueryable<Person> ScrubCprFromPersons(IQueryable<Person> queryable);
        PersonalAddress GetHomeAddress(Person person);
        Person AddHomeWorkDistanceToEmployments(Person person);
    }
}
