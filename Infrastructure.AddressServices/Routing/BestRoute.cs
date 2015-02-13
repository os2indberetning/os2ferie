using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.AddressServices.Classes;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.AddressServices.Routing
{
    public class BestRoute : IRoute<RouteInformation>
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
            var addressesList = addresses as IList<Address>;

            List<Coordinates> routeCoordinates = new List<Coordinates>();
            AddressCoordinates coordService = new AddressCoordinates();
            SeptimaRouter septimaService = new SeptimaRouter();
            
            var origin = addressesList[0];
            var destination = addressesList[addressesList.Count - 1];

            addressesList.Remove(origin);
            addressesList.Remove((destination));

            if (String.IsNullOrEmpty(origin.Longitude))
            {
                routeCoordinates.Add(coordService.GetCoordinates(origin, Coordinates.CoordinatesType.Origin));
            }
            else
            {
                routeCoordinates.Add(new Coordinates()
                {
                    Longitude = origin.Longitude,
                    Latitude = origin.Latitude,
                    Type = Coordinates.CoordinatesType.Origin
                });
            }

            foreach (var address in addressesList)
            {
                if (String.IsNullOrEmpty(address.Longitude))
                {
                    routeCoordinates.Add(coordService.GetCoordinates(address,
                        Coordinates.CoordinatesType.Via));
                }
                else
                {
                    routeCoordinates.Add(new Coordinates()
                    {
                        Longitude = address.Longitude,
                        Latitude = address.Latitude,
                        Type = Coordinates.CoordinatesType.Via
                    });
                }
            }

            if (String.IsNullOrEmpty(destination.Longitude))
            {
                routeCoordinates.Add(coordService.GetCoordinates(destination, Coordinates.CoordinatesType.Destination));
            }
            else
            {
                routeCoordinates.Add(new Coordinates()
                {
                    Longitude = destination.Longitude,
                    Latitude = destination.Latitude,
                    Type = Coordinates.CoordinatesType.Destination
                });
            }

            List<RouteInformation> routes = septimaService.GetRoute(routeCoordinates).OrderBy(x => x.Duration).ToList();

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
