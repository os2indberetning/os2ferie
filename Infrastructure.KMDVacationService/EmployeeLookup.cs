using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.KMDVacationService
{
    public class EmployeeLookup
    {
        public List<KMD_FerieService.ZLPE_EMPLOYEE_INFO> EmployeeInfo(DateTime date, string cpr)
        {
            using (var webService = new KMD_FerieService.LPT_VACAB_Service_OutClient("HTTPS_Port"))
            {
                webService.ClientCredentials?.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerName, "localhost");
                return webService.GET_EMPLOYEE_INFO(date.ToString("yyyy-MM-dd"), cpr).ToList();
            }
        }
    }
}
