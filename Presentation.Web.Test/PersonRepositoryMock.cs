using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace Presentation.Web.Test
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
                    FirstName = "Fissirul",
                    LastName = "Lehmann",
                    IsAdmin = true,
                    Initials = "FL",
                },
                new Person
                {
                    Id = 4,
                    CprNumber = "1234567894",
                    FirstName = "Morten",
                    LastName = "Rasmussen",
                    IsAdmin = true,
                    Initials = "MR",
                },
                new Person
                {
                    Id = 2,
                    CprNumber = "5761587423",
                    FirstName = "Morten",
                    LastName = "Jørgensen",
                    IsAdmin = true,
                    Initials = "MJ"
                },
                new Person
                {
                    Id = 3,
                    CprNumber = "8754875412",
                    FirstName = "Morten",
                    LastName = "Foo",
                    IsAdmin = true,
                    Initials = "MF"
                },
                new Person
                {
                    Id = 2308,
                    CprNumber = "123456781234",
                    FirstName = "Test",
                    LastName = "Foo",
                    IsAdmin = true,
                    IsActive = true,
                    Initials = "hshu"
                }
            };
        }
    }
}
