using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers.Drive
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
        public PointsController(IGenericRepository<Point> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo){}

        //GET: odata/Points
        /// <summary>
        /// GET API endpoint for Points
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<Point> Get(ODataQueryOptions<Point> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Points(5)
        /// <summary>
        /// GET API endpoint for a single point.
        /// </summary>
        /// <param name="key">Returns the point identified by key.</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IQueryable<Point> Get([FromODataUri] int key, ODataQueryOptions<Point> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Points(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Point> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Points
        /// <summary>
        /// POST API endpoint for points.
        /// </summary>
        /// <param name="Point">The point to be posted.</param>
        /// <returns>The posted point.</returns>
        [EnableQuery]
        public new IHttpActionResult Post(Point Point)
        {
            return base.Post(Point);
        }

        //PATCH: odata/Points(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Point> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/Points(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}
