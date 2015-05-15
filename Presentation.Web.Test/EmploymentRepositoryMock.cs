using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace Presentation.Web.Test
{
    public class EmploymentRepositoryMock : GenericRepositoryMock<Employment>
    {
        public static Employment employment = new Employment()
        {
            Id = 1,
            OrgUnitId = 1,
            OrgUnit = new OrgUnit()
            {
                Id = 1,
            },
            Person = new Person
            {
                Id = 1,
                CprNumber = "1234567894",
                FirstName = "Fissirul",
                LastName = "Lehmann",
                IsAdmin = true,
                Initials = "FL",
            },
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
