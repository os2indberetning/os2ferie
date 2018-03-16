using Core.DomainServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public class CustomSettings : ICustomSettings
    {
        private readonly string _protected = "PROTECTED_";

        public string SMTPPassword
        {
            get
            {
                return GetProtectedValue("SMTP_PASSWORD");
            }
        }
        public string SMTPHost
        {
            get
            {
                return GetProtectedValue("SMTP_HOST");
            }
        }
        public string SMTPUser
        {
            get
            {
                return GetProtectedValue("SMTP_USER");
            }
        }
        public string SMTPHostPort
        {
            get
            {
                return GetProtectedValue("SMTP_HOST_PORT");
            }
        }
        public string MailBody
        {
            get
            {
                return GetProtectedValue("MAIL_BODY");
            }
        }

        public string MailFromAddress
        {
            get
            {
                return GetProtectedValue("MAIL_FROM_ADDRESS");
            }
        }
        public string MailSubject
        {
            get
            {
                return GetProtectedValue("MAIL_SUBJECT");
            }
        }
        public string KMDFilePath
        {
            get
            {
                return GetProtectedValue("KMDFilePath");
            }
        }
        public string KMDBackupFilePath
        {
            get
            {
                return GetProtectedValue("KMDBackupFilePath");
            }
        }
        public string KMDFileName
        {
            get
            {
                return GetProtectedValue("KMDFileName");
            }
        }
        public string KMDHeader
        {
            get
            {
                return GetProtectedValue("KMDHeader");
            }
        }
        public string KMDStaticNumber
        {
            get
            {
                return GetProtectedValue("KMDStaticNr");
            }
        }
        public string KMDMunicipalityNumber
        {
            get
            {
                return GetProtectedValue("CommuneNr");
            }
        }
        public string KMDReservedNumber
        {
            get
            {
                return GetProtectedValue("KMDReservedNr");
            }
        }
        public string AdministrativeCostCenterPrefix
        {
            get
            {
                return GetProtectedValue("AdministrativeCostCenterPrefix");
            }
        }
        public string AdministrativeAccount
        {
            get
            {
                return GetProtectedValue("AdminstrativeAccount");
            }
        }

        public string Municipality
        {
            get
            {
                return GetProtectedValue("muniplicity");
            }
        }
        public string SdInstitutionNumber
        {
            get
            {
                return GetProtectedValue("institutionNumber");
            }
        }
        public string SdPassword
        {
            get
            {
                return GetProtectedValue("SDUserPassword");
            }
        }
        public string SdUsername
        {
            get
            {
                return GetProtectedValue("SDUserName");
            }
        }
        public bool SdIsEnabled
        {
            get
            {
                bool result;
                bool parseSucces = bool.TryParse(GetValue("UseSd"), out result);
                if (!parseSucces)
                {
                    return false;
                }
                else
                {
                    return result;
                }
            }
        }

        public string MapStartStreetName
        {
            get
            {
                return GetValue("MapStartStreetName");
            }
        }
        public string MapStartStreetNumber
        {
            get
            {
                return GetValue("MapStartStreetNumber");
            }
        }
        public string MapStartZipCode
        {
            get
            {
                return GetValue("MapStartZipCode");
            }
        }
        public string MapStartTown
        {
            get
            {
                return GetValue("MapStartTown");
            }
        }

        public string AdDomain
        {
            get
            {
                return GetProtectedValue("AD_DOMAIN");
            }
        }
        public string DailyErrorLogMail
        {
            get
            {
                return GetProtectedValue("DailyErrorLogMail");
            }
        }
        public string AlternativeCalculationMethod
        {
            get
            {
                return GetValue("AlternativeCalculationMethod");
            }
        }

        public string DbViewMedarbejder
        {
            get
            {
                return GetProtectedValue("DATABASE_VIEW_MEDARBEJDER");
            }
        }
        public string DbViewOrganisation
        {
            get
            {
                return GetProtectedValue("DATABASE_VIEW_ORGANISATION");
            }
        }

        public string DbViewVacationBalance
        {
            get
            {
                return GetProtectedValue("DATABASE_VIEW_VACATION_BALANCE");
            }
        }

        public string DbIntegration
        {
            get
            {
                return GetProtectedValue("DATABASE_INTEGRATION");
            }
        }

        public string SeptimaApiKey
        {
            get
            {
                return GetValue("SEPTIMA_API_KEY");
            }
        }
        public string Version
        {
            get
            {
                return GetValue("Version");
            }
        }

        private string GetProtectedValue(string key)
        {
            return GetValue(_protected + key);
        }

        private string GetValue(string key)
        {
            string result;
            try
            {
                result = ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                result = "";
            }
            return result ?? "";
        }
        public IQueryable<string> GetUnProtected()
        {
            throw new NotImplementedException();
        }
    }
}
