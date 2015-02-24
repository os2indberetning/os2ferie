using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.PersonalRoutes
{
    [TestFixture]
    public class PersonalRouteTest : BaseControllerTest<PersonalRoute>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<PersonalRoute>),
                    typeof (PersonalRouteRepositoryMock))
            };
        }

        protected override PersonalRoute GetReferenceEntity1()
        {
            return new PersonalRoute
            {
                Id = 1,
                Description = "Description 1",
                PersonId = 8
            };
        }

        protected override PersonalRoute GetReferenceEntity2()
        {
            return new PersonalRoute
            {
                Id = 2,
                Description = "Description 2",
                PersonId = 4
            };
        }

        protected override PersonalRoute GetReferenceEntity3()
        {
            return new PersonalRoute
            {
                Id = 3,
                Description = "Description 3",
                PersonId = 2
            };
        }

        protected override PersonalRoute GetPostReferenceEntity()
        {
            return new PersonalRoute
            {
                Id = 4,
                Description = "Description Posted",
                PersonId = 1
            };
        }

        protected override PersonalRoute GetPatchReferenceEntity()
        {
            return new PersonalRoute
            {
                Id = 3,
                Description = "Description patched",
                PersonId = 0
            };
        }

        protected override void AsssertEqualEntities(PersonalRoute entity1, PersonalRoute entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two personal Routes should be the same");
            Assert.AreEqual(entity1.Description, entity2.Description, "Description of the two personal Routes should be the same");
            Assert.AreEqual(entity1.PersonId, entity2.PersonId, "PersonId of the two personal Routes should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id': 4,
                        'Description' : 'Description Posted',
                        'PersonId' : 1
            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Description' : 'Description patched',
                        'PersonId' : 0
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/PersonalRoutes";
        }

        protected override void ReSeed()
        {
            new PersonalRouteRepositoryMock().ReSeed();
        }
    }
}
