namespace Core.DmzModel
{
    public partial class GPSCoordinate
    {
        public int Id { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int RouteId { get; set; }

        public virtual Route Route { get; set; }
    }
}
