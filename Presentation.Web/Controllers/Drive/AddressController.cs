using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices.Interfaces;
using Ninject;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace OS2Indberetning.Controllers.Drive
{
    public class AddressesController : BaseController<Address>
    {
        private readonly IGenericRepository<Employment> _employmentRepo;
        private readonly IAddressLaunderer _launderer;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<CachedAddress> _cachedAddressRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private static Address MapStartAddress { get; set; }

        //GET: odata/Addresses
        public AddressesController(IGenericRepository<Address> repository, IGenericRepository<Person> personRepo, IGenericRepository<Employment> employmentRepo, IAddressLaunderer launderer, IAddressCoordinates coordinates, IGenericRepository<CachedAddress> cachedAddressRepo, IGenericRepository<PersonalAddress> personalAddressRepo)
            : base(repository, personRepo)
        {
            _employmentRepo = employmentRepo;
            _launderer = launderer;
            _coordinates = coordinates;
            _cachedAddressRepo = cachedAddressRepo;
            _personalAddressRepo = personalAddressRepo;
        }

        /// <summary>
        /// ODATA GET api endpoint for addresses
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns>Addresses</returns>
        [EnableQuery]
        public IQueryable<Address> Get(ODataQueryOptions<Address> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            return res;
        }

        /// <summary>
        /// API endpoint for getting the starting address of the map in the frontend.
        /// </summary>
        /// <returns>Starting address of frontend map</returns>
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
        /// <summary>
        /// ODATA GET api endpoint for a single address
        /// </summary>
        /// <param name="key"></param>
        /// <param name="queryOptions"></param>
        /// <returns>An address</returns>
        public IQueryable<Address> Get([FromODataUri] int key, ODataQueryOptions<Address> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/Addresses(5)
        /// <summary>
        /// Is not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Address> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Addresses
        /// <summary>
        /// ODATA POST api endpoint for addresses.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns>The posted object</returns>
        [EnableQuery]
        public new IHttpActionResult Post(Address Address)
        {
            return base.Post(Address);
        }

        //PATCH: odata/Addresses(5)
        /// <summary>
        /// ODATA PATCH api endpoint for addresses.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Address> delta)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            return base.Patch(key, delta);

        }

        //DELETE: odata/Addresses(5)
        /// <summary>
        /// DELETE API Endpoint. Deletes the entity identified by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            var addr = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (addr == null)
            {
                return NotFound();
            }
            return base.Delete(key);
        }

        /// <summary>
        /// Returns personal and standard addresses for the user identified by personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Personal and standard addresses</returns>
        [EnableQuery]
        public IHttpActionResult GetPersonalAndStandard(int personId)
        {
            if (!CurrentUser.Id.Equals(personId) && !CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var rep = Repo.AsQueryable();
            // Select all standard addresses.
            var addresses = rep.Where(elem => !(elem is DriveReportPoint || elem is Point || elem is CachedAddress || elem is WorkAddress || elem is PersonalAddress)).ToList();
            // Add personal addresses to addresses.
            addresses.AddRange(_personalAddressRepo.AsQueryable().Where(elem => (elem.PersonId.Equals(personId)) && elem.Type != PersonalAddressType.OldHome));


            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var employments = _employmentRepo.AsQueryable().Where(x => x.PersonId.Equals(personId)
                                                                            && x.StartDateTimestamp < currentTimestamp 
                                                                            && (x.EndDateTimestamp > currentTimestamp ||x.EndDateTimestamp == 0))
                                                                  .ToList();
            // Add the workAddress of each of the user's employments.
            foreach (var empl in employments)
            {
                var tempAddr = empl.OrgUnit.Address;
                tempAddr.Description = empl.OrgUnit.LongDescription;
                addresses.Add(tempAddr);
            }

            return Ok(addresses.AsQueryable());
        }

        /// <summary>
        /// Returns CachedAddresses for address cleaning in the admin view.
        /// A clean address is an address on which a coordinate lookup was performed successfully.
        /// By default, it will only return addresses that could not be looked up.
        /// </summary>
        /// <param name="includeCleanAddresses">if includeCleanAddresses is true it will also return the clean ones.</param>
        /// <returns>Addresses for which coordinate lookup failed.</returns>
        [EnableQuery]
        public IHttpActionResult GetCachedAddresses(bool includeCleanAddresses = false)
        {
            if (CurrentUser.IsAdmin)
            {
                var repo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<CachedAddress>>();
                if (!includeCleanAddresses)
                {
                    var res = repo.AsQueryable().Where(x => x.IsDirty);
                    return Ok(res);
                }
                return Ok(repo.AsQueryable());
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Returns standard addresses
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public IQueryable<Address> GetStandard()
        {
            var rep = Repo.AsQueryable();
            var res = rep.Where(elem => !(elem is DriveReportPoint || elem is Point)).Where(elem => !(elem is PersonalAddress || elem is WorkAddress || elem is CachedAddress));
            return res.AsQueryable();
        }

        /// <summary>
        /// Receives an address from the address cleaning view of the admin page.
        /// The address is changed and a new coordinate lookup is performed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>IHttpActionResult</returns>
        [EnableQuery]
        public IHttpActionResult AttemptCleanCachedAddress(Address input)
        {
            try
            {
                var cleanAddress = _launderer.Launder(input);
                cleanAddress = _coordinates.GetAddressCoordinates(cleanAddress, true);
                var cachedAddr = _cachedAddressRepo.AsQueryable().Single(x => x.Id.Equals(input.Id));
                cachedAddr.Latitude = cleanAddress.Latitude;
                cachedAddr.Longitude = cleanAddress.Longitude;
                cachedAddr.StreetName = cleanAddress.StreetName;
                cachedAddr.StreetNumber = cleanAddress.StreetNumber;
                cachedAddr.ZipCode = cleanAddress.ZipCode;
                cachedAddr.Town = cleanAddress.Town;
                cachedAddr.IsDirty = false;
                _cachedAddressRepo.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
        }
    }
}
