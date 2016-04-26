using Infrastructure.KMDVacationService.Interfaces;

namespace Infrastructure.KMDVacationService
{
    public class ReportAbsence : IReportAbsence
    {
        public KMD_FerieService.BAPIRET2 Set_Absence_Plus(string iv_begda, string iv_begda_old, string iv_begti, string iv_begti_old, string iv_endda, string iv_endda_old, string iv_endti, string iv_endti_old,
            string iv_extra_data, string iv_opera, string iv_pernr, string iv_subty, string iv_subty_old)
        {
            using (var webService = new KMD_FerieService.LPT_VACAB_Service_OutClient("HTTPS_Port"))
            {
                webService.ClientCredentials?.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "Skanderborg kommune - Sysint");
                return webService.SET_ABSENCE_PULS(iv_begda, iv_begda_old, iv_begti, iv_begti_old, iv_endda, iv_endda_old, iv_endti, iv_endti_old, iv_extra_data, iv_opera, iv_pernr, iv_subty, iv_subty_old);

            }
        }
    }
}
