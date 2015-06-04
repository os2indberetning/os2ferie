using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class SubstitutesController : BaseController<Substitute>
    {
        private ISubstituteService _sub;

        //GET: odata/Substitutes
        public SubstitutesController(IGenericRepository<Substitute> repository, ISubstituteService sub, IGenericRepository<Person> personRepo)
            : base(repository, personRepo)
        {
            _sub = sub;
        }

        /// <summary>
        /// GET API endpoint for Substitutes.
        /// Strips CPR-number off.
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Substitutes.</returns>
        [EnableQuery]
        public IQueryable<Substitute> Get(ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //GET: odata/Substitutes(5)
        /// <summary>
        /// GET API endpoint for a single Substitute.
        /// Strips CPR-number off.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>A single Substitute.</returns>
        [EnableQuery]
        public IQueryable<Substitute> Get([FromODataUri] int key, ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(key, queryOptions);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //PUT: odata/Substitutes(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Substitute> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Substitutes
        /// <summary>
        /// POST API endpoint for Substitutes.
        /// Returns forbidden if the current user is not an admin or if the current user is not the leader of the substitute.
        /// Returns BadRequest if a substitute for the same person or OrgUnit already exists in an overlapping time period.
        /// </summary>
        /// <param name="Substitute">The substitute to be posted</param>
        /// <returns>The posted substitute</returns>
        [EnableQuery]
        public new IHttpActionResult Post(Substitute Substitute)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Substitute.LeaderId))
            {
                Substitute.StartDateTimestamp = _sub.GetStartOfDayTimestamp(Substitute.StartDateTimestamp);
                if (Substitute.EndDateTimestamp != 9999999999)
                {
                    Substitute.EndDateTimestamp = _sub.GetEndOfDayTimestamp(Substitute.EndDateTimestamp);
                }

                // Return BadRequest if a sub or personal approver already exists in the time period. Otherwise create the sub or approver.
                return !_sub.CheckIfNewSubIsAllowed(Substitute) ? BadRequest() : base.Post(Substitute);
            }
            return StatusCode(HttpStatusCode.Forbidden);

        }

        //PATCH: odata/Substitutes(5)
        /// <summary>
        /// PATCH API endpoint for Substitutes.
        /// Returns Forbidden if the current user is not an admin or if the current user is not the leader of the substitute.
        /// Returns BadRequest if a substitute for the same person or OrgUnit already exists in an overlapping time period.
        /// </summary>
        /// <param name="key">Patches the Substitute identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Substitute> delta)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).LeaderId))
            {
                // Get sub that is being patched, so we can check if it is allowed.
                var patchedSub = Repo.AsQueryable().First(x => x.Id.Equals(key));

                var startStamp = new object();
                if (delta.TryGetPropertyValue("StartDateTimestamp", out startStamp))
                {
                    var startOfDayStamp = _sub.GetStartOfDayTimestamp((long)startStamp);
                    delta.TrySetPropertyValue("StartDateTimestamp", startOfDayStamp);
                    // Set the new timestamp on the temporary sub to check if it is allowed.
                    patchedSub.StartDateTimestamp = startOfDayStamp;
                }

                var endStamp = new object();
                if (delta.TryGetPropertyValue("EndDateTimestamp", out endStamp))
                {
                    patchedSub.EndDateTimestamp = (long)endStamp;
                    // If endstamp is 9999999999 it means it is unlimited.
                    if ((long)endStamp != 9999999999)
                    {
                        var endOfDayStamp = _sub.GetEndOfDayTimestamp((long)endStamp);
                        delta.TrySetPropertyValue("EndDateTimestamp", endOfDayStamp);
                        patchedSub.EndDateTimestamp = endOfDayStamp;
                    }
                }

                // Check if the patch is allowed.
                return !_sub.CheckIfNewSubIsAllowed(patchedSub) ? BadRequest() : base.Patch(key,delta);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        //DELETE: odata/Substitutes(5)
        /// <summary>
        /// DELETE API endpoint for Substitutes.
        /// Returns Forbidden if the current user is not an admin or if the current user is not the leader of the Substitute.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).LeaderId))
            {
                return base.Delete(key);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        // GET: odata/Substitutes/SubstituteService.Personal
        /// <summary>
        /// Returns Personal Approvers
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult Personal()
        {
            //PersonId != LeaderId means it is a Personal Approver.
            var res = Repo.AsQueryable().Where(x => x.Person.Id != x.LeaderId);
            _sub.ScrubCprFromPersons(res);
            return Ok(res);
        }

        // GET: odata/Substitutes/SubstituteService.Substitute
        /// <summary>
        /// Returns Substitutes for OrgUnits.
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult Substitute()
        {
            //PersonId == LeaderId means it is a Substitute
            var res = Repo.AsQueryable().Where(x => x.Person.Id == x.LeaderId);
            _sub.ScrubCprFromPersons(res);
            return Ok(res);
        }
    }
}
