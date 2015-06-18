using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Core.DomainModel;
using Core.DomainServices.RoutingClasses;
using log4net;
using Newtonsoft.Json.Linq;

namespace Infrastructure.AddressServices.Routing
{
    public class SeptimaRouter
    {
        #region Public methods

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get routes and alternative routes for the given set of coordinates.
        /// </summary>
        /// <param name="transportType">Type of transport. Car or Bike.</param>
        /// <param name="routeCoordinates"></param>
        /// <exception cref="RouteInformationException">Thrown if no route was returned or no rute was found.</exception>
        /// <returns></returns>
        public IEnumerable<RouteInformation> GetRoute(DriveReportTransportType transportType, IEnumerable<Coordinates> routeCoordinates)
        {
            List<RouteInformation> routes = new List<RouteInformation>();
            HttpWebRequest request = CreateRequest(transportType, routeCoordinates);
            RootRouteObject result = ExecuteAndRead(transportType, request);

            if (result == null)
            {
                var e = new RouteInformationException("No route returned.");
                Logger.Error("Exception when getting route information", e);
                throw e;
            }

            if (result.status != 0)
            {
                var e = new RouteInformationException("No route found."); ;
                Logger.Error("Exception when getting route information", e);
                throw e;
            }

            var route = new RouteInformation();

            route.Length = result.route_summary.distance_not_including_ferry;
            route.Duration = result.route_summary.total_time;
            route.EndStreet = result.route_summary.end_point;
            route.StartStreet = result.route_summary.start_point;
            route.GeoPoints = result.route_geometry;

            routes.Add(route);
            for (int i = 0; i < result.alternative_summaries.Count; i++)
            {
                route = new RouteInformation
                {
                    Length = result.alternative_summaries[i].total_distance,
                    Duration = result.alternative_summaries[i].total_time,
                    EndStreet = result.alternative_summaries[i].end_point,
                    StartStreet = result.alternative_summaries[i].start_point,
                    GeoPoints = result.alternative_geometries[i]
                };

                routes.Add(route);
            }
            return routes;
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Build query URL and create HTTP request following the service API url specifications.
        /// </summary>
        /// <param name="transportType">Type of transport. Car or Bike</param>
        /// <param name="routeCoordinates"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(DriveReportTransportType transportType, IEnumerable<Coordinates> routeCoordinates)
        {
            string query = "";
            var coordinateses = routeCoordinates as IList<Coordinates> ?? routeCoordinates.ToList();
            Coordinates origin, destination;
            try
            {
                origin = coordinateses.Single(p => p.Type == Coordinates.CoordinatesType.Origin);
                destination = coordinateses.Single(p => p.Type == Coordinates.CoordinatesType.Destination);
            }
            catch (InvalidOperationException e)
            {
                var up = new RouteInformationException("Mutltiple coordinates with type Origin and/or Destination.", e);
                Logger.Error("Multiple coordinates with type origin and/or destination", up);
                throw up;
            }

            if (origin == null || destination == null)
            {
                var up = new RouteInformationException("Coordinate of type Origin and/or Destination missing.");
                Logger.Error("Coordinate of type Origin and/or destination missing.", up);
                throw up;
            }

            if (!((origin.Latitude.Length - origin.Latitude.IndexOf(".") + 1) <= 5))
            {
                origin.Latitude = origin.Latitude.Substring(0, origin.Latitude.IndexOf('.') + 1 + AddressCoordinates.CoordDecimals);
                origin.Longitude = origin.Longitude.Substring(0, origin.Longitude.IndexOf('.') + 1 + AddressCoordinates.CoordDecimals);
            }

            query = "loc=" + origin.Latitude + "," + origin.Longitude;

            query = coordinateses.Where(p => p.Type == Coordinates.CoordinatesType.Via).Aggregate(query, (current, point) => current + ("&loc=" + point.Latitude + "," + point.Longitude));

            query += "loc=" + destination.Latitude + "," + destination.Longitude + "&instructions=true";

            switch (transportType)
            {
                case DriveReportTransportType.Car:
                    return (HttpWebRequest)WebRequest.Create(UrlDefinitions.RoutingUrl + query);
                case DriveReportTransportType.Bike:
                    return (HttpWebRequest)WebRequest.Create(UrlDefinitions.BikeRoutingUrl + query);
            }

            // Should never be reached
            return null;


        }

        /// <summary>
        /// Execute HTTP request and read the response.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Formatted response from service.</returns>
        private RootRouteObject ExecuteAndRead(DriveReportTransportType transportType, HttpWebRequest request)
        {
            var responseString = "";

            Stream responseStream;
            try
            {
                var distanceResponse = request.GetResponse();
                responseStream = distanceResponse.GetResponseStream();
            }
            catch (WebException e)
            {
                var up = new RouteInformationException("Server error", e);
                Logger.Error("Server error", up);
                throw up;
            }

            if (responseStream == null) return null;

            var streamReader = new StreamReader(responseStream);
            responseString = streamReader.ReadToEnd();
            streamReader.Close();
            return ParseJson(transportType, responseString);
        }

        /// <summary>
        /// Formats and structures the response. 
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Response reprensented in custom class.</returns>
        private RootRouteObject ParseJson(DriveReportTransportType transportType, string response)
		{
			JToken jRouteObject = JToken.Parse(response);

			if (jRouteObject == null)
			{
				return null;
			}

			RootRouteObject route = new RootRouteObject(jRouteObject)
			{
				route_summary = new RouteSummary(jRouteObject["route_summary"])
			};

		    if (jRouteObject["alternative_geometries"] != null)
		    {
                if (jRouteObject["alternative_summaries"] != null)
		        {
                    var altSums = jRouteObject["alternative_summaries"].ToList();

                    var altGeos = jRouteObject["alternative_geometries"].ToList();
                    
                    for (int i = 0; i < altGeos.Count; i++)
                    {
                        route.alternative_geometries.Add((string)altGeos[i]);
                        route.alternative_summaries.Add(new AlternativeSummary(altSums[i]));
                    }
		        }                
		    }

		    if (jRouteObject["route_instructions"] != null)
		    {
                var distanceWithoutFerry = 0;
		        var list = jRouteObject["route_instructions"].ToList();

                // Iterate all route instructions except for the last one.
                // The last one is different from the others somehow.
                // It does not have a mode. It only has 7 properties where the others have 8.
                // It never seems to have any distance though, so it shouldnt matter.
                for (var i = 0; i < list.Count - 1; i++)
                {
                    var currentInstruction = list.ElementAt(i).ToList();
                    var mode = currentInstruction.ElementAt(8).ToString();
                                     
                    // If the transportType is car mode 2 means travelling on a ferry.
                    if (transportType.Equals(DriveReportTransportType.Car))
                    {
                        if (mode != "2")
                        {
                            distanceWithoutFerry += int.Parse(currentInstruction.ElementAt(5).ToString().Replace("m", ""));
                        }
                    }
                    // If the transportType is bicycle mode 3 means travelling on a ferry. Annoying that it's not mode 2 for both of them.
                    else if (transportType.Equals(DriveReportTransportType.Bike))
                    {
                        if (mode != "3")
                        {
                            distanceWithoutFerry += int.Parse(currentInstruction.ElementAt(5).ToString().Replace("m", ""));
                        }
                    }
                   
                }
                route.route_summary.distance_not_including_ferry = distanceWithoutFerry;
		    }
						       
			return route;
		}

        #endregion
    }
}
