using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices.Interfaces
{
    public interface IKMDAbsenceService
    {
        void ReportAbsence(IList<KMDAbsenceReport> absenceReports);
    }
}
