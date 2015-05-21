using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace Presentation.Web.Test
{
    public class OrgUnitRepositoryMock : GenericRepositoryMock<OrgUnit>
    {
        private readonly OrgUnit orgUnit = new OrgUnit()
        {
            Id = 1,
        };
        



        protected override List<OrgUnit> Seed()
        {
      
           return new List<OrgUnit>()
           {
               orgUnit
           };
        }
        
       
    }
}
