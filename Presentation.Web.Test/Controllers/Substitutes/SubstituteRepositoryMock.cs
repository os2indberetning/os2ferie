
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.Substitutes
{
    public class SubstituteRepositoryMock: GenericRepositoryMock<Substitute>
    {
        protected override List<Substitute> Seed()
        {
            return new List<Substitute>
            {
                new Substitute
                {
                    Id  = 1,
                    EndDateTimestamp = 1337,
                    StartDateTimestamp = 1336
                },
                new Substitute
                {
                    Id  = 2,
                    EndDateTimestamp = 2337,
                    StartDateTimestamp = 2336
                },
                new Substitute
                {
                    Id  = 3,
                    EndDateTimestamp = 3337,
                    StartDateTimestamp = 3336
                }
            };
        }
    }
}
