using System;
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
using Core.DomainServices.RoutingClasses;
using Infrastructure.DataAccess;
using log4net.Repository.Hierarchy;

namespace OS2Indberetning.Controllers
{
    public class PersonController : BaseController<Person>
    {
        private IPersonService _person;
        private readonly IGenericRepository<Employment> _employmentRepo = new GenericRepository<Employment>(new DataContext());
        private readonly IGenericRepository<LicensePlate> _licensePlateRepo = new GenericRepository<LicensePlate>(new DataContext());

        public PersonController(IGenericRepository<Person> repo, IPersonService personService, IGenericRepository<Employment> employmentRepo, IGenericRepository<LicensePlate> licensePlateRepo)
            : base(repo, repo)
        {
            _person = personService;
            _employmentRepo = employmentRepo;
            _licensePlateRepo = licensePlateRepo;
        }

        // GET: odata/Person
        [EnableQuery]
        public IHttpActionResult GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _person.ScrubCprFromPersons(res);
            _person.AddFullName(res);
            return Ok(res);
        }

        [EnableQuery]
        public Person GetCurrentUser()
        {
            var employments = _employmentRepo.AsQueryable().Where(x => x.PersonId.Equals(CurrentUser.Id));
            CurrentUser.Employments = employments.ToList();
            CurrentUser.FullName = CurrentUser.FirstName + " " + CurrentUser.LastName + " [" +  CurrentUser.Initials + "]";
            return CurrentUser;
        }

        //GET: odata/Person(5)
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            try
            {
                var cprScrubbed = _person.ScrubCprFromPersons(GetQueryable(key, queryOptions));
                _person.AddFullName(cprScrubbed);
                var res = cprScrubbed.ToList();

                res[0].DistanceFromHomeToWork = _person.GetDistanceFromHomeToWork(res[0]);

                return res.AsQueryable();
            }
            catch (RouteInformationException e)
            {
                throw new Exception("Kunne ikke beregne rute mellem hjemme- og arbejdsadresse.", e);
            }
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
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // PATCH: odata/Person(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Person> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // DELETE: odata/Person(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // GET odata/Person(5)/Employments
        public IQueryable<Employment> GetEmployments([FromODataUri] int key)
        {
            var result = _employmentRepo.AsQueryable().Where(x => x.PersonId == key);

            return result.AsQueryable();
        }

        // GET: odata/Person(5)/Service.HasLicensePlate
        [EnableQuery]
        [HttpGet]
        public IHttpActionResult HasLicensePlate([FromODataUri] int key, ODataActionParameters parameters)
        {
            return Ok(_licensePlateRepo.AsQueryable().Any(x => x.PersonId == key));
        }
    }
}
