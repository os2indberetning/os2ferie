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
        private readonly IGenericRepository<Substitute> _substituteRepo;

        public PersonController(IGenericRepository<Person> repo, IPersonService personService, IGenericRepository<Employment> employmentRepo, IGenericRepository<LicensePlate> licensePlateRepo, IGenericRepository<Substitute> substituteRepo)
            : base(repo, repo)
        {
            _person = personService;
            _employmentRepo = employmentRepo;
            _licensePlateRepo = licensePlateRepo;
            _substituteRepo = substituteRepo;
        }

        // GET: odata/Person
        [EnableQuery]
        public IHttpActionResult GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _person.ScrubCprFromPersons(res);
            _person.AddFullName(res);
            _person.AddHomeWorkDistanceToEmployments(res);
            return Ok(res);
        }

        [EnableQuery(MaxExpansionDepth = 4)]
        public Person GetCurrentUser()
        {
            var employments = _employmentRepo.AsQueryable().Where(x => x.PersonId.Equals(CurrentUser.Id));
            CurrentUser.Employments = employments.ToList();
            _person.AddHomeWorkDistanceToEmployments(CurrentUser);
            CurrentUser.CprNumber = "";
            CurrentUser.FullName = CurrentUser.FirstName + " " + CurrentUser.LastName + " [" + CurrentUser.Initials + "]";
            CurrentUser.IsSubstitute = _substituteRepo.AsQueryable().Any(x => x.SubId.Equals(CurrentUser.Id));
            return CurrentUser;
        }

        //GET: odata/Person(5)
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            try
            {
                var cprScrubbed = _person.ScrubCprFromPersons(GetQueryable(key, queryOptions));
                _person.AddFullName(cprScrubbed);
                _person.AddHomeWorkDistanceToEmployments(cprScrubbed);
                var res = cprScrubbed.ToList();

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
            var person = Repo.AsQueryable().Single(x => x.Id == key);
            return CurrentUser.IsAdmin || CurrentUser.Id == person.Id ? base.Patch(key, delta) : StatusCode(HttpStatusCode.Forbidden);
        }

        // DELETE: odata/Person(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // GET odata/Person(5)/Employments
        [EnableQuery]
        public IHttpActionResult GetEmployments([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            var person = Repo.AsQueryable().FirstOrDefault(x => x.Id.Equals(key));
            if (person == null)
            {
                return BadRequest("Der findes ingen person med id " + key);
            }
            person = _person.AddHomeWorkDistanceToEmployments(person);


            return Ok(person.Employments);
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
