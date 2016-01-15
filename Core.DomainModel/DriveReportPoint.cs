
namespace Core.DomainModel
{
    public class DriveReportPoint : Address
    {
        public int? NextPointId { get; set; }
        public int? PreviousPointId { get; set; }
        public int DriveReportId { get; set; }
        public virtual DriveReport DriveReport { get; set; }

        public override string ToString()
        {
            return StreetName + " " + StreetNumber + ", " + ZipCode + " " + Town;
        }
    }
}
