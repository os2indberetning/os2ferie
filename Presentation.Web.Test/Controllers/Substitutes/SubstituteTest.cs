using System;
using System.Collections.Generic;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.Substitutes
{
    [TestFixture]
    public class SubstituteTest : BaseControllerTest<Substitute>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Substitute>),typeof (SubstituteRepositoryMock)),
            };
        }

        protected override Substitute GetReferenceEntity1()
        {
            return new Substitute
            {
                Id = 1,
                EndDateTimestamp = 1337,
                StartDateTimestamp = 1336
            };
        }

        protected override Substitute GetReferenceEntity2()
        {
            return new Substitute
            {
                Id = 2,
                EndDateTimestamp = 2337,
                StartDateTimestamp = 2336
            };
        }

        protected override Substitute GetReferenceEntity3()
        {
            return new Substitute
            {
                Id = 3,
                EndDateTimestamp = 3337,
                StartDateTimestamp = 3336
            };
        }

        protected override Substitute GetPostReferenceEntity()
        {
            return new Substitute
            {
                Id = 4,
                EndDateTimestamp = 4337,
                StartDateTimestamp = 4336
            };
        }

        protected override Substitute GetPatchReferenceEntity()
        {
            return new Substitute
            {
                Id = 3,
                EndDateTimestamp = 5337,
                StartDateTimestamp = 5336
            };
        }

        protected override void AsssertEqualEntities(Substitute entity1, Substitute entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two Substitutes should be the same");
            Assert.AreEqual(entity1.EndDateTimestamp, entity2.EndDateTimestamp, "EndDateTimestamp of the two Substitutes should be the same");
            Assert.AreEqual(entity1.StartDateTimestamp, entity2.StartDateTimestamp, "StartDateTimestamp of the two Substitutes should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id': 4,
                        'EndDateTimestamp': 4337,
                        'StartDateTimestamp': 4336,
                        'Leader' : {
                                    'FirstName' : 'Preben',
                                    'LastName'  : 'Hansen'
                                   },
                        'Sub'    :  {
                                    'FirstName' : 'Carsten',
                                    'LastName'  : 'Jensen'
                                    },
                        'Person' : {'FirstName' : 'Brian', 'LastName' : 'Sørensen'} 
            }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'EndDateTimestamp': 5337,
                        'StartDateTimestamp': 5336
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/Substitutes";
        }

        protected override void ReSeed()
        {
            new SubstituteRepositoryMock().ReSeed();
        }
    }
}
