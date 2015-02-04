
namespace Core.DomainModel
{
    public class DriveReportPoint : Address
    {
        public virtual DriveReportPoint NextPoint { get; set; }
        public virtual DriveReportPoint PreviousPoint { get; set; }
        public virtual DriveReport DriveReport { get; set; }
    }
}
