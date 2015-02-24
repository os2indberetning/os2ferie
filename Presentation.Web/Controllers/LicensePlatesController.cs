using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.Http.OData.Routing;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Microsoft.Data.OData;
using Microsoft.SqlServer.Server;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<LicensePlate>("LicensePlates");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class LicensePlatesController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();
        private readonly IGenericRepository<LicensePlate> _genericRepo;

        public LicensePlatesController()
        {
            _genericRepo = new GenericRepository<LicensePlate>(new DataContext());
        }

        // GET: odata/LicensePlates
        [EnableQuery]
        public IQueryable<LicensePlate> GetLicensePlates(ODataQueryOptions<LicensePlate> queryOptions)
        {
            throw new NotImplementedException();
        }

        // GET: odata/LicensePlates(5)
        [EnableQuery]
        public IQueryable<LicensePlate> GetLicensePlate([FromODataUri] int key, ODataQueryOptions<LicensePlate> queryOptions)
        {
            var result = _genericRepo.AsQueryable().Where(x => x.Person.Id == key);

            return result;
        }

        // PUT: odata/LicensePlates(5)
        [EnableQuery]
        public IQueryable<LicensePlate> Put([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/LicensePlates
        [EnableQuery]
        public IQueryable<LicensePlate> Post(LicensePlate licensePlate)
        {
            _genericRepo.Insert(licensePlate);

            _genericRepo.Save();

            return new List<LicensePlate>() { licensePlate }.AsQueryable();
        }

        // PATCH: odata/LicensePlates(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<LicensePlate> Patch([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            throw new NotImplementedException();
        }

        // DELETE: odata/LicensePlates(5)
        [EnableQuery]
        public IQueryable<LicensePlate> Delete([FromODataUri] int key)
        {
            var plate = _genericRepo.AsQueryable().First(x => x.Id == key);

            if (plate != null)
            {
                _genericRepo.Delete(plate);

                _genericRepo.Save();

                return new List<LicensePlate>() { new LicensePlate() }.AsQueryable();
            }
            else
            {
                return new List<LicensePlate>() { plate }.AsQueryable();
            }
        }
    }
}
