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

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Point>("Points");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PointsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<Point> _repo;

        public PointsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;

            _repo = new GenericRepository<Point>(new DataContext());
        }

        // GET: odata/Points
        [EnableQuery]
        public IQueryable<Point> GetPoints(ODataQueryOptions<Point> queryOptions)
        {
            throw new NotImplementedException();
        }

        // GET: odata/Points(5)
        [EnableQuery]
        public IQueryable<Point> GetPoint([FromODataUri] int key, ODataQueryOptions<Point> queryOptions)
        {
            var result = _repo.AsQueryable().Where(x => x.PersonalRouteId == key);

            return result;
        }

        // PUT: odata/Points(5)
        [EnableQuery]
        public IQueryable<Point> Put([FromODataUri] int key, Delta<Point> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/Points
        [EnableQuery]
        public IQueryable<Point> Post(Point point)
        {
            throw new NotImplementedException();
        }

        // PATCH: odata/Points(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<Point> Patch([FromODataUri] int key, Delta<Point> delta)
        {
            throw new NotImplementedException();
        }

        // DELETE: odata/Points(5)
        [EnableQuery]
        public IQueryable<Point> Delete([FromODataUri] int key)
        {
            throw new NotImplementedException();
        }
    }
}
