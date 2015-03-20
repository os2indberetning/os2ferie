using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.Addresses
{
    [TestFixture]
    public class AddressTest : BaseControllerTest<Address>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Address>),
                    typeof (AddressRepositoryMock))
            };
        }

        protected override Address GetReferenceEntity1()
        {
            return new Address
            {
                Id = 1,
                Description = "Desc 1",
                StreetName = "First street"
            };
        }

        protected override Address GetReferenceEntity2()
        {
            return new Address
            {
                Id = 2,
                Description = "Desc 2",
                StreetName = "Second street"
            };
        }

        protected override Address GetReferenceEntity3()
        {
            return new Address
            {
                Id = 3,
                Description = "Desc 3",
                StreetName = "Third street"
            };
        }

        protected override Address GetPostReferenceEntity()
        {
            return new Address
            {
                Id = 4,
                Description = "Desc 4",
                StreetName = "Posted street"
            };
        }

        protected override Address GetPatchReferenceEntity()
        {
            return new Address
            {
                Id = 3,
                Description = "Desc Patched",
                StreetName = "Patched street"
            };
        }

        protected override void AsssertEqualEntities(Address entity1, Address entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two Addresss should be the same");
            Assert.AreEqual(entity1.Description, entity2.Description, "Description of the two Addresss should be the same");
            Assert.AreEqual(entity1.StreetName, entity2.StreetName, "StreetName of the two Addresss should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id': 4,
                        'Description': 'Desc 4',
                        'StreetName': 'Posted street'
            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Description' : 'Desc Patched',
                        'StreetName' : 'Patched street'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/Addresses";
        }

        protected override void ReSeed()
        {
            AddressRepositoryMock.addresses = new List<Address>()
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
            new AddressRepositoryMock().ReSeed();
        }
    }
}
