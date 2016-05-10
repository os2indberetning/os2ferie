using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.KMDVacationService.Models;

namespace Infrastructure.KMDVacationService.Interfaces
{
    public interface IKMDAbsenceService
    {
        void ReportAbsence(List<KMDAbsenceReport> absenceReports);
    }
}
