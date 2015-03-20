
using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.Addresses
{
    public class AddressRepositoryMock: GenericRepositoryMock<Address>
    {
        public static List<Address> addresses = new List<Address>()
        {
             new Address
                {
                    Id  = 1,
                    Description = "Desc 1",
                    StreetName = "First street"
                },
                new Address
                {
                    Id  = 2,
                    Description = "Desc 2",
                    StreetName = "Second street"
                },
                new Address
                {
                    Id  = 3,
                    Description = "Desc 3",
                    StreetName = "Third street"
                }
        }; 

        protected override List<Address> Seed()
        {
            return addresses;
        }
    }
}
