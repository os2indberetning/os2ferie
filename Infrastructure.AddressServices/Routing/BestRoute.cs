using System.Collections.Generic;
using System.Linq;
using Infrastructure.AddressServices.Classes;
using Infrastructure.AddressServices.Interfaces;

namespace Infrastructure.AddressServices.Routing
{
    public class BestRoute : IRoute
    {
        /// <summary>
        /// Returns the shortest route within the time limit. (Duration <= 300s , Length difference > 3000m)
        /// </summary>
        /// <param name="addresses"></param>
        /// <exception cref="AddressLaunderingException"></exception>
        /// <exception cref="AddressCoordinatesException"></exception>
        /// <exception cref="RouteInformationException"></exception>
        /// <returns></returns>
        public RouteInformation GetRoute(IEnumerable<Address> addresses)
        {
            if (addresses == null)
            {
                return null;
            }

            List<Coordinates> routeCoordinates = addresses.Select(address => AddressCoordinates.GetAddressCoordinates(address, address.Type)).ToList();

            List<RouteInformation> routes = SeptimaRouter.GetRoute(routeCoordinates).OrderBy(x => x.Duration).ToList();

            RouteInformation bestRoute = routes[0];

            foreach (var route in routes)
            {
                bool betterRoute = (route.Duration - bestRoute.Duration <= 300) && (bestRoute.Length - route.Length > 3000);
                if (betterRoute)
                {
                    bestRoute = route;
                }
            }

            return bestRoute;
        }
    }
}
