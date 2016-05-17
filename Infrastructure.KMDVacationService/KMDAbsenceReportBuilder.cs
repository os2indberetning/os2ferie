using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.Interfaces;

namespace Infrastructure.KMDVacationService
{
    public class KMDAbsenceReportBuilder : IKMDAbsenceReportBuilder
    {
        public IList<KMDAbsenceReport> Create(VacationReport report)
        {
            return Build(report, KMDAbsenceOperation.Create);
        }

        public IList<KMDAbsenceReport> Delete(VacationReport report)
        {
            return Build(report, KMDAbsenceOperation.Delete);
        }

        public IList<KMDAbsenceReport> Edit(VacationReport oldReport, VacationReport newReport)
        {
            var list = new List<KMDAbsenceReport>();

            var oldStartIsWithHalfDay = oldReport.StartTime != null;
            var newStartIsWithHalfDay = newReport.StartTime != null;

            if (oldStartIsWithHalfDay && !newStartIsWithHalfDay)
            {
                // We have to delete the old absence
                var absence = BuildStart(oldReport, KMDAbsenceOperation.Delete);
                list.Add(absence);
            }
            else if (!oldStartIsWithHalfDay && newStartIsWithHalfDay)
            {
                var absence = BuildStart(newReport, KMDAbsenceOperation.Create);
                list.Add(absence);
            }
            else if(oldStartIsWithHalfDay)
            {
                var oldStartAbsence = BuildStart(oldReport, KMDAbsenceOperation.Create);
                var newStartAbsence = BuildStart(newReport, KMDAbsenceOperation.Edit);
                UpdateAbsence(oldStartAbsence, newStartAbsence);
                list.Add(newStartAbsence);
            }

            var oldCenterAbsence = BuildCenter(oldReport, KMDAbsenceOperation.Create);
            var newCenterAbsence = BuildCenter(newReport, KMDAbsenceOperation.Edit);
            UpdateAbsence(oldCenterAbsence, newCenterAbsence);
            list.Add(newCenterAbsence);


            var oldEndsWithHalfDay = oldReport.EndTime != null;
            var newEndsWithHalfDay = newReport.EndTime != null;

            if (oldEndsWithHalfDay && !newEndsWithHalfDay)
            {
                // We have to delete the old absence
                var absence = BuildEnd(oldReport, KMDAbsenceOperation.Delete);
                list.Add(absence);
            }
            else if (!oldEndsWithHalfDay && newEndsWithHalfDay)
            {
                var absence = BuildEnd(newReport, KMDAbsenceOperation.Create);
                list.Add(absence);
            }
            else if (oldEndsWithHalfDay)
            {
                var oldEndAbsence = BuildEnd(oldReport, KMDAbsenceOperation.Create);
                var newEndAbsence = BuildEnd(newReport, KMDAbsenceOperation.Edit);
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

        private KMDAbsenceReport BuildBase(VacationReport report, KMDAbsenceOperation kmdAbsenceOperation)
        {
            var startDate = report.StartTimestamp.ToDateTime();
            var endDate = report.EndTimestamp.ToDateTime();

            return new KMDAbsenceReport
            {
                EmploymentId = report.Employment.EmploymentId,
                KmdAbsenceOperation = kmdAbsenceOperation,
                Type = report.VacationType,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        private KMDAbsenceReport BuildCenter(VacationReport report, KMDAbsenceOperation kmdAbsenceOperation)
        {
            var absence = BuildBase(report, kmdAbsenceOperation);

            if (report.StartTime != null)
                absence.StartDate = absence.StartDate.AddDays(1);

            if (report.EndTime != null)
                absence.EndDate = absence.EndDate.AddDays(-1);

            return absence;
        }

        private KMDAbsenceReport BuildStart(VacationReport report, KMDAbsenceOperation kmdAbsenceOperation)
        {
            var absence = BuildBase(report, kmdAbsenceOperation);

            absence.StartDate = absence.StartDate;
            absence.EndDate = absence.StartDate;
            absence.StartTime = report.StartTime;
            absence.EndTime = null;

            return absence;
        }

        private KMDAbsenceReport BuildEnd(VacationReport report, KMDAbsenceOperation kmdAbsenceOperation)
        {
            var absence = BuildBase(report, kmdAbsenceOperation);

            absence.StartDate = absence.EndDate;
            absence.EndDate = absence.EndDate;
            absence.StartTime = null;
            absence.EndTime = report.EndTime;

            return absence;
        }

        private IList<KMDAbsenceReport> Build(VacationReport report, KMDAbsenceOperation kmdAbsenceOperation)
        {
            var list = new List<KMDAbsenceReport>();

            var baseAbsence = BuildBase(report, kmdAbsenceOperation);

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
                list.Add(BuildStart(report, kmdAbsenceOperation));
            }

            list.Add(BuildCenter(report, kmdAbsenceOperation));

            if (report.EndTime != null)
            {
                list.Add(BuildEnd(report, kmdAbsenceOperation));
            }

            return list;
        }
    }
}
