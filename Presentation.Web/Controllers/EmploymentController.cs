using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class EmploymentsController : BaseController<Employment>
    {
        public EmploymentsController(IGenericRepository<Employment> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo){}
        
        //GET: odata/Employments
        [EnableQuery]
        public IQueryable<Employment> Get(ODataQueryOptions<Employment> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/Employments(5)
        public IQueryable<Employment> Get([FromODataUri] int key, ODataQueryOptions<Employment> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Employments(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Employment> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Employments
        [EnableQuery]
        public new IHttpActionResult Post(Employment Employment)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //PATCH: odata/Employments(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Employment> delta)
        {
            var firstOrDefault = Repo.AsQueryable().FirstOrDefault(x => x.Id == key);
            return firstOrDefault != null && firstOrDefault.PersonId.Equals(CurrentUser.Id) ? base.Patch(key, delta) : StatusCode(HttpStatusCode.Forbidden);
        }

        //DELETE: odata/Employments(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}