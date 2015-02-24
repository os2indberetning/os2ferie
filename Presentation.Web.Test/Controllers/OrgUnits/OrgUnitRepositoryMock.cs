using System.Collections.Generic;

namespace Presentation.Web.Test.Controllers.OrgUnits
{
    class OrgUnitRepositoryMock : GenericRepositoryMock<Core.DomainModel.OrgUnit>
    {
        protected override List<Core.DomainModel.OrgUnit> Seed()
        {
            return new List<Core.DomainModel.OrgUnit>
            {
                new Core.DomainModel.OrgUnit
                {
                    Id = 1,
                    Level = 0,
                    LongDescription = "Long dsc",
                    ShortDescription = "ID 1"
                },
                new Core.DomainModel.OrgUnit
                {
                    Id = 2,
                    Level = 0,
                    LongDescription = "Long dsc",
                    ShortDescription = "ID 2"
                },
                new Core.DomainModel.OrgUnit
                {
                    Id = 3,
                    Level = 0,
                    LongDescription = "Long dsc",
                    ShortDescription = "ID 3"
                }
            };
        }
    }
}
