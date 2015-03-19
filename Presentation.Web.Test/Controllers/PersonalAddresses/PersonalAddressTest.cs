using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.PersonalAddresses
{
    [TestFixture]
    public class PersonalAddressTest : BaseControllerTest<PersonalAddress>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<PersonalAddress>),
                    typeof (PersonalAddressRepositoryMock))
            };
        }

        protected override PersonalAddress GetReferenceEntity1()
        {
            return new PersonalAddress
            {
                Id = 1,
                Type = PersonalAddressType.Standard
            };
        }

        protected override PersonalAddress GetReferenceEntity2()
        {
            return new PersonalAddress
            {
                Id = 2,
                Type = PersonalAddressType.AlternativeHome
            };
        }

        protected override PersonalAddress GetReferenceEntity3()
        {
            return new PersonalAddress
            {
                Id = 3,
                Type = PersonalAddressType.Work
            };
        }

        protected override PersonalAddress GetPostReferenceEntity()
        {
            return new PersonalAddress
            {
                Id = 4,
                Type = PersonalAddressType.AlternativeWork,
                StreetName = "Katrinebjergvej",
                StreetNumber = "93B",
                ZipCode = 8200,
                Town = "Aarhus N"
            };
        }

        protected override PersonalAddress GetPatchReferenceEntity()
        {
            return new PersonalAddress
            {
                Id = 3,
                Type = PersonalAddressType.AlternativeWork
            };
        }

        protected override void AsssertEqualEntities(PersonalAddress entity1, PersonalAddress entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two personal addresses should be the same");
            Assert.AreEqual(entity1.Type, entity2.Type, "Type of the two personal addresses should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id': 4,
                        'Type': 'AlternativeWork',
                        'StreetName'    :   'Katrinebjergvej',
                        'StreetNumber'  :   '93B',
                        'ZipCode'       :   8200,
                        'Town'          :   'Aarhus N'

            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Type': 'AlternativeWork'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/PersonalAddresses";
        }

        protected override void ReSeed()
        {
            new PersonalAddressRepositoryMock().ReSeed();
        }
    }
}
