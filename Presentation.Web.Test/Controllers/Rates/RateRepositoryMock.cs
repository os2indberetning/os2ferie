
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.Rates
{
    public class RateRepositoryMock: GenericRepositoryMock<Rate>
    {
        protected override List<Rate> Seed()
        {
            return new List<Rate>
            {
                new Rate
                {
                    Id  = 1,
                    Type = "Type 1",
                    Year = 2015
                },
                new Rate
                {
                    Id  = 2,
                    Type = "Type 2",
                    Year = 2035
                },
                new Rate
                {
                    Id  = 3,
                    Type = "Type 3",
                    Year = 2013
                }
            };
        }
    }
}
