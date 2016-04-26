using System;
using System.Collections.Generic;
using Core.DomainModel;
using Infrastructure.KMDVacationService.Models;
using Core.DomainServices;

namespace Infrastructure.KMDVacationService
{
    public static class Builder
    {
        public static List<AbsenceObject> Create(VacationReport report)
        {
            var list = new List<AbsenceObject>();

            var startDate = report.StartTimestamp.ToDateTime();
            var endDate = report.EndTimestamp.ToDateTime();

            var sameDay = startDate.Date == endDate.Date;



            if (sameDay)
            {
                // Single report;
                var absence = new AbsenceObject
                {
                    StartTime = report.StartTime,
                    EndTime = report.EndTime,
                    StartDate = startDate,
                    EndDate = endDate,
                    EmploymentId = report.Employment.EmploymentId,
                    ExtraData = "",
                    Operation = Operation.Create,
                    Type = report.VacationType
                };

                list.Add(absence);
            }
            else
            {
                var baseAbsence = new AbsenceObject
                {
                    EmploymentId = report.Employment.EmploymentId,
                    ExtraData = "",
                    Operation = Operation.Create,
                    Type = report.VacationType
                };

                if (report.StartTime == null && report.EndTime == null)
                {
                    baseAbsence.StartDate = startDate;
                    baseAbsence.EndDate = endDate;

                    list.Add(baseAbsence);

                    return list;
                }
                
                var startAbsence = baseAbsence.Copy();
                var endAbsence = baseAbsence.Copy();

                if (report.StartTime != null)
                {
                    baseAbsence.StartDate = startDate.AddDays(1);

                    startAbsence.StartDate = startDate;
                    startAbsence.EndDate = startDate;
                    startAbsence.StartTime = report.StartTime;
                    startAbsence.EndTime = new TimeSpan(0, 23, 59, 59);

                    list.Add(startAbsence);
                }

                if (report.EndTime != null)
                {
                    baseAbsence.EndDate = endDate.AddDays(-1);

                    endAbsence.StartDate = endDate;
                    endAbsence.EndDate = endDate;
                    endAbsence.StartTime = new TimeSpan(0, 0, 0, 0, 0);
                    endAbsence.EndTime = report.EndTime;

                    list.Add(endAbsence);
                }

                list.Add(baseAbsence);
            }

            return list;
        } 
    }
}
