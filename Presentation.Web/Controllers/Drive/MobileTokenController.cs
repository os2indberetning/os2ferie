using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers.Drive
{
    public class MobileTokenController : BaseController<MobileToken>
    {
        private readonly IMobileTokenService _tokenService;

        public MobileTokenController(IGenericRepository<MobileToken> repo, IMobileTokenService tokenService, IGenericRepository<Person> personRepo)
            : base(repo, personRepo)
        {
            _tokenService = tokenService;
        }

        //GET: odata/MobileTokens
        /// <summary>
        /// ODATA GET API endpoint for MobileTokens
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>MobileTokens</returns>
        [EnableQuery]
        public IQueryable<MobileToken> Get(ODataQueryOptions<MobileToken> queryOptions)
        {
            return new List<MobileToken>().AsQueryable();
        }

        //GET: odata/MobileTokens(5)
        /// <summary>
        /// GET API endpoint for MobileTokens
        /// </summary>
        /// <param name="key">Returns MobileTokens belonging to the user identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>MobileTokens</returns>
        public IQueryable<MobileToken> Get([FromODataUri] int key, ODataQueryOptions<MobileToken> queryOptions)
        {
            return _tokenService.GetByPersonId(key);
        }

        //PUT: odata/MobileTokens(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/MobileTokens
        /// <summary>
        /// POST API endpoint for MobileTokens.
        /// Returns forbidden if the user associated with the token is not the current user.
        /// </summary>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IHttpActionResult Post(MobileToken mobileToken)
        {
            if (CurrentUser.Id.Equals(mobileToken.PersonId))
            {
                return Created(_tokenService.Create(mobileToken));
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        //PATCH: odata/MobileTokens(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<MobileToken> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        //DELETE: odata/MobileTokens(5)
        /// <summary>
        /// DELETE API endpoint for MobileTokens.
        /// Returns firbidden if the user associated with the MobileToken is not the current user.
        /// </summary>
        /// <param name="key">Deletes the MobileToken identified by key</param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) ? base.Delete(key) : StatusCode(HttpStatusCode.Forbidden);
        }
    }
}
