using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.Persons
{
    class PersonRepositoryMock : GenericRepositoryMock<Person>
    {
        protected override List<Person> Seed()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = 1,
                    CprNumber = "1234567894",
                    FirstName = "Morten",
                    LastName = "Rasmussen"
                },
                new Person
                {
                    Id = 2,
                    CprNumber = "5761587423",
                    FirstName = "Morten",
                    LastName = "Jørgensen"
                },
                new Person
                {
                    Id = 3,
                    CprNumber = "8754875412",
                    FirstName = "Morten",
                    LastName = "Foo"
                }
            };
        }
    }
}
