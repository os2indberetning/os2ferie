using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices.Interfaces;
using System;

namespace Infrastructure.KMDVacationService
{
    public class KMDAbsenceService : IKMDAbsenceService
    {
        public void SetAbsence(IList<KMDAbsenceReport> absenceReports)
        {
            using (var webService = new SetAbsenceAttendance.SetAbsenceAttendance_OS_SIClient("HTTPS_Port"))
            {
                webService.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "OPUS-indberetning (funktionscertifikat)");

                foreach (var report in absenceReports)
                {
                    var request = new SetAbsenceAttendance.SetAbsenceAttendanceRequest();
                    request.StartDate = report.StartDate.ToString("yyyy-MM-dd");
                    request.StartTime = report.StartTime?.ToString("hhmm");
                    request.EndDate = report.EndDate.ToString("yyyy-MM-dd");
                    request.EndTime = report.EndTime?.ToString("hhmm");
                    request.OriginalStartDate = report.OldStartDate?.ToString("yyyy-MM-dd");
                    request.OriginalStartTime = report.OldEndTime?.ToString("hhmm");
                    request.OriginalEndDate = report.OldEndDate?.ToString("yyyy-MM-dd");
                    request.OriginalEndTime = report.OldEndTime?.ToString("hhmm");
                    request.PersonnelNumber = report.EmploymentId.ToString();
                    request.Operation = report.KmdAbsenceOperation.AsString();
                    request.AdditionalData = report.ExtraData;

                    switch (report.Type)
                    {
                        case VacationType.Regular:
                            request.AbsenceAttendanceType = "FE";
                            request.OriginalAbsenceAttendanceType = "FE";
                            break;
                        case VacationType.SixthVacationWeek:
                            request.AbsenceAttendanceType = "6F";
                            request.OriginalAbsenceAttendanceType = "6F";
                            break;
                        case VacationType.Senior:
                            request.AbsenceAttendanceType = "SO";
                            request.OriginalAbsenceAttendanceType = "SO";
                            break;
                        case VacationType.Care:
                            request.AbsenceAttendanceType = "OS";
                            request.OriginalAbsenceAttendanceType = "OS";
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    var response = webService.SetAbsenceAttendance_OS_SI(request);

                    // If TYPE is empty, it succeeded
                    if (response.ReturnStatus.StatusType == "" || response.ReturnStatus.StatusType == "S") continue;

                    // Error occurred, cast exception containing error message.
                    throw new KMDSetAbsenceFailedException(response.ReturnStatus.MessageText);
                }

            }
        }

        public List<Child> GetChildren(Employment employment)
        {
            var children = new List<Child>();

#if !DEBUG

            using (var webService = new GetChildren.GetChildren_OS_SIClient("HTTPS_Port2"))
            {
                webService.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, "OPUS-indberetning (funktionscertifikat)");

                var request = new GetChildren.GetChildrenRequest();
                request.PersonnelNumber = employment.EmploymentId.ToString();
                

                var response = webService.GetChildren_OS_SI(request);
                
                foreach(var child in response.Child)
                {
                    children.Add(new Child
                    {
                        Id = int.Parse(child.ChildNumber),
                        FirstName = child.FirstName,
                        LastName = child.LastName
                    });
                }

            }

#endif

            return children;
        }
    }
}
