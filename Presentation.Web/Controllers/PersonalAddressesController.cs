using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class PersonalAddressesController : BaseController<PersonalAddress>
    {
        
        //GET: odata/PersonalAddresses
        public PersonalAddressesController(IGenericRepository<PersonalAddress> repository) : base(repository){}

        [EnableQuery]
        public IQueryable<PersonalAddress> Get(ODataQueryOptions<PersonalAddress> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/PersonalAddresses(5)
        public IQueryable<PersonalAddress> Get([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/PersonalAddresses(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/PersonalAddresses
        [EnableQuery]
        public new IHttpActionResult Post(PersonalAddress personalAddress)
        {
            return base.Post(personalAddress);
        }

        //PATCH: odata/PersonalAddresses(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/PersonalAddresses(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
