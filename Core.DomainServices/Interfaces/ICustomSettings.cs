using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainServices.Interfaces
{
    public interface ICustomSettings
    {
        string SMTPPassword { get; }
        string SMTPHost { get; }
        string SMTPUser { get; }
        string SMTPHostPort { get; }
        string MailFromAddress { get; }
        string MailSubject { get; }
        string MailBody { get; }

        string KMDFilePath { get; }
        string KMDBackupFilePath { get; }
        string KMDFileName { get; }
        string KMDHeader { get; }
        string KMDStaticNumber { get; }
        string KMDMunicipalityNumber { get; }
        string KMDReservedNumber { get; }
        string AdministrativeCostCenterPrefix { get; }
        string AdministrativeAccount { get; }

        string Municipality { get; }
        bool SdIsEnabled { get; }
        string SdUsername { get; }
        string SdPassword { get; }
        string SdInstitutionNumber { get; }

        string MapStartStreetName { get; }
        string MapStartStreetNumber { get; }
        string MapStartZipCode { get; }
        string MapStartTown { get; }
        string SeptimaApiKey { get; }
        string Version { get; }

        string AdDomain { get; }
        string DailyErrorLogMail { get; }
        string AlternativeCalculationMethod { get; }

        string DbViewMedarbejder { get; }
        string DbViewOrganisation { get; }
        string DbViewVacationBalance { get; }
        string DbIntegration { get; }

        IQueryable<string> GetUnProtected();
    }
}
