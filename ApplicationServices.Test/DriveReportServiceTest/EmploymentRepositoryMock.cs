using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    public class EmploymentRepositoryMock : GenericRepositoryMock<Employment>
    {
        private readonly Employment employment = new Employment()
        {
            Id = 1,
            OrgUnitId = 1,
            PersonId = 1,
            IsLeader = true
        };
        



        protected override List<Employment> Seed()
        {
      
           return new List<Employment>()
           {
               employment
           };
        }
        
       
    }
}
