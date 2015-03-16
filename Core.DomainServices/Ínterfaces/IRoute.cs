using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices.RoutingClasses;

namespace Core.DomainServices.Ínterfaces
{
    public interface IRoute
    {
        /// <summary>
        /// Returns a route for a set of addresses.
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        RouteInformation GetRoute(IEnumerable<Address> addresses);
    }
}
