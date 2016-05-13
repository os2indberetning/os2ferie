using System.Collections.Generic;
using Core.DomainServices.KMDAbsenceModels;

namespace Core.DomainServices.Interfaces
{
    public interface IKMDAbsenceService
    {
        void ReportAbsence(IList<KMDAbsenceReport> absenceReports);
    }
}
