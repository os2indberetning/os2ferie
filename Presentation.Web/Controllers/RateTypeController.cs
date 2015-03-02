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
        public RateTypesController(IGenericRepository<RateType> repo) : base(repo){}
        
        //GET: odata/RateTypes
        [EnableQuery]
        public IQueryable<RateType> Get(ODataQueryOptions<RateType> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/RateTypes(5)
        public IQueryable<RateType> Get([FromODataUri] int key, ODataQueryOptions<RateType> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/RateTypes(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<RateType> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/RateTypes
        [EnableQuery]
        public new IHttpActionResult Post(RateType RateType)
        {
            return base.Post(RateType);
        }

        //PATCH: odata/RateTypes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<RateType> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/OrgUnits(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}