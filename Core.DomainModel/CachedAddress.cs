
using System;

namespace Core.DomainModel
{
    public class CachedAddress : Address
    {
        public CachedAddress(Address addr)
        {
            this.StreetName = addr.StreetName;
            this.StreetNumber = addr.StreetNumber;
            this.Town = addr.Town;
            this.ZipCode = addr.ZipCode;
            this.Latitude = addr.Latitude;
            this.Longitude = addr.Longitude;
            this.Description = addr.Description;
            this.IsDirty = true;
            this.DirtyString = addr.StreetName + " " + addr.StreetNumber + ", " + addr.ZipCode + " " + addr.Town;
        }

        public CachedAddress()
        {
            this.IsDirty = true;
        }

        public bool IsDirty { get; set; }
        //This is what is used to map a dirty adress to a laundered adress.
        //The format is StreetNameStreetNumberZipCodeTown (no spaces between the variables)
        public string DirtyString { get; set; }
    }
}
