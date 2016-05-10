using System.Collections.Generic;
using Core.DomainModel;
using Infrastructure.KMDVacationService.Interfaces;
using Infrastructure.KMDVacationService.Models;

namespace Infrastructure.KMDVacationService
{
    public class KMDAbsenceService : IKMDAbsenceService
    {

        public void ReportAbsence(List<KMDAbsenceReport> absenceReports)
        {

            using (var webService = new KMD_FerieService.LPT_VACAB_Service_OutClient("HTTPS_Port"))
            {
                webService.ClientCredentials?.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "Skanderborg kommune - Sysint");

                foreach (var report in absenceReports)
                {
                    var response = webService.SET_ABSENCE_PULS(
                        report.StartDate.ToString("yyyy-MM-dd"),
                        report.OldStartDate?.ToString("yyyy-MM-dd"), report.StartTime?.ToString("hhmm"),
                        report.OldStartTime?.ToString("hhmm"), report.EndDate.ToString("yyyy-MM-dd"),
                        report.OldEndDate?.ToString("yyyy-MM-dd"), report.EndTime?.ToString("hhmm"),
                        report.OldEndTime?.ToString("hhmm"), report.ExtraData, report.Operation.AsString(),
                        report.EmploymentId.ToString(), report.Type == VacationType.Regular ? "FE" : "6F",
                        report.Type == VacationType.Regular ? "FE" : "6F"
                        );

                    if(response.TYPE == "") continue;

                    throw new KMDSetAbsenceFailedException(response.MESSAGE);
                }

            }
        }
    }
}
