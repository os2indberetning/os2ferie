namespace Core.DomainServices.RoutingClasses
{
    public class Coordinates
    {
        public enum CoordinatesType
        {
            Origin,
            Destination,
            Via,
            Unkown

        }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public CoordinatesType Type { get; set; }

        public override bool Equals(object obj)
        {
            var address = obj as Coordinates; 
            if (address == null)
            {
                return false;
            }
            if (Latitude != address.Latitude || Longitude != address.Longitude || Type != address.Type)
            {
                return false;
            }

            return true;
        }
    }
}
