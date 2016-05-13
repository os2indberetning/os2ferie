using System;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;

namespace Infrastructure.KMDVacationService.Test
{
    [TestFixture]
    public class AbsenceReportBuilderTests
    {

        public VacationReport CreateReportWithoutTime()
        {
            return new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };
        }

        public VacationReport CreateReportWithStartTime()
        {
            return new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTime = new TimeSpan(0, 10, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };
        }

        public VacationReport CreateReportWithEndTime()
        {
            return new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                EndTime = new TimeSpan(0, 14, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };
        }

        public VacationReport CreateReportWithTime()
        {
            return new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                EndTime = new TimeSpan(0, 14, 0, 0),
                StartTime = new TimeSpan(0, 10, 0, 0),
                StartTimestamp = new DateTime(2016, 4, 16).ToTimestamp(),
                EndTimestamp = new DateTime(2016, 4, 20).ToTimestamp(),
                VacationType = VacationType.Regular
            };
        }

        [Test]
        public void Create_PeriodWithoutTimeReport_Returns1Report()
        {
            var report = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Create_Report_ReturnsCreateOperation()
        {
            var report = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(KMDAbsenceOperation.Create, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Create_PeriodWithoutTimeReport_SetsOperationToCreate()
        {
            var report = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(KMDAbsenceOperation.Create, absence.KmdAbsenceOperation);
        }

        public void Create_PeriodWithoutTimeReport_TimeIsNull()
        {
            var report = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();
            Assert.AreEqual(null, absence.StartTime);
            Assert.AreEqual(null, absence.EndTime);
        }

        [Test]
        public void Create_SameDayWithTimeReport_Returns1Report()
        {
            var report = CreateReportWithTime();
            report.StartTimestamp = new DateTime(2016, 10, 10).ToTimestamp();
            report.EndTimestamp = new DateTime(2016, 10, 10).ToTimestamp();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Create_SameDayWithTimeReport_SetsTime()
        {
            var report = CreateReportWithTime();
            report.StartTimestamp = new DateTime(2016, 10, 10).ToTimestamp();
            report.EndTimestamp = new DateTime(2016, 10, 10).ToTimestamp();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(report.StartTime, absence.StartTime);
            Assert.AreEqual(report.EndTime, absence.EndTime);
        }

        [Test]
        public void Create_SameDayWithTimeReport_StartAndEndDateAreEqual()
        {
            var report = CreateReportWithTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.True(absence.StartDate.Date == absence.EndDate.Date);
        }

        [Test]
        public void Create_PeriodeWithEndTimeReport_Returns2Reports()
        {
            var report = CreateReportWithEndTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(2, list.Count);
        }


        [Test]
        public void Create_PeriodeWithStartTimeReport_Returns2Reports()
        {
            var report = CreateReportWithStartTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_Returns3Reports()
        {
            var report = CreateReportWithTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);

            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_EndAbsenceHasEndTimeSet()
        {
            var report = CreateReportWithTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.Last();

            Assert.AreEqual(report.EndTime, absence.EndTime);
            Assert.AreEqual(null, absence.StartTime);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_EndAbsenceHasStartTimeSet()
        {
            var report = CreateReportWithTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list.First();

            Assert.AreEqual(report.StartTime, absence.StartTime);
            Assert.AreEqual(null, absence.EndTime);
        }

        [Test]
        public void Create_PeriodeWithStartAndEndTimeReport_CenterAbsencePeriodIsDecremented()
        {
            var report = CreateReportWithTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Create(report);
            var absence = list[1];

            Assert.AreEqual(report.StartTimestamp.ToDateTime().AddDays(1), absence.StartDate);
            Assert.AreEqual(report.EndTimestamp.ToDateTime().AddDays(-1), absence.EndDate);
        }

        [Test]
        public void Delete_Report_OperationIsDelete()
        {
            var report = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Delete(report);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_Returns2Reports()
        {
            var oldReport = CreateReportWithStartTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_OldStartTimeIsDeleted()
        {
            var oldReport = CreateReportWithStartTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutStartTime_BasePeriodOperationIsEdited()
        {
            var oldReport = CreateReportWithStartTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Edit, list.Last().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_Returns2Reports()
        {
            var oldReport = CreateReportWithEndTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_OldEndTimeIsDeleted()
        {
            var oldReport = CreateReportWithEndTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.Last().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutEndTime_BasePeriodOperationIsEdited()
        {
            var oldReport = CreateReportWithEndTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Edit, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_OnlyBasePeriodEdited_BasePeriodOperationIsEdited()
        {
            var oldReport = CreateReportWithoutTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Edit, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_OnlyBasePeriodEdited_Returns1Report()
        {
            var oldReport = CreateReportWithoutTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(1, list.Count);
        }


        [Test]
        public void Edit_NewReportWithoutTimes_Returns3Reports()
        {
            var oldReport = CreateReportWithTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_DeletesStarTimeReport()
        {
            var oldReport = CreateReportWithTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.First().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_DeletesEndTimeReport()
        {
            var oldReport = CreateReportWithTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Delete, list.Last().KmdAbsenceOperation);
        }

        [Test]
        public void Edit_NewReportWithoutTimes_EditsBaseReport()
        {
            var oldReport = CreateReportWithTime();
            var newReport = CreateReportWithoutTime();

            var builder = new KMDAbsenceReportBuilder();

            var list = builder.Edit(oldReport, newReport);

            Assert.AreEqual(KMDAbsenceOperation.Edit, list[1].KmdAbsenceOperation);
        }

    }
}
