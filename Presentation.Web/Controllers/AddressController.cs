using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Routing;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using log4net;
using Microsoft.Owin.Security.Provider;
using Ninject;

namespace OS2Indberetning.Controllers
{


    public class AddressesController : BaseController<Address>
    {
        private static Address MapStartAddress { get; set; }

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //GET: odata/Addresses
        public AddressesController(IGenericRepository<Address> repository, IGenericRepository<Person> personRepo) : base(repository, personRepo) { }

        [EnableQuery]
        public IQueryable<Address> Get(ODataQueryOptions<Address> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        public Address GetMapStart()
        {
            if (MapStartAddress == null)
            {
                var coordinates = NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>();
                MapStartAddress = new Address
                {
                    StreetName = ConfigurationManager.AppSettings["MapStartStreetName"],
                    StreetNumber = ConfigurationManager.AppSettings["MapStartStreetNumber"],
                    ZipCode = int.Parse(ConfigurationManager.AppSettings["MapStartZipCode"]),
                    Town = ConfigurationManager.AppSettings["MapStartTown"],
                };

                MapStartAddress = coordinates.GetAddressCoordinates(MapStartAddress);
            }
            return MapStartAddress;
        }

        //GET: odata/Addresses(5)
        public IQueryable<Address> Get([FromODataUri] int key, ODataQueryOptions<Address> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        [EnableQuery]
        public IQueryable<Address> SetCoordinatesOnAddress(Address address)
        {
            var coordinates = NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>();
            var result = coordinates.GetAddressCoordinates(address);
            var list = new List<Address>()
            {
                result
            }.AsQueryable();
            return list;
        }

        //PUT: odata/Addresses(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Address> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Addresses
        [EnableQuery]
        public new IHttpActionResult Post(Address Address)
        {
            if (Address is Point || Address is DriveReportPoint)
            {
                return base.Post(Address);
            }
            if (CurrentUser.IsAdmin)
            {
                return base.Post(Address);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        //PATCH: odata/Addresses(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Address> delta)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            if (addr is Point || addr is DriveReportPoint)
            {
                return base.Patch(key, delta);
            }
            if (CurrentUser.IsAdmin)
            {
                return base.Patch(key, delta);
            }
            return StatusCode(HttpStatusCode.Forbidden);

        }

        //DELETE: odata/Addresses(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            if (addr is Point || addr is DriveReportPoint)
            {
                return base.Delete(key);
            }
            if (CurrentUser.IsAdmin)
            {
                return base.Delete(key);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        [EnableQuery]
        public IHttpActionResult GetPersonalAndStandard(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var rep = Repo.AsQueryable();
            var temp = rep.Where(elem => !(elem is DriveReportPoint || elem is Point));
            var res = new List<Address>();
            foreach (var address in temp)
            {
                if (address is PersonalAddress && ((PersonalAddress)address).PersonId == personId)
                {
                    res.Add(address);
                }
                else if (!(address is PersonalAddress))
                {
                    res.Add(address);
                }
            }

            return Ok(res.AsQueryable());
        }

        [EnableQuery]
        public IQueryable<Address> GetStandard()
        {
            var rep = Repo.AsQueryable();
            var res = rep.Where(elem => !(elem is DriveReportPoint || elem is Point)).Where(elem => !(elem is PersonalAddress));
            return res.AsQueryable();
        }
    }
}
