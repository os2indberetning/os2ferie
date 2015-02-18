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
    builder.EntitySet<PersonalAddress>("PersonalAddresses");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PersonalAddressesController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<PersonalAddress> _genericRepo;

        public PersonalAddressesController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;

            _genericRepo = new GenericRepository<PersonalAddress>(new DataContext());
        }

        // GET: odata/PersonalAddresses
        [EnableQuery]
        public IQueryable<PersonalAddress> GetPersonalAddresses(ODataQueryOptions<PersonalAddress> queryOptions)
        {
            var result = _genericRepo.AsQueryable();

            return result;
        }

        // GET: odata/PersonalAddresses(5)
        [EnableQuery]
        public IQueryable<PersonalAddress> GetPersonalAddress([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            var result = _genericRepo.AsQueryable().Where(x => x.PersonId == key);

            return result;
        }

        // PUT: odata/PersonalAddresses(5)
        [EnableQuery]
        public IQueryable<PersonalAddress> Put([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/PersonalAddresses
        [EnableQuery]
        public IQueryable<PersonalAddress> Post(PersonalAddress personalAddress)
        {
            throw new NotImplementedException();
        }

        // PATCH: odata/PersonalAddresses(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<PersonalAddress> Patch([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            var existing = _genericRepo.AsQueryable().First(x => x.Id == key);

            var temp = delta.GetEntity();

            foreach (var propertyInfo in typeof(PersonalAddress).GetProperties())
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

            _genericRepo.Update(existing);
            _genericRepo.Save();

            return new List<PersonalAddress>() { existing }.AsQueryable();
        }

        // DELETE: odata/PersonalAddresses(5)
        [EnableQuery]
        public IQueryable<PersonalAddress> Delete([FromODataUri] int key)
        {
            throw new NotImplementedException();
        }
    }
}
