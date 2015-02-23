using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class PersonController : BaseController<Person>
    {
        private IPersonService _person;

        public PersonController(IGenericRepository<Person> repo, IPersonService personService) : base(repo)
        {
            _person = personService;
        }

        // GET: odata/Person
        [EnableQuery]
        public IQueryable<Person> GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            return _person.ScrubCprFromPersons(GetQueryable(queryOptions));
        }

        //GET: odata/Person(5)
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            return _person.ScrubCprFromPersons(GetQueryable(key, queryOptions));
        }

        // PUT: odata/Person(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Person> delta)
        {
            return base.Put(key, delta);
        }

        // POST: odata/Person
        [EnableQuery]
        public new IHttpActionResult Post(Person person)
        {
            return base.Post(person);
        }

        // PATCH: odata/Person(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Person> delta)
        {
            return base.Patch(key, delta);
        }

        // DELETE: odata/Person(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
