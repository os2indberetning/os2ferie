
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.PersonalRoutes
{
    public class PersonalRouteRepositoryMock: GenericRepositoryMock<PersonalRoute>
    {
        protected override List<PersonalRoute> Seed()
        {
            return new List<PersonalRoute>
            {
                new PersonalRoute
                {
                    Id  = 1,
                    Description = "Description 1",
                    PersonId = 8
                },
                new PersonalRoute
                {
                    Id  = 2,
                    Description = "Description 2",
                    PersonId = 4
                },
                new PersonalRoute
                {
                    Id  = 3,
                    Description = "Description 3",
                    PersonId = 2
                }
            };
        }
    }
}
