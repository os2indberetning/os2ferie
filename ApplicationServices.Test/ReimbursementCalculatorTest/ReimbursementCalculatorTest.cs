using System.Collections.Generic;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.ReimbursementCalculatorTest
{
    [TestFixture]
    public class ReimbursementCalculatorTest : ReimbursementCalculatorBaseTest
    {
        /// <summary>
        /// Read
        /// </summary>
        [Test]
        public void Calculate_ReportMethodIsRead_WithoutFourKmRule()
        {           
            var report = GetDriveReport();
            report.FourKmRule = false;
            report.StartsAtHome = false;
            report.EndsAtHome = false;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIsRead);

            Assert.That(distance, Is.EqualTo(result.Distance));
            Assert.That(distance * report.KmRate, Is.EqualTo(result.AmountToReimburse));
        }

        [Test]
        public void Calculate_ReportMethodIsRead_WithFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = true;
            report.StartsAtHome = false;
            report.EndsAtHome = false;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIsRead);

            Assert.That(distance - 4, Is.EqualTo(result.Distance));
            Assert.That((distance - 4) * report.KmRate, Is.EqualTo(result.AmountToReimburse));
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Test]
        public void Calculate_ReportMethodIsCalculated_WithoutFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = false;
            report.StartsAtHome = false;
            report.EndsAtHome = false;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIscalculated);

            Assert.That((distance) * report.KmRate, Is.EqualTo(result.AmountToReimburse));
        }

        [Test]
        public void Calculate_ReportMethodIsCalculated_WithFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = true;
            report.StartsAtHome = false;
            report.EndsAtHome = false;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIscalculated);

            Assert.That((distance - 4) * report.KmRate, Is.EqualTo(result.AmountToReimburse));
        }

        /// <summary>
        /// Calculated without allowance
        /// </summary>
        [Test]
        public void Calculate_ReportMethodIsCalculatedWithoutAllowance_WithoutFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = false;
            report.StartsAtHome = true;
            report.EndsAtHome = true;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIscalculatedwithoutallowance);

            Assert.That(distance * report.KmRate, Is.EqualTo(result.AmountToReimburse));
        }
        
        [Test]
        public void Calculate_ReportMethodIsCalculatedWithoutAllowance_WithFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = true;
            report.StartsAtHome = true;
            report.EndsAtHome = true;
            report.Distance = 42;

            var distance = report.Distance;

            var calculator = GetCalculator();

            var result = calculator.Calculate(report, reportMethodIscalculatedwithoutallowance);

            Assert.That(((distance - 4) * report.KmRate), Is.EqualTo(result.AmountToReimburse));
        }
    }
}