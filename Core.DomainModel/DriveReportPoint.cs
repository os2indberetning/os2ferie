
namespace Core.DomainModel
{
    public class DriveReportPoint : Address
    {
        public int? NextPointId { get; set; }
        public virtual DriveReportPoint NextPoint { get; set; }
        public int? PreviousPointId { get; set; }
        public virtual DriveReportPoint PreviousPoint { get; set; }
        public int DriveReportId { get; set; }
        public virtual DriveReport DriveReport { get; set; }
    }
}
