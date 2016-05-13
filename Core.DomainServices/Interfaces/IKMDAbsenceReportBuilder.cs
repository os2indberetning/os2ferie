using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices.KMDAbsenceModels;

namespace Core.DomainServices.Interfaces
{
    public interface IKMDAbsenceReportBuilder
    {
        IList<KMDAbsenceReport> Create(VacationReport report);
        IList<KMDAbsenceReport> Delete(VacationReport report);
        IList<KMDAbsenceReport> Edit(VacationReport oldReport, VacationReport newReport);
    }
}
