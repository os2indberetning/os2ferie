using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;
namespace OS2Indberetning.Controllers
{
    public class MobileTokenController : BaseController<MobileToken>
    {
        public MobileTokenController(IGenericRepository<MobileToken> repo) : base(repo) { }
        
        //GET: odata/MobileTokens
        [EnableQuery]
        public IQueryable<MobileToken> Get(ODataQueryOptions<MobileToken> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/MobileTokens(5)
        public IQueryable<MobileToken> Get([FromODataUri] int key, ODataQueryOptions<MobileToken> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/MobileTokens(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/MobileTokens
        [EnableQuery]
        public new IHttpActionResult Post(MobileToken mobileToken)
        {
            return base.Post(mobileToken);
        }

        //PATCH: odata/MobileTokens(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/MobileTokens(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
