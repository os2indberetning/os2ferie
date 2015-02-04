
namespace Core.DomainModel
{
    public class Point : Address
    {
        public virtual Point NextPoint { get; set; }
        public virtual Point PreviousPoint { get; set; }
        public virtual PersonalRoute PersonalRoute { get; set; }
    }
}
