
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.PersonalRoutes
{
    public class PersonalRouteRepositoryMock : GenericRepositoryMock<PersonalRoute>
    {
        public static List<PersonalRoute> Items;

        protected override List<PersonalRoute> Seed()
        {
            Items = new List<PersonalRoute>
            {
                new PersonalRoute
                {
                    Id  = 1,
                    Description = "Description 1",
                    PersonId = 8,
                    Points = new List<Point>()
                },
                new PersonalRoute
                {
                    Id  = 2,
                    Description = "Description 2",
                    PersonId = 4,
                    Points = new List<Point>()

                },
                new PersonalRoute
                {
                    Id  = 3,
                    Description = "Description 3",
                    PersonId = 2,
                    Points = new List<Point>()
                }
            };
            return Items;
        }

        public override void Save()
        {
            var counter = 0;
            foreach (var personalRoute in Items)
            {
                foreach (var point in personalRoute.Points)
                {
                    point.Id = counter;
                    counter++;
                }
            }
        }
    }
}
