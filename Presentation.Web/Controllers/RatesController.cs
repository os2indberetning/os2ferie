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
        readonly RatePostService _ratePostService = new RatePostService();

          //GET: odata/Rates
        public RatesController(IGenericRepository<Rate> repository, IGenericRepository<Person> personRepo) : base(repository, personRepo){}

        /// <summary>
        /// GET API endpoint for Rates.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Rates</returns>
        [EnableQuery]
        public IQueryable<Rate> Get(ODataQueryOptions<Rate> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Rates(5)
        /// <summary>
        /// GET API endpoint for a single Rate.
        /// </summary>
        /// <param name="key">Returns the rate identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        public IQueryable<Rate> Get([FromODataUri] int key, ODataQueryOptions<Rate> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }


        //PUT: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Rate> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Rates
        /// <summary>
        /// POST API endpoint. 
        /// Returns forbidden if the current user is not an admin.
        /// Deactivates any existing rates in the same year with the same TF-code.
        /// </summary>
        /// <param name="Rate">The Rate to be posted.</param>
        /// <returns>The posted rate.</returns>
        [EnableQuery]
        public new IHttpActionResult Post(Rate Rate)
        {
            if (!CurrentUser.IsAdmin) return StatusCode(HttpStatusCode.Forbidden);
            _ratePostService.DeactivateExistingRate(Repo.AsQueryable(), Rate);
            return base.Post(Rate);
        }

        //PATCH: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Rate> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/Rates(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
        
        // GET: odata/Rates/Service.ThisYearsRates
        /// <summary>
        /// Returns Rates for the current year.
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IQueryable<Rate> ThisYearsRates()
        {
            var result = Repo.AsQueryable().Where(x => x.Year == (DateTime.Now).Year && x.Active);
            return result;
        }
    }
}
