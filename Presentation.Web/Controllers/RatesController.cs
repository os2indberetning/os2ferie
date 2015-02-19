using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class RatesController : BaseController<Rate>
    {
          //GET: odata/Rates
        public RatesController(IGenericRepository<Rate> repository) : base(repository){}

        [EnableQuery]
        public IQueryable<Rate> Get(ODataQueryOptions<Rate> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Rates(5)
        public IQueryable<Rate> Get([FromODataUri] int key, ODataQueryOptions<Rate> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Rates(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Rate> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Rates
        [EnableQuery]
        public new IHttpActionResult Post(Rate Rate)
        {
            return base.Post(Rate);
        }

        //PATCH: odata/Rates(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Rate> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/Rates(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
        
        // GET: odata/Rates/RateService.ThisYearsRates
        [EnableQuery]
        [HttpGet]
        public IQueryable<Rate> ThisYearsRates()
        {
            var result = _repo.AsQueryable().Where(x => x.Year == (DateTime.Now).Year);
            return result;
        }
    }
}
