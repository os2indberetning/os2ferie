using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.OData;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
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

    public class GetResponsibleLeaderTests
    {
        private IGenericRepository<Employment> _emplMock;
        private IGenericRepository<OrgUnit> _orgUnitMock;
        private IGenericRepository<Core.DomainModel.Substitute> _subMock;
        private IReportService<DriveReport> _uut;
        private IReimbursementCalculator _calculatorMock;
        private IGenericRepository<DriveReport> _reportRepoMock;
        private IGenericRepository<RateType> _rateTypeMock;
        private IRoute<RouteInformation> _routeMock;
        private IAddressCoordinates _coordinatesMock;
        private IMailSender _mailMock;
        private List<DriveReport> repoList;
        private ILogger _loggerMock;

        [SetUp]
        public void SetUp()
        {
            var idCounter = 0;

            repoList = new List<DriveReport>();
            _emplMock = Substitute.For<IGenericRepository<Employment>>();
            _calculatorMock = Substitute.For<IReimbursementCalculator>();
            _orgUnitMock = Substitute.For<IGenericRepository<OrgUnit>>();
            _routeMock = Substitute.For<IRoute<RouteInformation>>();
            _coordinatesMock = Substitute.For<IAddressCoordinates>();
            _subMock = Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            _mailMock = Substitute.For<IMailSender>();
            _reportRepoMock = NSubstitute.Substitute.For<IGenericRepository<DriveReport>>();
            _loggerMock = NSubstitute.Substitute.For<ILogger>();

            _reportRepoMock.Insert(new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>()).AndDoes(x => repoList.Add(x.Arg<DriveReport>())).AndDoes(x => x.Arg<DriveReport>().Id = idCounter).AndDoes(x => idCounter++);
            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(repoList.AsQueryable());

            _calculatorMock.Calculate(new RouteInformation(), new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>());

            _coordinatesMock.GetAddressCoordinates(new Address()).ReturnsForAnyArgs(new DriveReportPoint()
            {
                Latitude = "1",
                Longitude = "2",
            });

            _routeMock.GetRoute(DriveReportTransportType.Car, new List<Address>()).ReturnsForAnyArgs(new RouteInformation()
            {
                Length = 2000
            });

            _uut = new DriveReportService(_reportRepoMock, _calculatorMock, _coordinatesMock, _routeMock, _rateTypeMock, _mailMock, _orgUnitMock, _emplMock, _subMock, _loggerMock);

        }

        [Test]
        public void NoSubs_NoLeaderInReportOrg_ShouldReturn_ClosestParentOrgLeader()
        {
            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>().AsQueryable());

            var report = new DriveReport()
            {
                PersonId = 1,
                Person = new Person()
                {
                    Id = 1,
                },
                EmploymentId = 1,
                Employment = new Employment()
                {
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2
                    },
                    OrgUnitId = 2

                }
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1,
                    Parent = new OrgUnit()
                    {
                        Id = 1,
                        Level = 0
                    }
                }
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    PersonId = 1,
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "Jon Badstue"
                    },
                    Id = 1,
                    IsLeader = false,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2
                },
                new Employment()
                {
                    PersonId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                        FullName = "Eva Due",
                    },
                    Id = 12,
                    IsLeader = true,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 1,
                    },
                    OrgUnitId = 1
                },
            }.AsQueryable());

            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("Eva Due", res.FullName);
        }

        [Test]
        public void SubstituteForLeader_ShouldReturnSubstitute()
        {
            var yesterdayStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds;
            var tomorrowStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds;

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                new Core.DomainModel.Substitute()
                {
                    StartDateTimestamp = yesterdayStamp,
                    EndDateTimestamp = tomorrowStamp,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                    },
                    PersonId = 2,
                    LeaderId = 2,
                    Leader = new Person()
                    {
                        Id = 2
                    },
                    Sub = new Person()
                    {
                        Id = 3,
                        FullName = "Heidi Huber"
                    },
                    SubId = 3
                }
            }.AsQueryable());

            var report = new DriveReport()
            {
                PersonId = 1,
                Person = new Person()
                {
                    Id = 1,
                },
                EmploymentId = 1,
                Employment = new Employment()
                {
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2
                    },
                    OrgUnitId = 2

                }
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1,
                    Parent = new OrgUnit()
                    {
                        Id = 1,
                        Level = 0
                    }
                }
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    PersonId = 1,
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "Jon Badstue"
                    },
                    Id = 1,
                    IsLeader = false,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2
                },
                new Employment()
                {
                    PersonId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                        FullName = "Eva Due",
                    },
                    Id = 12,
                    IsLeader = true,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 1,
                    },
                    OrgUnitId = 1
                },
            }.AsQueryable());

            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("Heidi Huber", res.FullName);
        }

        [Test]
        public void PersonalApproverForReportOwner_ShouldReturnPersonalApprover()
        {
            var yesterdayStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds;
            var tomorrowStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds;

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                new Core.DomainModel.Substitute()
                {
                    StartDateTimestamp = yesterdayStamp,
                    EndDateTimestamp = tomorrowStamp,
                    Person = new Person()
                    {
                        Id = 1,
                    },
                    PersonId = 1,
                    LeaderId = 2,
                    Leader = new Person()
                    {
                        Id = 2
                    },
                    Sub = new Person()
                    {
                        Id = 3,
                        FullName = "Heidi Huber Approves"
                    },
                    SubId = 3
                }
            }.AsQueryable());

            var report = new DriveReport()
            {
                PersonId = 1,
                Person = new Person()
                {
                    Id = 1,
                },
                EmploymentId = 1,
                Employment = new Employment()
                {
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2
                    },
                    OrgUnitId = 2

                }
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1,
                    Parent = new OrgUnit()
                    {
                        Id = 1,
                        Level = 0
                    }
                }
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    PersonId = 1,
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "Jon Badstue"
                    },
                    Id = 1,
                    IsLeader = false,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2
                },
                new Employment()
                {
                    PersonId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                        FullName = "Eva Due",
                    },
                    Id = 12,
                    IsLeader = true,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 1,
                    },
                    OrgUnitId = 1
                },
            }.AsQueryable());

            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("Heidi Huber Approves", res.FullName);
        }

        [Test]
        public void PersonalApproverForReportOwner_AndSubstituteForLeader_ShouldReturnPersonalApprover()
        {
            var yesterdayStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds;
            var tomorrowStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds;

            _subMock.AsQueryable().ReturnsForAnyArgs(new List<Core.DomainModel.Substitute>()
            {
                new Core.DomainModel.Substitute()
                {
                    StartDateTimestamp = yesterdayStamp,
                    EndDateTimestamp = tomorrowStamp,
                    Person = new Person()
                    {
                        Id = 1,
                    },
                    PersonId = 1,
                    LeaderId = 2,
                    Leader = new Person()
                    {
                        Id = 2
                    },
                    Sub = new Person()
                    {
                        Id = 3,
                        FullName = "Heidi Huber Approves"
                    },
                    SubId = 3
                },
                new Core.DomainModel.Substitute()
                {
                    StartDateTimestamp = yesterdayStamp,
                    EndDateTimestamp = tomorrowStamp,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                    },
                    PersonId = 2,
                    LeaderId = 2,
                    Leader = new Person()
                    {
                        Id = 2
                    },
                    Sub = new Person()
                    {
                        Id = 3,
                        FullName = "Heidi Huber"
                    },
                    SubId = 3
                }
            }.AsQueryable());

            var report = new DriveReport()
            {
                PersonId = 1,
                Person = new Person()
                {
                    Id = 1,
                },
                EmploymentId = 1,
                Employment = new Employment()
                {
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2
                    },
                    OrgUnitId = 2

                }
            };

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1,
                    Parent = new OrgUnit()
                    {
                        Id = 1,
                        Level = 0
                    }
                }
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    PersonId = 1,
                    Person = new Person()
                    {
                        Id = 1,
                        FullName = "Jon Badstue"
                    },
                    Id = 1,
                    IsLeader = false,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 2,
                    },
                    OrgUnitId = 2
                },
                new Employment()
                {
                    PersonId = 2,
                    Person = new Person()
                    {
                        Id = 2,
                        FullName = "Eva Due",
                    },
                    Id = 12,
                    IsLeader = true,
                    OrgUnit = new OrgUnit()
                    {
                        Id = 1,
                    },
                    OrgUnitId = 1
                },
            }.AsQueryable());

            var res = _uut.GetResponsibleLeaderForReport(report);
            Assert.AreEqual("Heidi Huber Approves", res.FullName);
        }



    }
}
