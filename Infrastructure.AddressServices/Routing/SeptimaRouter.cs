using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Core.DomainServices.RoutingClasses;
using Newtonsoft.Json.Linq;

namespace Infrastructure.AddressServices.Routing
{
	public class SeptimaRouter
	{
		#region Public methods

		/// <summary>
		/// Get routes and alternative routes for the given set of coordinates.
		/// </summary>
		/// <param name="routeCoordinates"></param>
		/// <exception cref="RouteInformationException">Thrown if no route was returned or no rute was found.</exception>
		/// <returns></returns>
		public IEnumerable<RouteInformation> GetRoute(IEnumerable<Coordinates> routeCoordinates)
		{
			List<RouteInformation> routes = new List<RouteInformation>();
			HttpWebRequest request = CreateRequest(routeCoordinates);
			RootRouteObject result = ExecuteAndRead(request);

			if (result == null)
			{
				throw new RouteInformationException("No route returned.");
			}

			if (result.status != 0)
			{
				throw new RouteInformationException("No route found.");
			}
			var route = new RouteInformation();

			route.Length = result.route_summary.total_distance;
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
		/// <param name="routeCoordinates"></param>
		/// <returns></returns>
		private HttpWebRequest CreateRequest(IEnumerable<Coordinates> routeCoordinates)
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
				throw new RouteInformationException("Mutltiple coordinates with type Origin and/or Destination.", e);
			}

			if (origin == null || destination == null)
			{
				throw new RouteInformationException("Coordinate of type Origin and/or Destination missing.");
			}

			if( !((origin.Latitude.Length - origin.Latitude.IndexOf(".") + 1) <= 5) )
			{
				origin.Latitude = origin.Latitude.Substring(0, origin.Latitude.IndexOf('.') + 1 + AddressCoordinates.CoordDecimals);
				origin.Longitude = origin.Longitude.Substring(0, origin.Longitude.IndexOf('.') + 1 + AddressCoordinates.CoordDecimals);
			}

			query = "loc=" + origin.Latitude + "," + origin.Longitude;

			query = coordinateses.Where(p => p.Type == Coordinates.CoordinatesType.Via).Aggregate(query, (current, point) => current + ("&loc=" + point.Latitude + "," + point.Longitude));

			query += "loc=" + destination.Latitude + "," + destination.Longitude;

			return (HttpWebRequest)WebRequest.Create(UrlDefinitions.RoutingUrl + query);
		}

		/// <summary>
		/// Execute HTTP request and read the response.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Formatted response from service.</returns>
		private RootRouteObject ExecuteAndRead(HttpWebRequest request)
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
				throw new RouteInformationException("Server error, coordinates invalid.", e);
			}

			if (responseStream == null) return null;

			var streamReader = new StreamReader(responseStream);
			responseString = streamReader.ReadToEnd();
			streamReader.Close();
			return ParseJson(responseString);
		}

		/// <summary>
		/// Formats and structures the response. 
		/// </summary>
		/// <param name="response"></param>
		/// <returns>Response reprensented in custom class.</returns>
		private RootRouteObject ParseJson(string response)
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
						       
			return route;
		}

		#endregion
	}
}
