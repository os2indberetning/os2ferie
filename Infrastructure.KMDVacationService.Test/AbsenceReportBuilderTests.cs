using System;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.KMDVacationService.Models;
using NUnit.Framework;

namespace Infrastructure.KMDVacationService.Test
{
    [TestFixture]
    public class AbsenceReportBuilderTests
    {
        [Test]
        public void Create_PeriodWithoutTimeReport_Returns1Report()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Create_Report_ReturnsCreateOperation()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(Operation.Create, list.First().Operation);
        }

        [Test]
        public void Create_PeriodWithoutTimeReport_SetsOperationToCreate()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(Operation.Create, absence.Operation);
        }

        public void Create_PeriodWithoutTimeReport_TimeIsNull()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();
            Assert.AreEqual(null, absence.StartTime);
            Assert.AreEqual(null, absence.EndTime);
        }

        [Test]
        public void Create_SameDayWithTimeReport_Returns1Report()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 10, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTime = new TimeSpan(0, 14, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Create_SameDayWithTimeReport_SetsTime()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 10, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTime = new TimeSpan(0, 14, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(report.StartTime, absence.StartTime);
            Assert.AreEqual(report.EndTime, absence.EndTime);
        }

        [Test]
        public void Create_SameDayWithTimeReport_StartAndEndDateAreEqual()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 10, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTime = new TimeSpan(0, 14, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.True(absence.StartDate.Date == absence.EndDate.Date);
        }

        [Test]
        public void Create_PeriodeWithEndTimeReport_Returns2Reports()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = null,
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = new TimeSpan(0, 14, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(2, list.Count);
        }


        [Test]
        public void Create_PeriodeWithStartTimeReport_Returns2Reports()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 14, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = null,
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_Returns3Reports()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 14, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = new TimeSpan(0, 10, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_EndAbsenceHasEndTimeSet()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 14, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = new TimeSpan(0, 10, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.Last();

            Assert.AreEqual(report.EndTime, absence.EndTime);
            Assert.AreEqual(null, absence.StartTime);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_EndAbsenceHasStartTimeSet()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 14, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = new TimeSpan(0, 10, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(report.StartTime, absence.StartTime);
            Assert.AreEqual(null, absence.EndTime);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_CenterAbsencePeriodIsDecremented()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                StartTime = new TimeSpan(0, 14, 0, 0),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                EndTime = new TimeSpan(0, 10, 0, 0),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list[1];

            Assert.AreEqual(report.StartTimestamp.ToDateTime().AddDays(1), absence.StartDate);
            Assert.AreEqual(report.EndTimestamp.ToDateTime().AddDays(-1), absence.EndDate);
        }

        [Test]
        public void Delete_Report_OperationIsDelete()
        {
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Delete(report);

            Assert.AreEqual(Operation.Delete, list.First().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_Returns2Reports()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Delete, list.First().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_OldStartTimeIsDeleted()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Delete, list.First().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_BasePeriodOperationIsEdited()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Edit, list.Last().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_Returns2Reports()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_OldEndTimeIsDeleted()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Delete, list.Last().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_BasePeriodOperationIsEdited()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Edit, list.First().Operation);
        }

        [Test]
        public void Edit_OnlyBasePeriodEdited_BasePeriodOperationIsEdited()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Edit, list.First().Operation);
        }

        [Test]
        public void Edit_OnlyBasePeriodEdited_Returns1Report()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(1, list.Count);
        }


        [Test]
        public void Edit_NewReportWithoutTimes_Returns3Reports()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_DeletesStarTimeReport()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Delete, list.First().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_DeletesEndTimeReport()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Delete, list.Last().Operation);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_EditsBaseReport()
        {
            var oldReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var newReport = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 22).ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(Operation.Edit, list[1].Operation);
        }

    }
}
