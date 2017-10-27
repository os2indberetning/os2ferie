using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers.Vacation
{
    public class VacationBalanceController : BaseController<VacationBalance>
    {


        public VacationBalanceController(IGenericRepository<VacationBalance> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo)
        {
        }

        // GET: odata/VacationBalance
        [EnableQuery]
        public IQueryable<VacationBalance> Get(ODataQueryOptions<VacationBalance> queryOptions)
        {
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var currentYear = DateTime.Now.Year;
            if (Repo.AsQueryable().Any())
            {
                currentYear = Repo.AsQueryable().Max(y => y.Year);
            }
            
            var queryable =
                GetQueryable(queryOptions)
                    .Where(x => x.PersonId == CurrentUser.Id && x.Year == currentYear && (x.Employment.EndDateTimestamp == 0 || x.Employment.EndDateTimestamp >= currentTimestamp));

            return queryable;
        }


        //GET: odata/VacationBalance(5)
        public IQueryable<VacationBalance> Get([FromODataUri] int key, ODataQueryOptions<VacationBalance> queryOptions)
        {
            var res = GetQueryable(key, queryOptions);
            return res;
        }

        // PUT: odata/VacationBalance(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<VacationBalance> delta)
        {
            throw new NotSupportedException();
        }

        // POST: odata/VacationBalance
        [EnableQuery]
        public IHttpActionResult Post(VacationBalance vacationBalance, string emailText)
        {
            throw new NotSupportedException();
        }

        // PATCH: odata/VacationBalance(6)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<VacationBalance> delta, string emailText)
        {
            throw new NotSupportedException();
        }

        // DELETE: odata/VacationBalance(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            throw new NotSupportedException();
        }

        // GET: odata/VacationBalance/Service.VacationForUser
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of the employment</param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult VacationForEmployment(int id)
        {
            var currentYear = DateTime.Now.Year;

            if (Repo.AsQueryable().Any())
            {
                currentYear = Repo.AsQueryable().Max(y => y.Year);
            }
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var empl = Repo.AsQueryable().First(x => x.EmploymentId == id && x.Year == currentYear);
            return Ok(empl);
        }

        // GET: odata/VacationBalance/Service.VacationForUser
        /// <summary>
        /// Returns the people in the same organisation as the given employment.
        /// </summary>
        /// <param name="id">Id of the person</param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult VacationForEmployee(int id)
        {
            var currentYear = DateTime.Now.Year;

            if (Repo.AsQueryable().Any())
            {
                currentYear = Repo.AsQueryable().Max(y => y.Year);
            }
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var balances = Repo.AsQueryable().Where(x => x.PersonId == id && x.Year == currentYear && (x.Employment.EndDateTimestamp == 0 || x.Employment.EndDateTimestamp > currentTimestamp));
            VacationBalance totalBalance = new VacationBalance();

            foreach(var balance in balances)
            {

                totalBalance.FreeVacationHours += balance.FreeVacationHours;
                totalBalance.VacationHours += balance.VacationHours;
                totalBalance.TotalVacationHours += balance.TotalVacationHours;
                totalBalance.TransferredHours += balance.TransferredHours;
                
            }

            return Ok(totalBalance);
        }

    }
}
