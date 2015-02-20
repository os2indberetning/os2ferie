using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
namespace OS2Indberetning.Controllers
{
    public class MobileTokenController : BaseController<MobileToken>
    {
        private IMobileTokenService _tokenService;

        public MobileTokenController(IGenericRepository<MobileToken> repo, IMobileTokenService tokenService)
            : base(repo)
        {
            _tokenService = tokenService;
        }
        
        //GET: odata/MobileTokens
        [EnableQuery]
        public IQueryable<MobileToken> Get(ODataQueryOptions<MobileToken> queryOptions)
        {
            return new List<MobileToken>().AsQueryable();
        }

        //GET: odata/MobileTokens(5)
        public IQueryable<MobileToken> Get([FromODataUri] int key, ODataQueryOptions<MobileToken> queryOptions)
        {
            return _tokenService.GetByPersonId(key);
        }

        //PUT: odata/MobileTokens(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/MobileTokens
        [EnableQuery]
        public new IHttpActionResult Post(MobileToken mobileToken)
        {
            return Created(_tokenService.Create(mobileToken));
        }

        //PATCH: odata/MobileTokens(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/MobileTokens(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
