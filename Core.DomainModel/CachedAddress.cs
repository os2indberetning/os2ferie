
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
        }

        public CachedAddress()
        {
            this.IsDirty = true;
        }

        public bool IsDirty { get; set; }
    }
}
