using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace OS2Indberetning.Controllers
{
    public class PersonController : BaseController<Person>
    {

        private readonly IGenericRepository<Employment> _employmentRepo = new GenericRepository<Employment>(new DataContext());
        private readonly IGenericRepository<LicensePlate> _licensePlateRepo = new GenericRepository<LicensePlate>(new DataContext());


        // GET: odata/Person
        [EnableQuery]
        public IQueryable<Person> GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            return GetQueryable(queryOptions);
        }

        //GET: odata/Person(5)
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            return GetQueryable(key, queryOptions);
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

        // GET odata/Person(5)/Employments
        public IQueryable<Employment> GetEmployments([FromODataUri] int key)
        {
            var result = _employmentRepo.AsQueryable().Where(x => x.PersonId == key);

            return result.AsQueryable();
        }

        // GET: odata/Person(5)/PersonService.HasLicensePlate
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult HasLicensePlate([FromODataUri] int key , ODataActionParameters parameters)
        {
            return Ok(_licensePlateRepo.AsQueryable().Any(x => x.PersonId == key));
        }
    }
}
