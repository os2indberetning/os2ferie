using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using Ninject;
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
        public void Calculate_WithRoundTrip_ShouldDoubleDistance()
        {
            var report = GetDriveReport();
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
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.Read;
            report.IsRoundTrip = true;

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

            var result = calculator.Calculate(new RouteInformation() { Length = 100 }, report);

            Assert.AreEqual(84, report.Distance);

        }

        [Test]
        public void Calculate_WithoutRoundTrip_ShouldNotDoubleDistance()
        {
            var report = GetDriveReport();
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
            report.Distance = 42;
            report.KilometerAllowance = KilometerAllowance.Read;
            report.IsRoundTrip = false;

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

            var result = calculator.Calculate(new RouteInformation() { Length = 100 }, report);

            Assert.AreEqual(42, report.Distance);

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

        [Test]
        public void ReportWhereAddressHistoryExistsForPeriod_ShouldUseAddressHistory_AndSetTrueRouteBeginsAtHome()
        {
            var historyMockData = new List<AddressHistory>()
            {
                new AddressHistory
            {
                StartTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
                WorkAddress = new WorkAddress
                {
                    StreetName = "TestWorkAddress",
                    StreetNumber = "123",
                    ZipCode = 1234,
                    Town = "TestTown",
                    Latitude = "5555",
                    Longitude = "5555"
                },
                HomeAddress = new PersonalAddress
                {
                    StreetName = "TestHomeAddress",
                    StreetNumber = "123",
                    ZipCode = 1234,
                    Town = "TestTown",
                    Latitude = "1234",
                    Longitude = "1234",
                    Type = PersonalAddressType.OldHome
                },
                EmploymentId = 1,
                Id = 1
            }
            };

            var emplMockData = new List<Employment>
            {
                new Employment
                {
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "TestPerson"
                    },
                    OrgUnit = new OrgUnit()
                    {
                        Address = new WorkAddress()
                        {
                             StreetName = "RealTestWork",
                             StreetNumber = "123",
                             ZipCode = 1234,
                             Town = "TestTown",
                             Latitude = "9999",
                             Longitude = "9999"
                        }
                    },
                    PersonId = 1,
                    Id = 1,
                }
            };

            var calculator = GetCalculator(emplMockData, historyMockData);

            var driveReport = new DriveReport()
            {
                Id = 1,
                PersonId = 1,
                EmploymentId = 1,
                DriveDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint()
                    {
                        Latitude = "1234",
                        Longitude = "1234"
                    },
                    new DriveReportPoint()
                    {
                        Latitude = "5555",
                        Longitude = "5555"
                    }
                }
            };

            calculator.Calculate(new RouteInformation(){Length = 123}, driveReport);
            Assert.AreEqual(true, driveReport.StartsAtHome);
            Assert.AreEqual(false, driveReport.EndsAtHome);
        }

        [Test]
        public void ReportWhereAddressHistoryExistsForPeriod_ShouldUseAddressHistory_AndSetTrueRouteEndsAtHome()
        {
            var historyMockData = new List<AddressHistory>()
            {
                new AddressHistory
            {
                StartTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
                WorkAddress = new WorkAddress
                {
                    StreetName = "TestWorkAddress",
                    StreetNumber = "123",
                    ZipCode = 1234,
                    Town = "TestTown",
                    Latitude = "5555",
                    Longitude = "5555"
                },
                HomeAddress = new PersonalAddress
                {
                    StreetName = "TestHomeAddress",
                    StreetNumber = "123",
                    ZipCode = 1234,
                    Town = "TestTown",
                    Latitude = "1234",
                    Longitude = "1234",
                    Type = PersonalAddressType.OldHome
                },
                EmploymentId = 1,
                Id = 1
            }
            };

            var emplMockData = new List<Employment>
            {
                new Employment
                {
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "TestPerson"
                    },
                    OrgUnit = new OrgUnit()
                    {
                        Address = new WorkAddress()
                        {
                             StreetName = "RealTestWork",
                             StreetNumber = "123",
                             ZipCode = 1234,
                             Town = "TestTown",
                             Latitude = "9999",
                             Longitude = "9999"
                        }
                    },
                    PersonId = 1,
                    Id = 1,
                }
            };

            var calculator = GetCalculator(emplMockData, historyMockData);

            var driveReport = new DriveReport()
            {
                Id = 1,
                PersonId = 1,
                EmploymentId = 1,
                DriveDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint()
                    {
                        Latitude = "5555",
                        Longitude = "5555"
                    },
                     new DriveReportPoint()
                    {
                        Latitude = "1234",
                        Longitude = "1234"
                    }
                }
            };

            calculator.Calculate(new RouteInformation() { Length = 123 }, driveReport);
            Assert.AreEqual(true, driveReport.EndsAtHome);
            Assert.AreEqual(false, driveReport.StartsAtHome);
        }

        [Test]
        public void ReportWhereNoAddressHistoryExistsForPeriod_ShouldNotUseAddressHistory_AndNotSetTrueRouteBeginsOrEndsAtHome()
        {
            var historyMockData = new List<AddressHistory>()
            {
            };

            var emplMockData = new List<Employment>
            {
                new Employment
                {
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "TestPerson"
                    },
                    OrgUnit = new OrgUnit()
                    {
                        Address = new WorkAddress()
                        {
                             StreetName = "RealTestWork",
                             StreetNumber = "123",
                             ZipCode = 1234,
                             Town = "TestTown",
                             Latitude = "9999",
                             Longitude = "9999"
                        }
                    },
                    PersonId = 1,
                    Id = 1,
                }
            };

            var calculator = GetCalculator(emplMockData, historyMockData);

            var driveReport = new DriveReport()
            {
                Id = 1,
                PersonId = 1,
                EmploymentId = 1,
                DriveDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint()
                    {
                        Latitude = "5555",
                        Longitude = "5555"
                    },
                     new DriveReportPoint()
                    {
                        Latitude = "1234",
                        Longitude = "1234"
                    }
                }
            };

            calculator.Calculate(new RouteInformation() { Length = 123 }, driveReport);
            Assert.AreEqual(false, driveReport.EndsAtHome);
            Assert.AreEqual(false, driveReport.StartsAtHome);
        }
    }
}