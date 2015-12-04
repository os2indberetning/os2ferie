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
        public void GetResponsibleLeader_WithNoSub_ShouldGetActualLeader()
        {
            var leader = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var user = new Person()
            {
                Id = 2,
                FirstName = "User",
                LastName = "Usersen",
                Initials = "UU",
                FullName = "User Usersen [UU]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var leaderEmpl = new Employment()
            {
                Id = 1,
                OrgUnit = orgUnit,
                OrgUnitId = 1,
                Person = leader,
                PersonId = leader.Id,
                IsLeader = true
            };

            var userEmpl = new Employment()
            {
                Id = 2,
                OrgUnit = orgUnit,
                PersonId = user.Id,
                OrgUnitId = 1,
                Person = user,
                IsLeader = false
            };

            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                OrgUnitId = 12,
                PersonId = 3,
                LeaderId = 1,
                Sub = new Person()
                {
                    FullName = "En Substitute [ES]"
                },
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             leaderEmpl,userEmpl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new DriveReport()
            {
                    Id = 1,
                    Employment = userEmpl,
                    EmploymentId = userEmpl.Id,
                    PersonId = user.Id,
                    Person = user
            };


            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("Test Testesen [TT]", res.FullName);
        }

        [Test]
        public void GetResponsibleLeader_WithSub_ShouldReturnSub()
        {
            var leader = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var user = new Person()
            {
                Id = 2,
                FirstName = "User",
                LastName = "Usersen",
                Initials = "UU",
                FullName = "User Usersen [UU]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var leaderEmpl = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = leader,
                PersonId = leader.Id,
                IsLeader = true
            };
            var userEmpl = new Employment()
            {
                Id = 2,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = user,
                PersonId = user.Id,
                IsLeader = false
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = leader.Id,
                Person = leader,
                OrgUnitId = leaderEmpl.OrgUnitId,
                LeaderId = leader.Id,
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
             leaderEmpl,userEmpl   
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                orgUnit
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                substitute
            }.AsQueryable());

            var report = new DriveReport()
            {
                    Id = 1,
                    Employment = userEmpl,
                    EmploymentId = userEmpl.Id,
                    PersonId = user.Id,
                    Person = user,
           };


            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("En Substitute [ES]", res.FullName);
        }

        [Test]
        public void GetResponsibleLeader_WithMultipleReports_SomeWithSubSomeWithout_ShouldFindCorrectLeaders()
        {

            var leader1 = new Person()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT",
                FullName = "Test Testesen [TT]"
            };

            var leader2 = new Person()
            {
                Id = 2,
                FirstName = "Test",
                LastName = "Tester",
                Initials = "TT",
                FullName = "Test Tester [TT]"
            };
            
            var user1 = new Person()
            {
                Id = 3,
                FirstName = "User",
                LastName = "Usersen",
                Initials = "UU",
                FullName = "User Usersen [UU]"
            };

            var orgUnit = new OrgUnit()
            {
                Id = 1,
            };

            var orgUnit2 = new OrgUnit()
            {
                Id = 2,
            };

            var leaderEmpl1 = new Employment()
            {
                Id = 1,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = leader1,
                PersonId = leader1.Id,
                IsLeader = true
            };

            var leaderEmpl2 = new Employment()
            {
                Id = 2,
                OrgUnitId = 2,
                OrgUnit = orgUnit2,
                Person = leader2,
                PersonId = leader2.Id,
                IsLeader = true
            };

            var userEmpl1 = new Employment()
            {
                Id = 3,
                OrgUnitId = 1,
                OrgUnit = orgUnit,
                Person = user1,
                PersonId = user1.Id,
                IsLeader = false
            };

            var userEmpl2 = new Employment()
            {
                Id = 4,
                OrgUnitId = 2,
                OrgUnit = orgUnit,
                Person = user1,
                PersonId = user1.Id,
                IsLeader = false
            };



            var substitute = new Core.DomainModel.Substitute()
            {
                Id = 1,
                PersonId = 1,
                LeaderId = 1,
                OrgUnitId = leaderEmpl1.Id,
                Person = leader1,
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
             leaderEmpl1,leaderEmpl2, userEmpl1, userEmpl2
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
                    Employment = userEmpl1,
                    EmploymentId = userEmpl1.Id,
                    PersonId = user1.Id,
                    Person = user1,
                },
                new DriveReport()
                {
                    Id = 2,
                    Employment = userEmpl1,
                    EmploymentId = userEmpl1.Id,
                    PersonId = user1.Id,
                    Person = user1,
                },
                new DriveReport()
                {
                    Id = 3,
                    Employment = userEmpl2,
                    EmploymentId = userEmpl2.Id,
                    PersonId = user1.Id,
                    Person = user1,
                }
            };


            var res0 = _uut.GetResponsibleLeaderForReport(report.AsQueryable().ElementAt(0));
            var res1 = _uut.GetResponsibleLeaderForReport(report.AsQueryable().ElementAt(1));
            var res2 = _uut.GetResponsibleLeaderForReport(report.AsQueryable().ElementAt(2));
            Assert.AreEqual("En Substitute [ES]", res0.FullName);
            Assert.AreEqual("En Substitute [ES]", res1.FullName);
            Assert.AreEqual("Test Tester [TT]", res2.FullName);
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
            var empl = new Employment
            {
                Id = 4,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit(),
                Person = new Person() { Id = 1},
                PersonId = 12,
                IsLeader = false
            };

            var leaderEmpl = new Employment
            {
                Id = 1,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit()
                {
                    Id = 2
                },
                Person = new Person() { Id = 13 },
                PersonId = 13,
                IsLeader = true
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 2
                }
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>().AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl, leaderEmpl
            }.AsQueryable());

            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Read,
                Distance = 12,
                Purpose = "Test",
                PersonId = 12,
                EmploymentId = 4,
            };
            _uut.Create(report);
            _calculatorMock.ReceivedWithAnyArgs().Calculate(new RouteInformation(), report);
        }

        [Test]
        public void Create_ValidCalculatedReport_ShouldCallAddressCoordinates()
        {
            var empl = new Employment
            {
                Id = 4,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit(),
                Person = new Person() { Id = 1 },
                PersonId = 12,
                IsLeader = false
            };

            var leaderEmpl = new Employment
            {
                Id = 1,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit()
                {
                    Id = 2
                },
                Person = new Person() { Id = 13 },
                PersonId = 13,
                IsLeader = true
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 2
                }
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>().AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl, leaderEmpl
            }.AsQueryable());

            var report = new DriveReport
            {
                KilometerAllowance = KilometerAllowance.Calculated,
                DriveReportPoints = new List<DriveReportPoint>
                {
                    new DriveReportPoint(),
                    new DriveReportPoint()
                },
                PersonId = 12,
                EmploymentId = 4,
                Purpose = "Test",
                TFCode = "1234",
                    
            };
            _uut.Create(report);
            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new DriveReportPoint());
        }

        [Test]
        public void Create_ValidCalculatedReport_DistanceLessThanZero_ShouldSetDistanceToZero()
        {
            var empl = new Employment
            {
                Id = 4,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit(),
                Person = new Person() { Id = 1 },
                PersonId = 12,
                IsLeader = false
            };

            var leaderEmpl = new Employment
            {
                Id = 1,
                OrgUnitId = 2,
                OrgUnit = new OrgUnit()
                {
                    Id = 2
                },
                Person = new Person() { Id = 13 },
                PersonId = 13,
                IsLeader = true
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 2
                }
            }.AsQueryable());

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>().AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
             empl, leaderEmpl
            }.AsQueryable());

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
                PersonId = 12,
                EmploymentId = 4,
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
