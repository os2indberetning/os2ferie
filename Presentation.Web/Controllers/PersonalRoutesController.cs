using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class PersonalRoutesController : BaseController<PersonalRoute>
    {
        private readonly IPersonalRouteService _routeService;
        //GET: odata/PersonalRoutes
        public PersonalRoutesController(IGenericRepository<PersonalRoute> repository, IPersonalRouteService routeService, IGenericRepository<Person> personRepo)
            : base(repository, personRepo)
        {
            _routeService = routeService;
        }

        [EnableQuery]
        public IQueryable<PersonalRoute> Get(ODataQueryOptions<PersonalRoute> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/PersonalRoutes(5)
        public IQueryable<PersonalRoute> Get([FromODataUri] int key, ODataQueryOptions<PersonalRoute> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/PersonalRoutes(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/PersonalRoutes
        [EnableQuery]
        public new IHttpActionResult Post(PersonalRoute personalRoute)
        {
            return personalRoute.PersonId.Equals(CurrentUser.Id) ? (IHttpActionResult)Ok(_routeService.Create(personalRoute)) : Unauthorized();
        }

        //PATCH: odata/PersonalRoutes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            return Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId.Equals(CurrentUser.Id) ? base.Patch(key, delta) : Unauthorized();
        }

        //DELETE: odata/PersonalRoutes(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) ? base.Delete(key) : Unauthorized();
        }
    }
}
