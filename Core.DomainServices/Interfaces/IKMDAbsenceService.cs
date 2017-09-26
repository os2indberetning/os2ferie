using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices.Interfaces
{
    public interface IKMDAbsenceService
    {
        void SetAbsence(IList<KMDAbsenceReport> absenceReports);
        List<Child> GetChildren(Employment employment);
    }
}
