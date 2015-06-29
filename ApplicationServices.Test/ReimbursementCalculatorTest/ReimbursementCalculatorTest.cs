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
            report.Employment = new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            };
            report.EndsAtHome = false;
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.Read;

            var distance = report.Distance;

            var calculator = GetCalculator(new List<Employment>()
            {
                new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            }
            });

            var result = calculator.Calculate(new RouteInformation(){Length = 100}, report);

            Assert.That(distance, Is.EqualTo(result.Distance));
            Assert.That(distance * report.KmRate / 100, Is.EqualTo(result.AmountToReimburse));
        }

        [Test]
        public void Calculate_ReportMethodIsRead_WithFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = true;
            report.Employment = new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            };
            report.StartsAtHome = false;
            report.EndsAtHome = false;
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.Read;

            var distance = report.Distance;

            var calculator = GetCalculator(new List<Employment>()
            {
                new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            }
            });

            var result = calculator.Calculate(new RouteInformation(), report);

            Assert.That(distance - 4, Is.EqualTo(result.Distance));
            Assert.AreEqual((distance - 4) * report.KmRate / 100, result.AmountToReimburse, 0.001);
        }


        /// <summary>
        /// Calculated without allowance
        /// </summary>
        [Test]
        public void Calculate_ReportMethodIsCalculatedWithoutAllowance_WithoutFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = false;
            report.Employment = new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            };
            report.StartsAtHome = true;
            report.EndsAtHome = true;
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.CalculatedWithoutExtraDistance;

            var distance = report.Distance;

            var calculator = GetCalculator(new List<Employment>()
            {
                new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            }
            });

            var result = calculator.Calculate(new RouteInformation(){Length = 42}, report);

            Assert.That(distance * report.KmRate / 100, Is.EqualTo(result.AmountToReimburse));
        }

        [Test]
        public void Calculate_ReportMethodIsCalculatedWithoutAllowance_WithFourKmRule()
        {
            var report = GetDriveReport();
            report.FourKmRule = true;
            report.Employment = new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            };
            report.StartsAtHome = true;
            report.EndsAtHome = true;
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.CalculatedWithoutExtraDistance;

            var distance = report.Distance;

            var calculator = GetCalculator(new List<Employment>()
            {
                new Employment()
            {
                OrgUnit = new OrgUnit()
                {
                    Address = new WorkAddress()
                    {
                        StreetName = "Katrinebjergvej",
                        StreetNumber = "93B",
                        ZipCode = 8200,
                        Town = "Aarhus N"
                    }
                }
            }
            });

            var result = calculator.Calculate(new RouteInformation(){Length = 42}, report);

            Assert.AreEqual(((distance - 4) * report.KmRate / 100), result.AmountToReimburse, 0.001);
        }
    }
}