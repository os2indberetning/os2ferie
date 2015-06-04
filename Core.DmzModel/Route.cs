namespace Core.DmzModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Route
    {
        public Route()
        {
            GPSCoordinates = new HashSet<GPSCoordinate>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public double TotalDistance { get; set; }

        public virtual DriveReport DriveReport { get; set; }

        public virtual ICollection<GPSCoordinate> GPSCoordinates { get; set; }
    }
}
