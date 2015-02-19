using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OS2Indberetning;
using OS2Indberetning.App_Start;
using Owin;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Presentation.Web.Test.Controllers.Models;

namespace Presentation.Web.Test.Controllers
{

    /**
     * This class serves as a base for doing integration testing
     * for all the controllers. The integration test mocks the repository.
     * In case some controllers have specific functionality for certain 
     * request methods they should override the appropriate methods to get
     * the methods testing with regards to the extra functionality
     * The mocked repository should contain three entities.
     */

    

    [TestClass]
    public abstract class BaseControllerTest<T> where T : class, new()
    {
        protected TestServer Server;
       
        /**
         * Returns a list of type pairs that should be injected using ninject
         */
        protected abstract List<KeyValuePair<Type, Type>> GetInjections();

        /**
         * Gets an entity that matches the first entity in the mocked repository.
         */
        protected abstract T GetReferenceEntity1();

        /**
         * Gets an entity that matches the second entity in the mocked repository.
         */
        protected abstract T GetReferenceEntity2();

        /**
         * Gets an entity that matches the third entity in the mocked repository.
         */
        protected abstract T GetReferenceEntity3();

        /**
         * Gets an entity that matches the entity that will be created in a post request.
         * The posted entity should have id 4.
         */
        protected abstract T GetPostReferenceEntity();

        /**
         * Gets an entity that matches the entity that will be created in a patch request.
         * Notice that the patched entity will be the one with ID 3
         */
        protected abstract T GetPatchReferenceEntity();

        /*
         * Asserts that two entities are equal with respects to the properties
         * that was provided in the mocked entities.
         */
        protected abstract void AsssertEqualEntities(T entity1, T entity2);

        /**
         * Gets the content of the post body that should be send to test the post request
         * The posted content should have ID 4.
         */
        protected abstract string GetPostBodyContent();

        /**
         * Gets the content of the patch body that should be send to test the post request
         * Do not alter the ID of the entity. The entity with ID 3 will be altered.
         */
        protected abstract string GetPatchBodyContent();

        /**
         * Gets the odata uri path used in the requests, e.g. "/odata/OrgUnits"
         */
        protected abstract string GetUriPath();

        /**
         * Reseeds the repository. This is run before each test.
         */
        protected abstract void ReSeed();

        [TestInitialize]
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

        [TestCleanup]
        public void TearDown()
        {
            if (Server != null)
                Server.Dispose();
        }

        [TestMethod]
        public async Task GetShouldReturnThreeElements()
        {
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath()).GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response to get request should be OK");
            Assert.AreEqual(3, result.value.Count, "Expects the return of a get request to have three entitys");
        }

        [TestMethod]
        public async Task GetWithKeyShouldReturnCorrectElement()
        {
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(2)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response to get request should be OK");
            Assert.AreEqual(1, result.value.Count, "Expects the return of a get request with key is to have one entity");
            var entity = result.value[0];
            AsssertEqualEntities(GetReferenceEntity2(), entity);
        }

        [TestMethod]
        public async Task GetWithInvalidKeyShouldReturnNoEntity()
        {
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(5)").GetAsync();
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Unknown id should resolve in an internal server error");
        }

        [TestMethod]
        public async Task GetWithOdataQuery()
        {
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "?$orderby=Id desc").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response to get request should be OK");
            Assert.AreEqual(3, result.value.Count, "Expects the return of a get request to have three entitys");
            var entity = result.value[0];
            AsssertEqualEntities(GetReferenceEntity3(), entity);
            entity = result.value[1];
            AsssertEqualEntities(GetReferenceEntity2(), entity);
            entity = result.value[2];
            AsssertEqualEntities(GetReferenceEntity1(), entity);
        }

        [TestMethod]
        public async Task PutShouldReturnMethodNotAllowed()
        {
            var httpContent = new StreamContent(Stream.Null);
            var response = await Server.HttpClient.PutAsync(GetUriPath() + "(3)", httpContent);
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode, "Put method should not be allowed");
        }

        [TestMethod]
        public async Task PostShouldInsertAnEntity()
        {
            //Make sure that an entity with id 4 does not exists before the test
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "The repo should not contain an item with id 4");


            var request = Server.CreateRequest(GetUriPath() + "")
                                .And(r => r.Content = new StringContent(GetPostBodyContent()))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var postResponse = await request.PostAsync();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode, "Post request should return status code Created");

            //After the post the repo should contain an entity with id 4, and it should match referencePost
            response = await Server.CreateRequest(GetUriPath() + "(4)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 4 after the post");
            AsssertEqualEntities(GetPostReferenceEntity(), result.value[0]);
        }

        [TestMethod]
        public async Task PatchShouldAlterAnEntity()
        {
            //Make sure that an entity with id 3 looks as expected
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(3)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 3 before the patch");
            AsssertEqualEntities(GetReferenceEntity3(), result.value[0]);

            var request = Server.CreateRequest(GetUriPath() + "(3)")
                                .And(r => r.Content = new StringContent(GetPatchBodyContent()))
                                .And(r => r.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"));
            var patchResponse = await request.SendAsync("PATCH");
            Assert.AreEqual(HttpStatusCode.NoContent, patchResponse.StatusCode, "Patch request should return status code NoContent");

            //After the patch the repo should contain an entity with id 3, and it should match the reference patch entity
            response = await Server.CreateRequest(GetUriPath() + "(3)").GetAsync();
            result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 3 after the patch");
            AsssertEqualEntities(GetPatchReferenceEntity(), result.value[0]);
        }

        [TestMethod]
        public async Task DeleteShouldRemoveAnEntity()
        {
            //Make sure that an entity with id 3 exists
            HttpResponseMessage response = await Server.CreateRequest(GetUriPath() + "(3)").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<T>>();
            Assert.AreEqual(1, result.value.Count, "There should be excatly one entity with id 3 before the patch");

            var request = Server.CreateRequest(GetUriPath() + "(3)");
            var patchResponse = await request.SendAsync("DELETE");
            Assert.AreEqual(HttpStatusCode.OK, patchResponse.StatusCode, "Delete request should return status code OK");

            //After the delete the repo should not contain an entity with id 3
            response = await Server.CreateRequest(GetUriPath() + "(3)").GetAsync();
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "No entity with id 3 should exists, so internal server error is returned");
        }

    }
}
