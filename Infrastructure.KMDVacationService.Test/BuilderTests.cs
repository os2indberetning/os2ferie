using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;

namespace Infrastructure.KMDVacationService.Test
{
    [TestFixture]
    public class BuilderTests
    {
        [Test]
        public void Create_PeriodWithoutTimeReport()
        {
            var startDate = new DateTime(2016, 4, 16);
            var endDate = new DateTime(2016, 4, 20);
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = startDate.ToTimestamp(),
                EndTimestamp = endDate.ToTimestamp(),
                VacationType = VacationType.Regular
            };

            var list = KMDVacationService.Builder.Create(report);
            
            //Assert
            Assert.AreEqual(1, list.Count);
            var absence = list.First();

            Assert.AreEqual(startDate, absence.StartDate);
            Assert.AreEqual(endDate, absence.EndDate);
        }

        [Test]
        public void Create_SameDayWithTimeReport()
        {
            var startDate = new DateTime(2016, 4, 16);
            var startTime = new TimeSpan(0, 10, 0, 0);
            var endTime = new TimeSpan(0, 14, 0, 0);
            var endDate = new DateTime(2016, 4, 16);
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = startDate.ToTimestamp(),
                StartTime = startTime,
                EndTimestamp = endDate.ToTimestamp(),
                EndTime = endTime,
                VacationType = VacationType.Regular
            };

            var list = KMDVacationService.Builder.Create(report);

            //Assert
            Assert.AreEqual(1, list.Count);
            var absence = list.First();

            Assert.AreEqual(startDate, absence.StartDate);
            Assert.AreEqual(endDate, absence.EndDate);
            Assert.AreEqual(startTime, absence.StartTime);
            Assert.AreEqual(endTime, absence.EndTime);
        }

        [Test]
        public void Create_PeriodeWithEndTimeReport()
        {
            var startDate = new DateTime(2016, 4, 16);
            TimeSpan? startTime = null;
            var endTime = new TimeSpan(0, 14, 0, 0);
            var endDate = new DateTime(2016, 4, 20);
            var report = new VacationReport
            {
                Employment = new Employment
                {
                    EmploymentId = 134123
                },
                StartTimestamp = startDate.ToTimestamp(),
                StartTime = startTime,
                EndTimestamp = endDate.ToTimestamp(),
                EndTime = endTime,
                VacationType = VacationType.Regular
            };

            var list = KMDVacationService.Builder.Create(report);

            //Assert
            Assert.AreEqual(2, list.Count);
            var absence = list.Last();

            //Assert.AreEqual(startDate, absence.StartDate);
            Assert.AreEqual(endDate.AddDays(-1), absence.EndDate);
            Assert.AreEqual(startTime, absence.StartTime);
            Assert.AreEqual(endTime, absence.EndTime);
        }
    }
}
