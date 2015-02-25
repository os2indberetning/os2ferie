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
    builder.EntitySet<Point>("Points");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PointsController : BaseController<Point>
    {
        public PointsController(IGenericRepository<Point> repo) : base(repo){}

        //GET: odata/Points
        [EnableQuery]
        public IQueryable<Point> Get(ODataQueryOptions<Point> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Points(5)
        public IQueryable<Point> Get([FromODataUri] int key, ODataQueryOptions<Point> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Points(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Point> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Points
        [EnableQuery]
        public new IHttpActionResult Post(Point Point)
        {
            return base.Post(Point);
        }

        //PATCH: odata/Points(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Point> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/Points(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
