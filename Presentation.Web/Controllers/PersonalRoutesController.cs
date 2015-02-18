using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.Http.OData.Routing;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Microsoft.Data.OData;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<PersonalRoute>("PersonalRoutes");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PersonalRoutesController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<PersonalRoute> _repo;

        public PersonalRoutesController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;

            _repo = new GenericRepository<PersonalRoute>(new DataContext());
        }

        // GET: odata/PersonalRoutes
        [EnableQuery]
        public IQueryable<PersonalRoute> GetPersonalRoutes(ODataQueryOptions<PersonalRoute> queryOptions)
        {
            var result = _repo.AsQueryable();

            return result;
        }

        // GET: odata/PersonalRoutes(5)
        [EnableQuery]
        public IQueryable<PersonalRoute> GetPersonalRoute([FromODataUri] int key, ODataQueryOptions<PersonalRoute> queryOptions)
        {
            var result = _repo.AsQueryable().Where(x => x.PersonId == key);

            return result;
        }

        // PUT: odata/PersonalRoutes(5)
        [EnableQuery]
        public IQueryable<PersonalRoute> Put([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/PersonalRoutes
        [EnableQuery]
        public IQueryable<PersonalRoute> Post(PersonalRoute personalRoute)
        {
            var result = _repo.Insert(personalRoute);

            _repo.Save();

            return new List<PersonalRoute>() { result }.AsQueryable();
        }

        // PATCH: odata/PersonalRoutes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<PersonalRoute> Patch([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            var existing = _repo.AsQueryable().First(x => x.Id == key);

            var temp = delta.GetEntity();

            foreach (var propertyInfo in typeof(PersonalRoute).GetProperties())
            {
                var itemType = existing.GetType();

                PropertyInfo prop;

                if (propertyInfo.Name == "Id")
                    continue; // skip primary key

                if (propertyInfo.GetValue(temp) != null)
                {
                    prop = itemType.GetProperty(propertyInfo.Name);

                    prop.SetValue(existing, propertyInfo.GetValue(temp));
                }
            }

            _repo.Update(existing);
            _repo.Save();

            return new List<PersonalRoute>() { existing }.AsQueryable();
        }

        // DELETE: odata/PersonalRoutes(5)
        [EnableQuery]
        public IQueryable<PersonalRoute> Delete([FromODataUri] int key)
        {
            throw new NotImplementedException();
        }
    }
}
