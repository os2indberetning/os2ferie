using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [EnableQuery]
        public IQueryable<Substitute> Get(ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //GET: odata/Substitutes(5)
        [EnableQuery]
        public IQueryable<Substitute> Get([FromODataUri] int key, ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(key, queryOptions);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //PUT: odata/Substitutes(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Substitute> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Substitutes
        [EnableQuery]
        public new IHttpActionResult Post(Substitute Substitute)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Substitute.LeaderId))
            {
                return base.Post(Substitute);
            }
            return StatusCode(HttpStatusCode.Forbidden);

        }

        //PATCH: odata/Substitutes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Substitute> delta)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).LeaderId))
            {
                return base.Patch(key, delta);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        //DELETE: odata/Substitutes(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            if (CurrentUser.IsAdmin || CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).LeaderId))
            {
                return base.Delete(key);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        // GET: odata/Substitutes/SubstituteService.Personal
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult Personal()
        {
            var res = Repo.AsQueryable().Where(x => x.Person.Id != x.LeaderId);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return Ok(res);
        }

        // GET: odata/Substitutes/SubstituteService.Substitute
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult Substitute()
        {
            var res = Repo.AsQueryable().Where(x => x.Person.Id == x.LeaderId);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return Ok(res);
        }
    }
}
