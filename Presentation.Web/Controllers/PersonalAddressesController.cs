using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class PersonalAddressesController : BaseController<PersonalAddress>
    {

        //GET: odata/PersonalAddresses
        public PersonalAddressesController(IGenericRepository<PersonalAddress> repository, IGenericRepository<Person> personRepo) : base(repository, personRepo) { }

        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return Ok(GetQueryable(queryOptions));
        }

        //GET: odata/PersonalAddresses(5)
        public IHttpActionResult Get([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return Ok(GetQueryable(key, queryOptions));

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
            if (!CurrentUser.Id.Equals(personalAddress.PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var coordinates = NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>();
            var result = coordinates.GetAddressCoordinates(personalAddress);
            personalAddress.Latitude = result.Latitude;
            personalAddress.Longitude = result.Longitude;
            return base.Post(personalAddress);
        }

        //PATCH: odata/PersonalAddresses(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<PersonalAddress> delta)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) || CurrentUser.IsAdmin ? base.Patch(key, delta) : Unauthorized();
        }

        //DELETE: odata/PersonalAddresses(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) || CurrentUser.IsAdmin ? base.Delete(key) : Unauthorized();
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeHome
        public IHttpActionResult GetAlternativeHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var res = Repo.AsQueryable().FirstOrDefault(x => x.PersonId == personId && x.Type == PersonalAddressType.AlternativeHome);
            return res == null ? (IHttpActionResult) Ok() : Ok(res);
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeHome
        public IHttpActionResult GetRealHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var res = Repo.AsQueryable().FirstOrDefault(x => x.PersonId == personId && x.Type == PersonalAddressType.Home);
            return res == null ? (IHttpActionResult)Ok() : Ok(res);
        }

        //GET odata/PersonalAddresses/Service.GetAlternativeHome?personId=1
        public IHttpActionResult GetHome(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var addresses =
                Repo.AsQueryable()
                    .Where(
                        x =>
                            (x.Type == PersonalAddressType.Home || x.Type == PersonalAddressType.AlternativeHome)
                            && x.PersonId == personId);


            var res = new List<PersonalAddress>
            {
                addresses.Any(x => x.Type == PersonalAddressType.AlternativeHome)
                    ? addresses.First(x => x.Type == PersonalAddressType.AlternativeHome)
                    : addresses.First(x => x.Type == PersonalAddressType.Home)
            };

            return Ok(res.AsQueryable());
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeWork
        public IHttpActionResult GetAlternativeWork([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            if (!CurrentUser.Id.Equals(key) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            return Ok(GetQueryable(queryOptions).Where(x => x.PersonId == key && x.Type == PersonalAddressType.AlternativeWork));
        }
    }
}
