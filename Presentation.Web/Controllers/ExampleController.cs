using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel.Example;
using Core.DomainServices.Example;
using Infrastructure.DataAccess.Example;

namespace OS2Indberetning.Controllers
{
    public class ExampleController : ApiController
    {
        public static IExample<Product> ProductRepository = new Example<Product>();

        public IEnumerable<Product> Get()
        {
            return ProductRepository.Items.ToArray();
        }

        public Product Get(Guid id)
        {
            Product entity = ProductRepository.Get(id);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return entity;
        }

        public HttpResponseMessage Post(Product value)
        {
            var result = ProductRepository.Add(value);
            if (result == null)
            {
                // the entity with this key already exists
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, value);
            string uri = Url.Link("DefaultApi", new { id = value.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public HttpResponseMessage Put(Guid id, Product value)
        {
            value.Id = id;
            var result = ProductRepository.Update(value);
            if (result == null)
            {
                // entity does not exist
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete(Guid id)
        {
            var result = ProductRepository.Delete(id);
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
