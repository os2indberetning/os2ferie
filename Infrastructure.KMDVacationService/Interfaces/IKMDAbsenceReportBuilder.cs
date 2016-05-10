using System.Collections.Generic;
using Core.DomainModel;
using Infrastructure.KMDVacationService.Models;

namespace Infrastructure.KMDVacationService.Interfaces
{
    public interface IKMDAbsenceReportBuilder
    {
        List<KMDAbsenceReport> Create(VacationReport report);
        List<KMDAbsenceReport> Delete(VacationReport report);
        List<KMDAbsenceReport> Edit(VacationReport oldReport, VacationReport newReport);
    }
}