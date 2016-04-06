using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers.Drive
{
    public class AppLoginController : BaseController<AppLogin>
    {

        private IAppLoginService _loginService;

        public AppLoginController(IGenericRepository<AppLogin> repo, IGenericRepository<Person> personRepo, IAppLoginService loginService) : base(repo, personRepo)
        {
            _loginService = loginService;
        }

        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<AppLogin> queryOptions)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        public IHttpActionResult Get([FromODataUri] int key, ODataQueryOptions<AppLogin> queryOptions)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        public new IHttpActionResult Put([FromODataUri] int key, Delta<AppLogin> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [EnableQuery]
        public new IHttpActionResult Post(AppLogin AppLogin)
        {
            var prepared = _loginService.PrepareAppLogin(AppLogin);
            Repo.Insert(prepared);
            Repo.Save();
            _loginService.SyncToDmz(prepared);
            return Ok();
        }

        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<AppLogin> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            var toDelete = Repo.AsQueryable().Where(x => x.PersonId == key).ToList();
            Repo.DeleteRange(toDelete);
            Repo.Save();
            _loginService.RemoveFromDmz(key);
            return Ok();
        }
    }
}