using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.PersonalAddresses
{
    class PersonalAddressRepositoryMock : GenericRepositoryMock<PersonalAddress>
    {
        protected override List<PersonalAddress> Seed()
        {
            return new List<PersonalAddress>
            {
                new PersonalAddress
                {
                    Id = 1,
                    Type = PersonalAddressType.Standard
                },
                new PersonalAddress
                {
                    Id = 2,
                    Type = PersonalAddressType.AlternativeHome
                },
                new PersonalAddress
                {
                    Id = 3,
                    Type = PersonalAddressType.Work
                }
            };
        }
    }
}
