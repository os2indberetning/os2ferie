using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class RatesController : BaseController<Rate>
    {
        RatePostService ratePostService = new RatePostService();

          //GET: odata/Rates
        public RatesController(IGenericRepository<Rate> repository, IGenericRepository<Person> personRepo) : base(repository, personRepo){}

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
            if (CurrentUser.IsAdmin)
            {
                ratePostService.DeactivateExistingRate(Repo.AsQueryable(), Rate);
                return base.Post(Rate);
            }
            return Unauthorized();
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
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
        
        // GET: odata/Rates/Service.ThisYearsRates
        [EnableQuery]
        [HttpGet]
        public IQueryable<Rate> ThisYearsRates()
        {
            var result = Repo.AsQueryable().Where(x => x.Year == (DateTime.Now).Year && x.Active);
            return result;
        }
    }
}
