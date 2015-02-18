
namespace Core.DomainModel
{
    public class Point : Address
    {
        public int? NextPointId { get; set; }
        public virtual Point NextPoint { get; set; }
        public int? PreviousPointId { get; set; }
        public virtual Point PreviousPoint { get; set; }
        public int PersonalRouteId { get; set; }
        public virtual PersonalRoute PersonalRoute { get; set; }
    }
}
