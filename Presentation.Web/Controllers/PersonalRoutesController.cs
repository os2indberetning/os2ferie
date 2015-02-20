using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class PersonalRoutesController : BaseController<PersonalRoute>
    {
        //GET: odata/PersonalRoutes
        public PersonalRoutesController(IGenericRepository<PersonalRoute> repository) : base(repository){}

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
        public new IHttpActionResult Post(PersonalRoute PersonalRoute)
        {
            return base.Post(PersonalRoute);
        }

        //PATCH: odata/PersonalRoutes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<PersonalRoute> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/PersonalRoutes(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
