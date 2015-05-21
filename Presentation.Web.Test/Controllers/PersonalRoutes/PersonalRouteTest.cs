using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Presentation.Web.Test.Controllers.Models;
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

        [Test]
        protected override async Task PostShouldInsertAnEntity()
        {
            const string route = @"{
                            'Description':'TestRoute',
                           'PersonId':1,
                           'Points':[{'StreetName':'Langdraget',
		                              'StreetNumber':'1',
                                      'ZipCode':2720,
                                      'Town':'Vanløse',
                                      'Latitude':'',
                                      'Longitude':'',
                                      'Description':''
                                     },{'StreetName':'J.C. Christensens Gade',
                                        'StreetNumber':'2A',
                                        'ZipCode':2300,
                                        'Town':'København S',
                                        'Latitude':'',
                                        'Longitude':'',
                                        'Description':''
                                     },{'StreetName':'Amager Boulevard',
                                        'StreetNumber':'101',
                                        'ZipCode':2300,
                                        'Town':'København S',
                                        'Latitude':'',
                                        'Longitude':'',
                                        'Description':''
                                     }
                                    ]}";

           var request = Server.CreateRequest(GetUriPath() + "")
                                .And(r => r.Content = new StringContent(route))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request.PostAsync();
            NUnit.Framework.Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode, "Post request should return status code OK");

            //After the post the repo should contain an entity with id 4, and it should match referencePost
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "?$expand=Points &$filter=Id eq 0").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<PersonalRoute>>();
            NUnit.Framework.Assert.AreEqual(1, result.value.Count, "There should be exactly one entity with id 0 after the post");

            for (var i = 0; i < result.value[0].Points.Count; i++)
            {
                if (i == 0)
                {
                    // First element
                    Assert.AreEqual(result.value[0].Points.ElementAt(i+1).Id, result.value[0].Points.ElementAt(i).NextPointId);
                }

                else if (i == result.value[0].Points.Count - 1)
                {
                    // Last element
                    Assert.AreEqual(result.value[0].Points.ElementAt(i - 1).Id,result.value[0].Points.ElementAt(i).PreviousPointId);
                }

                else
                {
                    // Viapoints
                    Assert.AreEqual(result.value[0].Points.ElementAt(i - 1).Id, result.value[0].Points.ElementAt(i).PreviousPointId);
                    Assert.AreEqual(result.value[0].Points.ElementAt(i + 1).Id, result.value[0].Points.ElementAt(i).NextPointId);
                }
            }
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
