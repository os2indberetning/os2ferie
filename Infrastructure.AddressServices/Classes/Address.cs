namespace Infrastructure.AddressServices.Classes
{
    public class Address
    {
        public int Id { get; set; }
        /// <summary>
        /// StreetName name.
        /// </summary>
        public string StreetName { get; set; }
        /// <summary>
        /// Building identifier
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Zip code
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Address type.
        /// </summary>

        public string Town { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Description { get; set; }
    }
}

        