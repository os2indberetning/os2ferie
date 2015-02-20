using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IReimbursementCalculator
    {
        DriveReport Calculate(DriveReport report, string reportMethod);
    }
}