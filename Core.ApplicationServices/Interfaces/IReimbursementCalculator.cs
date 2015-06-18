using Core.DomainModel;
using Core.DomainServices.RoutingClasses;

namespace Core.ApplicationServices.Interfaces
{
    public interface IReimbursementCalculator
    {
        DriveReport Calculate(RouteInformation drivenRoute, DriveReport report);
    }
}