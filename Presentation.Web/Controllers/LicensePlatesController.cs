using System.ComponentModel;
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
    public class LicensePlatesController : BaseController<LicensePlate>
    {
        private readonly ILicensePlateService _plateService;

        public LicensePlatesController(IGenericRepository<LicensePlate> repo, ILicensePlateService plateService, IGenericRepository<Person> personRepo)
            : base(repo, personRepo)
        {
            _plateService = plateService;
        }

        //GET: odata/LicensePlates
        [EnableQuery]
        public IQueryable<LicensePlate> Get(ODataQueryOptions<LicensePlate> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/LicensePlates(5)
        public IQueryable<LicensePlate> Get([FromODataUri] int key, ODataQueryOptions<LicensePlate> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/LicensePlates(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/LicensePlates
        [EnableQuery]
        public new IHttpActionResult Post(LicensePlate LicensePlate)
        {
            if (!CurrentUser.Id.Equals(LicensePlate.PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }


            if (!Repo.AsQueryable().Any(lp => lp.PersonId == LicensePlate.PersonId))
            {
                LicensePlate.IsPrimary = true;
            }

            return base.Post(LicensePlate);
        }

        //PATCH: odata/LicensePlates(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<LicensePlate> delta)
        {
            if (!CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var primary = new object();
            if (delta.TryGetPropertyValue("IsPrimary", out primary) && (bool)primary)
            {
                _plateService.MakeLicensePlatePrimary(key);
            }
            return base.Patch(key, delta);
        }

        //DELETE: odata/LicensePlates(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            // Get the plate to be deleted
            var plate = Repo.AsQueryable().SingleOrDefault(lp => lp.Id == key);

            if (!CurrentUser.Id.Equals(plate.PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (plate != null && plate.IsPrimary)
            {
                // Delete the plate. Save the result.
                var res = base.Delete(key);
                // Find a new plate to make primary.
                var newPrimary = Repo.AsQueryable().FirstOrDefault(lp => lp.PersonId == plate.PersonId);
                if (newPrimary != null)
                {
                    _plateService.MakeLicensePlatePrimary(newPrimary.Id);
                }
                // Make the new plate primary and return the result of the delete action.
                return res;
            }

            return base.Delete(key);
        }

        public IHttpActionResult MakePrimary(int plateId)
        {
            if (!CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(plateId)).PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (_plateService.MakeLicensePlatePrimary(plateId))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}