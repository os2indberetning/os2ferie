using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Routing;
using Core.DomainModel;
using Microsoft.Data.OData;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Person>("Person");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PersonController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        // GET: odata/Person
        [EnableQuery]
        public IQueryable<Person> GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            throw new NotImplementedException();
        }

        // GET: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            throw new NotImplementedException();
        }

        // PUT: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> Put([FromODataUri] int key, Delta<Person> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/Person
        [EnableQuery]
        public IQueryable<Person> Post(Person person)
        {
            throw new NotImplementedException();
        }

        // PATCH: odata/Person(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<Person> Patch([FromODataUri] int key, Delta<Person> delta)
        {            
            throw new NotImplementedException();
        }

        // DELETE: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> Delete([FromODataUri] int key)
        {
            throw new NotImplementedException();
        }

        // PUT: odata/Person/SetHomeWorkOverride
        [EnableQuery]
        public IQueryable<bool> SetHomeWorkOverride([FromODataUri] float value)
        {
            int i = 0;

            return new List<bool>(){true}.AsQueryable();
        }
    }
}
