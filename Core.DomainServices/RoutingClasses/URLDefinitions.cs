
using System.Configuration;

namespace Core.DomainServices.RoutingClasses
{
    public class UrlDefinitions
    {
        private const string _coordinateToAddressUrl = @"http://dawa.aws.dk/adgangsadresser/reverse?";
        private const string _coordinatesURL = @"http://dawa.aws.dk/adresser?";
        private const string _launderingUrl = @"http://dawa.aws.dk/adgangsadresser?";



        /// <summary>
        /// URL for address laundering.
        /// </summary>
        public static string LaunderingUrl
        {
            get { return _launderingUrl; }

        }

        /// <summary>
        /// URL for address coordinates.
        /// </summary>
        public static string CoordinatesUrl
        {
            get { return _coordinatesURL; }
        }

        public static string CoordinateToAddressUrl
        {
            get { return _coordinateToAddressUrl; }
        }

        /// <summary>
        /// URL for car route construction.
        /// </summary>
        public static string RoutingUrl
        {
            get
            {
                var apiKey = ConfigurationManager.AppSettings["PROTECTED_SEPTIMA_API_KEY"];
                return "http://new-routing.septima.dk/" + apiKey + "/car/viaroute?";
            }
        }

        /// <summary>
        /// URL for bike route construction.
        /// </summary>
        public static string BikeRoutingUrl
        {
            get
            {
                var apiKey = ConfigurationManager.AppSettings["PROTECTED_SEPTIMA_API_KEY"];
                return "http://new-routing.septima.dk/" + apiKey + "/bicycle/viaroute?";
            }
        }
    }
}
