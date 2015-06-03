using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class RateTypesController : BaseController<RateType>
    {
        public RateTypesController(IGenericRepository<RateType> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo){}
        
        //GET: odata/RateTypes
        /// <summary>
        /// GET API endpoint for RateTypes.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>RateTypes</returns>
        [EnableQuery]
        public IQueryable<RateType> Get(ODataQueryOptions<RateType> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/RateTypes(5)
        /// <summary>
        /// GET API endpoint for a single RateType
        /// </summary>
        /// <param name="key">Returns the RateType identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single RateType</returns>
        public IQueryable<RateType> Get([FromODataUri] int key, ODataQueryOptions<RateType> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/RateTypes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<RateType> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/RateTypes
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="RateType"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IHttpActionResult Post(RateType RateType)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //PATCH: odata/RateTypes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<RateType> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/OrgUnits(5)
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