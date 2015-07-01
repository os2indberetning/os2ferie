using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class CoordinateController : ApiController
    {
        private readonly IAddressCoordinates _coordinates;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        public CoordinateController(IAddressCoordinates coordinates)
        {
            _coordinates = coordinates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public IHttpActionResult SetCoordinatesOnAddressList(IEnumerable<Address> addresses)
        {
            var result = addresses.Select(address => _coordinates.GetAddressCoordinates(address,true)).ToList();
            return Ok(result);
        }


    }
}