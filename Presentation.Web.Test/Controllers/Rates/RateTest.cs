using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.Rates
{
    [TestFixture]
    public class RateTest : BaseControllerTest<Rate>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Rate>),
                    typeof (RateRepositoryMock))
            };
        }

        protected override Rate GetReferenceEntity1()
        {
            return new Rate
            {
                Id = 1,
                Type = new RateType(){Description = "Type 1"},
                Year = 2015
            };
        }

        protected override Rate GetReferenceEntity2()
        {
            return new Rate
            {
                Id = 2,
                Type = new RateType() { Description = "Type 2" },
                Year = 2035
            };
        }

        protected override Rate GetReferenceEntity3()
        {
            return new Rate
            {
                Id = 3,
                Type = new RateType() { Description = "Type 3" },
                Year = 2013
            };
        }

        protected override Rate GetPostReferenceEntity()
        {
            return new Rate
            {
                Id = 4,
                Type = new RateType() { Description = "Type Posted" },
                Year = 1313
            };
        }

        protected override Rate GetPatchReferenceEntity()
        {
            return new Rate
            {
                Id = 3,
                Type = new RateType() { Description = "Type Patched" },
                Year = 2000
            };
        }

        protected override void AsssertEqualEntities(Rate entity1, Rate entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two rates should be the same");
            Assert.AreEqual(entity1.Year, entity2.Year, "Year of the two rates should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id': 4,
                        'Type': {'Description' : 'Type Posted'},
                        'Year': 1313
            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Year' : 2000
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/Rates";
        }

        protected override void ReSeed()
        {
            new RateRepositoryMock().ReSeed();
        }
    }
}
