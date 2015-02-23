using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.PersonService
{
    [TestFixture]
    public class PersonServiceTest
    {
        private IQueryable<Person> persons = new List<Person>
        {
            new Person
            {
                Id = 1,
                FirstName = "Morten",
                LastName = "Rasmussen",
                CprNumber = "1234567890"
            },
            new Person
            {
                Id = 2,
                FirstName = "Morten",
                LastName = "Jørgensen",
                CprNumber = "0987654321"
            }
        }.AsQueryable();

        [Test]
        public void ScrubCprsShouldRemoveCprNumbers()
        {
            foreach (var person in persons)
            {
                Assert.AreNotEqual("", person.CprNumber, "Person should have a CPR number before scrubbing");
            }
            persons = new Core.ApplicationServices.PersonService().ScrubCprFromPersons(persons);
            foreach (var person in persons)
            {
                Assert.AreEqual("", person.CprNumber, "Person should not have a CPR number before scrubbing");
            }
        }
    }
}
