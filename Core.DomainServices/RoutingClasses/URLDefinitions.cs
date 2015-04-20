
using System.Configuration;

namespace Core.DomainServices.RoutingClasses
{
    public class UrlDefinitions
    {
        private const string _coordinateToAddressUrl = @"http://dawa.aws.dk/adgangsadresser/reverse?";
        private const string _coordinatesURL = @"http://dawa.aws.dk/adresser?";



        /// <summary>
        /// URL for address laundering.
        /// </summary>
        public static string LaunderingUrl
        {
            get
            {
                var apiKey = ConfigurationManager.AppSettings["SEPTIMA_API_KEY"];
                return "http://service.adressevask.dk/" + apiKey + "/json?json=";
            }
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
        /// URL for route construction.
        /// </summary>
        public static string RoutingUrl
        {
            get
            {
                var apiKey = ConfigurationManager.AppSettings["SEPTIMA_API_KEY"];
                return "http://routing.septima.dk/" + apiKey + "/car/viaroute?";
            }
        }
    }
}

//favrskov-HIEEPKWJED
