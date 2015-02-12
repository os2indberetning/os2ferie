namespace Infrastructure.AddressServices.Classes
{
    public class Address
    {
        /// <summary>
        /// Street name.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Building identifier
        /// </summary>
        public string StreetNr { get; set; }
        /// <summary>
        /// Zip code
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Address type.
        /// </summary>
        public Coordinates.CoordinatesType Type { get; set; }
    }
}
