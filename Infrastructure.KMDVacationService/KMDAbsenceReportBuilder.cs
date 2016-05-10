using System.Collections.Generic;
using Core.DomainModel;
using Infrastructure.KMDVacationService.Models;
using Core.DomainServices;
using Infrastructure.KMDVacationService.Interfaces;

namespace Infrastructure.KMDVacationService
{
    public class KMDAbsenceReportBuilder : IKMDAbsenceReportBuilder
    {
        public List<KMDAbsenceReport> Create(VacationReport report)
        {
            return Build(report, Operation.Create);
        }

        public List<KMDAbsenceReport> Delete(VacationReport report)
        {
            return Build(report, Operation.Delete);
        }
        public List<KMDAbsenceReport> Edit(VacationReport oldReport, VacationReport newReport)
        {
            var list = new List<KMDAbsenceReport>();

            var oldStartsWithHalfDay = oldReport.StartTime != null;
            var newStartsWithHalfDay = newReport.StartTime != null;

            if (oldStartsWithHalfDay && !newStartsWithHalfDay)
            {
                // We have to delete the old absence
                var absence = BuildStart(oldReport, Operation.Delete);
                list.Add(absence);
            }
            else if (!oldStartsWithHalfDay && newStartsWithHalfDay)
            {
                var absence = BuildStart(newReport, Operation.Create);
                list.Add(absence);
            }
            else if(oldStartsWithHalfDay)
            {
                var oldStartAbsence = BuildStart(oldReport, Operation.Create);
                var newStartAbsence = BuildStart(newReport, Operation.Edit);
                UpdateAbsence(oldStartAbsence, newStartAbsence);
                list.Add(newStartAbsence);
            }

            var oldCenterAbsence = BuildCenter(oldReport, Operation.Create);
            var newCenterAbsence = BuildCenter(newReport, Operation.Edit);
            UpdateAbsence(oldCenterAbsence, newCenterAbsence);
            list.Add(newCenterAbsence);


            var oldEndsWithHalfDay = oldReport.EndTime != null;
            var newEndsWithHalfDay = newReport.EndTime != null;

            if (oldEndsWithHalfDay && !newEndsWithHalfDay)
            {
                // We have to delete the old absence
                var absence = BuildEnd(oldReport, Operation.Delete);
                list.Add(absence);
            }
            else if (!oldEndsWithHalfDay && newEndsWithHalfDay)
            {
                var absence = BuildEnd(newReport, Operation.Create);
                list.Add(absence);
            }
            else if (oldEndsWithHalfDay)
            {
                var oldEndAbsence = BuildEnd(oldReport, Operation.Create);
                var newEndAbsence = BuildEnd(newReport, Operation.Edit);
                UpdateAbsence(oldEndAbsence, newEndAbsence);
                list.Add(newEndAbsence);
            }

            return list;
        }

        private void UpdateAbsence(KMDAbsenceReport oldReport, KMDAbsenceReport newReport)
        {
            newReport.OldEndDate = oldReport.EndDate;
            newReport.OldEndTime = oldReport.EndTime;
            newReport.OldStartDate = oldReport.StartDate;
            newReport.OldStartTime = oldReport.StartTime;
            newReport.OldType = oldReport.Type;
        }

        private KMDAbsenceReport BuildBase(VacationReport report, Operation operation)
        {
            var startDate = report.StartTimestamp.ToDateTime();
            var endDate = report.EndTimestamp.ToDateTime();

            return new KMDAbsenceReport
            {
                EmploymentId = report.Employment.EmploymentId,
                Operation = operation,
                Type = report.VacationType,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        private KMDAbsenceReport BuildCenter(VacationReport report, Operation operation)
        {
            var absence = BuildBase(report, operation);

            if(report.StartTime != null)
                absence.StartDate = absence.StartDate.AddDays(1);

            if (report.EndTime != null)
                absence.EndDate = absence.EndDate.AddDays(-1);

            return absence;
        }

        private KMDAbsenceReport BuildStart(VacationReport report, Operation operation)
        {
            var absence = BuildBase(report, operation);

            absence.StartDate = absence.StartDate;
            absence.EndDate = absence.StartDate;
            absence.StartTime = report.StartTime;
            absence.EndTime = null;

            return absence;
        }

        private KMDAbsenceReport BuildEnd(VacationReport report, Operation operation)
        {
            var absence = BuildBase(report, operation);

            absence.StartDate = absence.EndDate;
            absence.EndDate = absence.EndDate;
            absence.StartTime = null;
            absence.EndTime = report.EndTime;

            return absence;
        }

        private List<KMDAbsenceReport> Build(VacationReport report, Operation operation)
        {
            var list = new List<KMDAbsenceReport>();

            var baseAbsence = BuildBase(report, operation);

            var sameDay = baseAbsence.StartDate.Date == baseAbsence.EndDate.Date;

            if (sameDay || (report.StartTime == null && report.EndTime == null))
            {
                // Single report;
                baseAbsence.StartTime = report.StartTime;
                baseAbsence.EndTime = report.EndTime;
                list.Add(baseAbsence);
                return list;
            }

            if (report.StartTime != null)
            {
                list.Add(BuildStart(report, operation));
            }

            list.Add(BuildCenter(report, operation));

            if (report.EndTime != null)
            {
                list.Add(BuildEnd(report, operation));
            }

            return list;
        }

    }
}
