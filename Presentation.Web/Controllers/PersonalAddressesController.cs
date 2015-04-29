using System.Collections.Generic;
using System.Linq;
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
            var res = GetQueryable(queryOptions);

            if (CurrentUser.IsAdmin || !res.Any(x => !x.PersonId.Equals(CurrentUser.Id)))
            {
                return Ok(res);
            }
            return Unauthorized();
        }

        //GET: odata/PersonalAddresses(5)
        public IHttpActionResult Get([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            var res = GetQueryable(key, queryOptions);
            if (CurrentUser.IsAdmin || !res.Any(x => !x.PersonId.Equals(CurrentUser.Id)))
            {
                return Ok(res);
            }
            return Unauthorized();
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
                return Unauthorized();
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
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) ? base.Patch(key, delta) : Unauthorized();
        }

        //DELETE: odata/PersonalAddresses(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.Id.Equals(Repo.AsQueryable().Single(x => x.Id.Equals(key)).PersonId) ? base.Delete(key) : Unauthorized();
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeHome
        public IQueryable<PersonalAddress> GetAlternativeHome([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return GetQueryable(queryOptions).Where(x => x.PersonId == key && x.Type == PersonalAddressType.AlternativeHome);
        }

        //GET odata/PersonalAddresses/Service.GetAlternativeHome?personId=1
        public IQueryable<PersonalAddress> GetHome(int personId)
        {
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

            return res.AsQueryable();
        }

        //GET odata/PersonalAddresses(5)/GetAlternativeWork
        public IQueryable<PersonalAddress> GetAlternativeWork([FromODataUri] int key, ODataQueryOptions<PersonalAddress> queryOptions)
        {
            return GetQueryable(queryOptions).Where(x => x.PersonId == key && x.Type == PersonalAddressType.AlternativeWork);
        }
    }
}
