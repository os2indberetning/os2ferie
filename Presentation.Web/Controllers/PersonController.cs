using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
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
        /// <summary>
        /// GET API endpoint for Person
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>People</returns>
        [EnableQuery]
        public IHttpActionResult GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _person.ScrubCprFromPersons(res);
            return Ok(res);
        }

        
        /// <summary>
        /// GET API endpoint for CurrentUser.
        /// Sets HomeWorkDistance on each of the users employments.
        /// Strips CPR-number off.
        /// </summary>
        /// <returns>The user currently logged in.</returns>
        [EnableQuery(MaxExpansionDepth = 4)]
        // Disable caching.
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public Person GetCurrentUser()
        {
            var employments = _employmentRepo.AsQueryable().Where(x => x.PersonId.Equals(CurrentUser.Id));
            CurrentUser.Employments = employments.ToList();
            _person.AddHomeWorkDistanceToEmployments(CurrentUser);
            CurrentUser.CprNumber = "";
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            CurrentUser.IsSubstitute = _substituteRepo.AsQueryable().Any(x => x.SubId.Equals(CurrentUser.Id) && x.StartDateTimestamp < currentDateTimestamp && x.EndDateTimestamp > currentDateTimestamp);
            return CurrentUser;
        }

        //GET: odata/Person(5)
        /// <summary>
        /// GET API endpoint for a single person
        /// Strips CPR-number off.
        /// </summary>
        /// <param name="key">Returns the person identified by key</param>
        /// <param name="queryOptions"></param>
        /// <returns>A single Person</returns>
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            try
            {
                var cprScrubbed = _person.ScrubCprFromPersons(GetQueryable(key, queryOptions));
                var res = cprScrubbed.ToList();
                return res.AsQueryable();
            }
            catch (RouteInformationException e)
            {
                throw new Exception("Kunne ikke beregne rute mellem hjemme- og arbejdsadresse.", e);
            }
        }

        // PUT: odata/Person(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Person> delta)
        {
            return base.Put(key, delta);
        }

        // POST: odata/Person
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [EnableQuery]
        public new IHttpActionResult Post(Person person)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // PATCH: odata/Person(5)
        /// <summary>
        /// PATCH API endpoint for person.
        /// Returns forbidden if the person identified by key is not the current user or if the current user is not an admin.
        /// </summary>
        /// <param name="key">Patches the Person identified by key</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [System.Web.Http.AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Person> delta)
        {
            var person = Repo.AsQueryable().Single(x => x.Id == key);
            return CurrentUser.IsAdmin || CurrentUser.Id == person.Id ? base.Patch(key, delta) : StatusCode(HttpStatusCode.Forbidden);
        }

        // DELETE: odata/Person(5)
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // GET odata/Person(5)/Employments
        /// <summary>
        /// Returns employments belonging to the user identified by key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>Employments</returns>
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
        /// <summary>
        /// Returns whether or not the user identified by key has a license plate.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [EnableQuery]
        [System.Web.Http.HttpGet]
        public IHttpActionResult HasLicensePlate([FromODataUri] int key, ODataActionParameters parameters)
        {
            return Ok(_licensePlateRepo.AsQueryable().Any(x => x.PersonId == key));
        }
    }
}
