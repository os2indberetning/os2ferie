
namespace Core.DomainServices.RoutingClasses
{
    public class RouteInformation
    {
        /// <summary>
        /// String containing route geo points.
        /// </summary>
        public string GeoPoints { get; set; }
        /// <summary>
        /// Route length.
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Route duration.
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Start street.
        /// </summary>
        public string StartStreet { get; set; }
        /// <summary>
        /// End street.
        /// </summary>
        public string EndStreet { get; set; }
    }
}
