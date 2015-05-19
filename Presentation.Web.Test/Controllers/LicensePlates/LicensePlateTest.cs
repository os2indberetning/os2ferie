using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using OS2Indberetning;
using Owin;
using Presentation.Web.Test.Controllers.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.LicensePlates
{
    [TestFixture]
    public class LicensePlateTest
    {






        // =============================   DISCLAIMER =======================================
        // These tests only work if the actual people table contains the AD user set in basecontroller, and the personId on the license plate test data is the
        // Same as the id of that AD person.



        public TestServer Server { get; set; }

        [SetUp]
        public void Setup()
        {
            Server = TestServer.Create(app =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                config.DependencyResolver = new NinjectDependencyResolver(NinjectTestInjector.CreateKernel(GetInjections()));
                app.UseWebApi(config);
            });
            //Bit of a hack to make sure that the repository is seeded
            //before each test, but at the same time that it does not 
            //seed each time it is loaded which forgets state if it is
            //queried multiple times during a single test
            ReSeed();
        }

        private List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<LicensePlate>),
                    typeof (LicensePlateRepositoryMock))
            };
        }



        [Test]
        public async void NoExistingPlates_PostedPlate_ShouldBeMadePrimary()
        {
            ReSeed();
            //Make sure that an entity with id 4 does not exists before the test
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            AssertEmptyResponse(response);

            var request = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode, "Post request should return status code Created");

            //After the post the repo should contain an entity with id 4, and it should match referencePost
            response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 4 after the post");
            Assert.AreEqual(true,result.value[0].IsPrimary, "The posted licenseplate should be made primary");
        }

        [Test]
        public async void OneExistingPlate_PostedPlate_ShouldNotBeMadePrimary()
        {
            ReSeed();
            //Make sure that an entity with id 4 does not exists before the test
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            AssertEmptyResponse(response);

            //Make sure that an entity with id 5 does not exists before the test
            HttpResponseMessage response2 = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            AssertEmptyResponse(response2);


            // Create first plate
            var request = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode, "Post request should return status code Created");

            // Create second plate
            var request2 = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent2()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse2 = await request2.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse2.StatusCode, "Post request should return status code Created");

            //After the post the repo should contain an entity with id 4
            response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 4 after the post");
            Assert.AreEqual(true, result.value[0].IsPrimary, "The posted licenseplate should be made primary");

            //After the post the repo should contain an entity with id 5
            response = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            var result2 = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            Assert.AreEqual(1, result2.value.Count, "There should be excatly one entity with id 5 after the post");
            Assert.AreEqual(false, result2.value[0].IsPrimary, "The second posted licenseplate should not be made primary");
        }


        [Test]
        public async Task TwoExistingPlates_DeletePrimary_RemainingShouldBeMadePrimary()
        {
            ReSeed();
            //Make sure that an entity with id 4 does not exists before the test
            HttpResponseMessage response1 = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            AssertEmptyResponse(response1);

            //Make sure that an entity with id 5 does not exists before the test
            HttpResponseMessage response2 = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            AssertEmptyResponse(response2);


            // Create first plate
            var request1 = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request1.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode, "Post request should return status code Created");

            // Create second plate
            var request2 = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent2()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse2 = await request2.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse2.StatusCode, "Post request should return status code Created");

            //Make sure that an entity with id 3 exists
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            NUnit.Framework.Assert.AreEqual(1, result.value.Count, "There should be exactly one entity with id 4 before the delete");

            var request = Server.CreateRequest(GetUriPath() + "(4)");
            var patchResponse = await request.SendAsync("DELETE");
            NUnit.Framework.Assert.AreEqual(HttpStatusCode.OK, patchResponse.StatusCode, "Delete request should return status code OK");

            //After the delete the repo should not contain an entity with id 4
            response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            AssertEmptyResponse(response);

            //After the post the repo should contain an entity with id 5 which is primary
            response = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            var result2 = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            Assert.AreEqual(1, result2.value.Count, "There should be excatly one entity with id 5 after the post");
            Assert.AreEqual(true, result2.value[0].IsPrimary, "The second posted licenseplate should be made primary");
        }

        [Test]
        public async Task TwoExistingPlates_DeleteNonPrimary_RemainingShouldStillBePrimary()
        {
            ReSeed();
            //Make sure that an entity with id 4 does not exists before the test
            HttpResponseMessage response1 = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            AssertEmptyResponse(response1);

            //Make sure that an entity with id 5 does not exists before the test
            HttpResponseMessage response2 = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            AssertEmptyResponse(response2);


            // Create first plate
            var request1 = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request1.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode, "Post request should return status code Created");

            // Create second plate
            var request2 = Server.CreateRequest(GetUriPath() + "")
                                 .And(r => r.Content = new StringContent(GetPostBodyContent2()))
                                 .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse2 = await request2.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse2.StatusCode, "Post request should return status code Created");

            //Make sure that an entity with id 4 exists
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            NUnit.Framework.Assert.AreEqual(1, result.value.Count, "There should be exactly one entity with id 4 before the delete");

            var request = Server.CreateRequest(GetUriPath() + "(5)");
            var patchResponse = await request.SendAsync("DELETE");
            NUnit.Framework.Assert.AreEqual(HttpStatusCode.OK, patchResponse.StatusCode, "Delete request should return status code OK");

            //After the delete the repo should not contain an entity with id 5
            response = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            AssertEmptyResponse(response);

            //After the post the repo should contain an entity with id 4 which is primary
            response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result2 = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            Assert.AreEqual(1, result2.value.Count, "There should be excatly one entity with id 4 after the post");
            Assert.AreEqual(true, result2.value[0].IsPrimary, "The second posted licenseplate should be made primary");
        }


        private string GetUriPath()
        {
            return "/odata/LicensePlates";
        }

        protected string GetPostBodyContent()
        {
            return @"{
                        'Id' : 4,
                        'PersonId' : 2302,
                        'Plate' : '666',
                        'Description' : 'Posted Description',
                        'IsPrimary'   : 'false'
                    }";
        }

        protected string GetPostBodyContent2()
        {
            return @"{
                        'Id' : 5,
                        'PersonId': 2302,
                        'Plate' : '666',
                        'Description' : 'Posted Description',
                        'IsPrimary'   : 'false'
                    }";
        }

        public async static void AssertEmptyResponse(HttpResponseMessage response)
        {
            NUnit.Framework.Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Return code of get should be ok");
            var result = await response.Content.ReadAsAsync<ODataResponse<LicensePlate>>();
            NUnit.Framework.Assert.AreEqual(0, result.value.Count, "Nothing should be return from get request");
        }


        [TearDown]
        public void TearDown()
        {
            if (Server != null)
                Server.Dispose();
        }


        protected void ReSeed()
        {
            new LicensePlateRepositoryMock().ReSeed();
        }
    }
}
