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
        public PersonalAddressesController(IGenericRepository<PersonalAddress> repository) : base(repository) { }

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
            return base.Patch(key, delta);
        }

        //DELETE: odata/PersonalAddresses(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
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
