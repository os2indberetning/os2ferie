using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.OData;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Microsoft.Owin.Testing;
using Ninject;
using NUnit.Framework;
using OS2Indberetning;
using Owin;
using Presentation.Web.Test.Controllers.DriveReports;
using Presentation.Web.Test.Controllers.PersonalRoutes;
using NSubstitute;
using Presentation.Web.Test;
using Substitute = NSubstitute.Substitute;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture] //TODO rewrite tests, did not catch that the person was always set as responsible leader
    /** Things to test: 
     *      person is an employee
     *      person is a leader (approver is leader of next level
     *      Person is leader on two levels
     *      Person has personal approver
     *      persons leader has substitute
     */

    public class DriveReportServiceTests
    {
        private IGenericRepository<Employment> _emplMock;
        private IGenericRepository<OrgUnit> _orgUnitMock;
        private IGenericRepository<Core.DomainModel.Substitute> _subMock;
        private IDriveReportService _uut;
        private IReimbursementCalculator _calculatorMock;
        private IGenericRepository<DriveReport> _reportRepoMock;
        private IGenericRepository<RateType> _rateTypeMock;
        private IRoute<RouteInformation> _routeMock;
        private IAddressCoordinates _coordinatesMock;
        private IMailSender _mailMock;
        private List<DriveReport> repoList;

        [SetUp]
        public void SetUp()
        {
            var idCounter = 0;

            repoList = new List<DriveReport>();
            _emplMock = Substitute.For<IGenericRepository<Employment>>();
            _calculatorMock = Substitute.For<IReimbursementCalculator>();
            _orgUnitMock = Substitute.For<IGenericRepository<OrgUnit>>();
            _rateTypeMock = Substitute.For<IGenericRepository<RateType>>();
            _routeMock = Substitute.For<IRoute<RouteInformation>>();
            _coordinatesMock = Substitute.For<IAddressCoordinates>();
            _subMock = Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            _mailMock = Substitute.For<IMailSender>();
            _reportRepoMock = NSubstitute.Substitute.For<IGenericRepository<DriveReport>>();

            _reportRepoMock.Insert(new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>()).AndDoes(x => repoList.Add(x.Arg<DriveReport>())).AndDoes(x => x.Arg<DriveReport>().Id = idCounter).AndDoes(x => idCounter++);
            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(repoList.AsQueryable());

            _calculatorMock.Calculate(new RouteInformation(), new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>());

            _rateTypeMock.AsQueryable().ReturnsForAnyArgs(new List<RateType>
            {
                new RateType()
                {
                    TFCode = "1234",
                    IsBike = false,
                    RequiresLicensePlate = true,
                    Id = 1,
                    Description = "TestRate"
                }
            }.AsQueryable());

            _coordinatesMock.GetAddressCoordinates(new Address()).ReturnsForAnyArgs(new DriveReportPoint()
            {
                Latitude = "1",
                Longitude = "2",
            });

            _routeMock.GetRoute(DriveReportTransportType.Car, new List<Address>()).ReturnsForAnyArgs(new RouteInformation()
            {
                Length = 2000
            });

            _uut = new DriveReportService(_mailMock, _reportRepoMock, _calculatorMock, _orgUnitMock, _emplMock, _subMock, _coordinatesMock, _routeMock, _rateTypeMock);

        }

        
        [Test]
        public void AttachResponsibleLeader_WithNoSub_ShouldAttachLeader()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var empl = new Employment()
            {
                Id = 1,
                OrgUnit = orgUnit,
                OrgUnitId = 1,
                Person = person,
                IsLeader = true
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person
                }
            };


            var res = _uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(0).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithSub_ShouldAttachSub()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = person,
                IsLeader = true
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                Person = person,
                LeaderId = 1,
                Sub = new Person()
                {
                    Id = 3,
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES",
                    FullName = "En Substitute [ES]"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person,
                }
            };


            var res = _uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_WithSub_ShouldAttachSub()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var empl = new Employment()
            {
                Id = 1,
                OrgUnit = orgUnit,
                OrgUnitId = 1,
                Person = person,
                IsLeader = true
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                Person = person,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES",
                    FullName = "En Substitute [ES]"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person,
                }
            };


            var res = _uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(2).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_WithoutSub_ShouldAttachLeader()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };


            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = person,
                IsLeader = true
            };


            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 2,
                LeaderId = 1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES",
                    FullName = "En Substitute [ES]"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person
                }
            };


            var res = _uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Testesen [TT]", res.ElementAt(2).ResponsibleLeader.FullName);
        }

        [Test]
        public void AttachResponsibleLeader_WithMultipleReports_SomeWithSubSomeWithout_ShouldAttachCorrectly()
        {

            var person1 = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var person2 = new Person()
            {
                Id = 2,
                FirstName = "Test",
                LastName = "Tester",
                Initials = "TT",
                FullName = "Test Tester [TT]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var orgUnit2 = new OrgUnit()
            {
                Id = 2,
            };

            var empl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = person1,
                IsLeader = true
            };

            var empl2 = new Employment()
            {
                Id = 1,
                OrgUnitId = 2,
                OrgUnit = orgUnit2,
                Person = person2,
                IsLeader = true
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                LeaderId = 1,
                Person = person1,
                Sub = new Person()
                {
                    FirstName = "En",
                    LastName = "Substitute",
                    Initials = "ES",
                    FullName = "En Substitute [ES]"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl,empl2
            }.AsQueryable());


            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit,orgUnit2
            }.AsQueryable());


            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());



            var report = new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl,
                    PersonId = 1,
                    Person = person1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Employment = empl2,
                    PersonId = 1,
                    Person = person1,
                }
            };


            var res = _uut.AttachResponsibleLeader(report.AsQueryable());
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(0).ResponsibleLeader.FullName);
            Assert.AreEqual("En Substitute [ES]", res.ElementAt(1).ResponsibleLeader.FullName);
            Assert.AreEqual("Test Tester [TT]", res.ElementAt(2).ResponsibleLeader.FullName);
        }

        [Test]
        public void ReportWithRead_AndDistanceZero_ShouldReturn_False()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Read,
                Distance = 0
            };

            var res = _uut.Validate(report);
            Assert.IsFalse(res);
        }

        [Test]
        public void ReportWithCalculated_AndOnePoint_ShouldReturn_False()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint()
                }
            };

            var res = _uut.Validate(report);
            Assert.IsFalse(res);
        }

        [Test]
        public void ReportWithNoPurpose_ShouldReturn_False()
        {
            var report = new DriveReport
            {
               KilometerAllowance = KilometerAllowance.Read,
               Distance = 10
            };

            var res = _uut.Validate(report);
            Assert.IsFalse(res);
        }

        [Test]
        public void ReportWith_PurposeReadCorrectDistance_ShouldReturn_True()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Read,
                Distance = 10,
                Purpose = "Test"
            };

            var res = _uut.Validate(report);
            Assert.IsTrue(res);
        }

        [Test]
        public void ReportWith_PurposeCalculatedTwoPoints_ShouldReturn_True()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint(),
                    new DriveReportPoint(),
                },
                Purpose = "Test"
            };

            var res = _uut.Validate(report);
            Assert.IsTrue(res);
        }


        [Test]
        public void ReportWith_CalculatedDistance7_ShouldReturn_False()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                Distance = 7,
                DriveReportPoints = new List<DriveReportPoint>(),
                Purpose = "Test"
            };

            var res = _uut.Validate(report);
            Assert.IsFalse(res);
        }


        [Test]
        public void Create_InvalidReport_ShouldThrowException()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                Distance = 7,
                Purpose = "Test"
            };
            Assert.Throws<Exception>(() => _uut.Create(report));
        }

        [Test]
        public void Create_InvalidReadReport_ShouldThrowException()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Read,
                Purpose = "Test"
            };
            Assert.Throws<Exception>(() => _uut.Create(report));
        }

        [Test]
        public void Create_ValidReadReport_ShouldCallCalculate()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Read,
                Distance = 12,
                Purpose = "Test",
                PersonId = 1
            };
            _uut.Create(report);
            _calculatorMock.ReceivedWithAnyArgs().Calculate(new RouteInformation(), report);
        }

        [Test]
        public void Create_ValidCalculatedReport_ShouldCallAddressCoordinates()
        {
            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint(),
                    new DriveReportPoint()
                },
                PersonId = 1,
                Purpose = "Test",
                TFCode = "1234",
                    
            };
            _uut.Create(report);
            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new DriveReportPoint());
        }

        [Test]
        public void Create_ValidCalculatedReport_DistanceLessThanZero_ShouldSetDistanceToZero()
        {
            _routeMock.GetRoute(DriveReportTransportType.Car, new List<Address>()).ReturnsForAnyArgs(new RouteInformation()
            {
                Length = -10
            });

            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint(),
                    new DriveReportPoint(),
                    new DriveReportPoint()
                },
                PersonId = 1,
                Purpose = "Test",
                TFCode = "1234",

            };
            var res = _uut.Create(report);
            Assert.AreEqual(0,res.Distance);
        }

        [Test]
        public void RejectedReport_shouldCall_SendMail_WithCorrectParameters()
        {

            var delta = new Delta<DriveReport>(typeof(DriveReport));
            delta.TrySetPropertyValue("Status", ReportStatus.Rejected);
            delta.TrySetPropertyValue("Comment", "Afvist, du");

            repoList.Add(new DriveReport
            {
                Id = 1,
                Status = ReportStatus.Pending,
                Person = new Person
                {
                    Mail = "test@mail.dk",
                    FullName = "TestPerson"
                }
            });

            _uut.SendMailIfRejectedReport(1, delta);
            _mailMock.Received().SendMail("test@mail.dk","Afvist indberetning","Din indberetning er blevet afvist med kommentaren: \n \n" + "Afvist, du");
        }

        [Test]
        public void RejectedReport_PersonWithNoMail_ShouldThrowException()
        {

            var delta = new Delta<DriveReport>(typeof(DriveReport));
            delta.TrySetPropertyValue("Status", ReportStatus.Rejected);
            delta.TrySetPropertyValue("Comment", "Afvist, du");

            repoList.Add(new DriveReport
            {
                Id = 1,
                Status = ReportStatus.Pending,
                Person = new Person
                {
                    Mail = "",
                    FullName = "TestPerson"
                }
            });

            Assert.Throws<Exception>(() => _uut.SendMailIfRejectedReport(1, delta));
        }



       
    }
}
