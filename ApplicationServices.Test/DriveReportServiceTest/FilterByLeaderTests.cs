using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using NSubstitute;
using NUnit.Framework;
using Substitute = Core.DomainModel.Substitute;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture]
    class FilterByLeaderTests
    {
        private Person leader1, leader2;
        private OrgUnit org1, org2;
        private Employment leaderEmpl1, leaderEmpl2;
        private IGenericRepository<Employment> _emplMock;
        private IReimbursementCalculator _calculatorMock;
        private IGenericRepository<OrgUnit> _orgUnitMock;
        private IRoute<RouteInformation> _routeMock;
        private IAddressCoordinates _coordinatesMock;
        private IGenericRepository<Substitute> _subRepoMock;
        private DriveReportService _uut;
        private IGenericRepository<DriveReport> _reportRepoMock;
        private IGenericRepository<RateType> _rateTypeMock;
        
        [SetUp]
        public void SetUp()
        {

            var idCounter = 0;
            var repoList = new List<DriveReport>();

            _emplMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _calculatorMock = NSubstitute.Substitute.For<IReimbursementCalculator>();
            _orgUnitMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            _routeMock = NSubstitute.Substitute.For<IRoute<RouteInformation>>();
            _rateTypeMock = NSubstitute.Substitute.For<IGenericRepository<RateType>>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _subRepoMock = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            _uut = new DriveReportService(NSubstitute.Substitute.For<IMailSender>(), NSubstitute.Substitute.For<IGenericRepository<DriveReport>>(), _calculatorMock, _orgUnitMock, _emplMock, _subRepoMock, _coordinatesMock, _routeMock,_rateTypeMock);
            _reportRepoMock = NSubstitute.Substitute.For<IGenericRepository<DriveReport>>();

            _reportRepoMock.Insert(new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>()).AndDoes(x => repoList.Add(x.Arg<DriveReport>())).AndDoes(x => x.Arg<DriveReport>().Id = idCounter).AndDoes(x => idCounter++);
            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(repoList.AsQueryable());

            _calculatorMock.Calculate(new RouteInformation(){Length = 100}, new DriveReport()).ReturnsForAnyArgs(x => x.Arg<DriveReport>());

            _coordinatesMock.GetAddressCoordinates(new Address()).ReturnsForAnyArgs(new Address()
            {
                Latitude = "1",
                Longitude = "2",
            });

            leader1 = new Person()
            {
                Id = 1,
                FirstName = "Jacob",
                LastName = "Jensen",
                Initials = "JJ",
            };

            leader2 = new Person()
            {
                Id = 2,
                FirstName = "Morten",
                LastName = "Rasmussen",
                Initials = "MR",
            };

            org1 = new OrgUnit()
            {
                Id = 1,
                Level = 0,
            };

            org2 = new OrgUnit()
            {
                Id = 2,
                Level = 1,
                ParentId = 1
            };

            leaderEmpl1 = new Employment()
            {
                Person = leader1,
                OrgUnit = org1,
                OrgUnitId = 1,
                IsLeader = true
            };

            leaderEmpl2 = new Employment()
            {
                Person = leader2,
                OrgUnitId = 2,
                OrgUnit = org2,
                IsLeader = true
            };
        }

        [Test]
        public void OneDriveReport_ForLeaderOfChild_NoReportsInOwnOrg_ShouldReturnChildLeaderReport()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());


            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment",
                    Employment = leaderEmpl2,
                }
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(1, res.Count());
        }

        [Test]
        public void FiveDriveReports_ForLeaderOfChild_NoReportsInOwnOrg_ShouldReturnChildLeaderReports()
        {

            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());

            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment3",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment4",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment5",
                    Employment = leaderEmpl2,
                }
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(5, res.Count());
        }

        [Test]
        public void FiveDriveReports_ForLeaderOfChild_AndInOwnOrg_ShouldReturnChildLeaderAndOwnOrgReports()
        {
           
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());


            var org1Person = new Person()
            {
                Id = 3,
                FirstName = "Test",
                LastName = "Testesen",
                Initials = "TT"
            };

            var org1PersonEmpl = new Employment()
            {
                Id = 3,
                Person = org1Person,
                IsLeader = false,
                OrgUnit = org1
            };


            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment3",
                    Employment = leaderEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = org1Person,
                    Comment = "TestComment4",
                    Employment = org1PersonEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = org1Person,
                    Comment = "TestComment5",
                    Employment = org1PersonEmpl,
                }
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, org1PersonEmpl
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(4, res.Count());
        }

        [Test]
        public void OnlyReportsForLeader_ShouldReturn_Nothing()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());


            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment1",
                    Employment = leaderEmpl1,
                },
                new DriveReport()
                {
                    Id = 2,
                    Person = leader1,
                    Comment = "TestComment2",
                    Employment = leaderEmpl1,
                },
                new DriveReport()
                {
                    Id = 3,
                    Person = leader1,
                    Comment = "TestComment3",
                    Employment = leaderEmpl1,
                },
                new DriveReport()
                {
                    Id = 4,
                    Person = leader1,
                    Comment = "TestComment4",
                    Employment = leaderEmpl1,
                },
                new DriveReport()
                {
                    Id = 5,
                    Person = leader1,
                    Comment = "TestComment5",
                    Employment = leaderEmpl1,
                },
                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void OnlyReportsForNonRelatedOrgUnit_ShouldReturn_Nothing()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());

            var notRelatedOrg = new OrgUnit()
            {
                Id = 4,
                Level = 0,
            };

            var notRelatedEmpl = new Employment()
            {
                OrgUnit = notRelatedOrg,
                Person = leader2,
                IsLeader = false,
            };

            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment1",
                    Employment = notRelatedEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = notRelatedEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment3",
                    Employment = notRelatedEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment4",
                    Employment = notRelatedEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment5",
                    Employment = notRelatedEmpl,
                },

                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2, notRelatedOrg
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, notRelatedEmpl
                
            }.AsQueryable());     

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void OnlyReportsForSeveralNonRelatedOrgUnits_ShouldReturn_Nothing()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());

            var notRelatedOrg1 = new OrgUnit()
            {
                Id = 4,
                Level = 0,
            };

            var notRelatedEmpl1 = new Employment()
            {
                OrgUnit = notRelatedOrg1,
                Person = leader2,
                IsLeader = false,
            };

            var notRelatedPerson2 = new Person()
            {
                Id = 22,
                FirstName = "Jacob",
                LastName = "Jensen",
                Initials = "JJ",
            };

            var notRelatedOrg2 = new OrgUnit()
            {
                Id = 5,
                Level = 0,
            };

            var notRelatedEmpl2 = new Employment()
            {
                OrgUnit = notRelatedOrg2,
                Person = notRelatedPerson2,
                IsLeader = false,
            };

            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment1",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment3",
                    Employment = notRelatedEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment4",
                    Employment = notRelatedEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment5",
                    Employment = notRelatedEmpl1,
                },

                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2, notRelatedOrg1, notRelatedOrg2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, notRelatedEmpl1, notRelatedEmpl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void ReportsForLeaderButDifferentEmpl_ShouldReturn_Nothing()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());


            var notRelatedOrg1 = new OrgUnit()
            {
                Id = 4,
                Level = 0,
            };

            var notRelatedEmpl1 = new Employment()
            {
                OrgUnit = notRelatedOrg1,
                Person = leader1,
                IsLeader = false,
            };

            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment1",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment2",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment3",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment4",
                    Employment = notRelatedEmpl1,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader1,
                    Comment = "TestComment5",
                    Employment = notRelatedEmpl1,
                },

                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2, notRelatedOrg1
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, notRelatedEmpl1
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void ReportsForLeader_WhoHasSubstitute_ShouldReturnNothing()
        {
            var substitute = new Substitute()
            {
                Person = leader1,
                Sub = leader2,
                Leader = leader1,
                OrgUnit = org1,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {
                substitute
            }.AsQueryable());




            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment1",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment3",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment4",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment5",
                    Employment = leaderEmpl2,
                },

                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void ReportsForLeader_WhoHasSubstituteAndReportsInOtherOrg_AlsoWithSubstitute_ShouldReturnNothing()
        {

            var leader1Org2 = new OrgUnit()
            {
                Id = 66,
                Level = 0
            };

            var substitute1 = new Substitute()
            {
                Person = leader1,
                Sub = leader2,
                Leader = leader1,
                OrgUnit = org1,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            var substitute2 = new Substitute()
            {
                Person = leader1,
                Sub = leader2,
                Leader = leader1,
                OrgUnit = leader1Org2,
                StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds,
                EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            };

            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {
                substitute1, substitute2
            }.AsQueryable());

            

            var leader1Empl2 = new Employment()
            {
                Person = leader1,
                OrgUnit = leader1Org2,
                IsLeader = true
            };




            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment1",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment2",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment3",
                    Employment = leaderEmpl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment4",
                    Employment = leader1Empl2,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment5",
                    Employment = leader1Empl2,
                },

                
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2, leader1Org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, leader1Empl2
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1);
            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void ReportsForNonLeader_InChildOrg_ShouldReturnNothing()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>()
            {

            }.AsQueryable());


            var nonLeaderChildPerson = new Person()
            {
                Id = 77,
                FirstName = "Jacob",
                LastName = "Jensen",
                Initials = "JJ",
            };

            var nonLeaderChildEmpl = new Employment()
            {
                OrgUnit = org2,
                Person = nonLeaderChildPerson,
                IsLeader = false
            };

            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = nonLeaderChildPerson,
                    Comment = "TestComment",
                    Employment = nonLeaderChildEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = nonLeaderChildPerson,
                    Comment = "TestComment2",
                    Employment = nonLeaderChildEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = nonLeaderChildPerson,
                    Comment = "TestComment3",
                    Employment = nonLeaderChildEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = nonLeaderChildPerson,
                    Comment = "TestComment4",
                    Employment = nonLeaderChildEmpl,
                },
                new DriveReport()
                {
                    Id = 1,
                    Person = nonLeaderChildPerson,
                    Comment = "TestComment5",
                    Employment = nonLeaderChildEmpl,
                }
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                leaderEmpl1,leaderEmpl2, nonLeaderChildEmpl
                
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1, true);
            Assert.AreEqual(0, res.Count());
        }  

        [Test]
        public void ReportsFor_NonLeaderChild_LeaderChild_Leader_NoSub_ShouldReturnLeaderChildReports()
        {
            _subRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Substitute>().AsQueryable());

            var childLeader = new Person()
            {
                Id = 2,
                FullName = "Child Leader [CL]",
                FirstName = "Child",
                LastName = "Leader",
                Initials = "CL",
            };

            var childEmployee = new Person()
            {
                Id = 3,
                FullName = "Child Employee [CE]",
                FirstName = "Child",
                LastName = "Employee",
                Initials = "CE"
            };

            var leader = new Person()
            {
                Id = 1,
                FullName = "Jacob Jensen [JJ]",
                FirstName = "Jacob",
                LastName = "Jensen",
                Initials = "JJ"
            };

            var leaderOrg = new OrgUnit()
            {
                Id = 1
            };
            
            var childOrg = new OrgUnit()
            {
                Id = 2,
                ParentId = 1,
                Parent = leaderOrg,
            };



            var childLeaderEmpl = new Employment()
            {
                Person = childLeader,
                PersonId = 2,
                OrgUnit = childOrg,
                OrgUnitId = 2,
                IsLeader = true,
            };

            var childEmployeeEmpl = new Employment()
            {
                Person = childEmployee,
                PersonId = 3,
                OrgUnit = childOrg,
                OrgUnitId = 2,
                IsLeader = false,
            };

            var leaderEmpl = new Employment()
            {
                Person = leader,
                PersonId = 1,
                OrgUnit = leaderOrg,
                OrgUnitId = 1,
                IsLeader = true,
            };

            _emplMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                childLeaderEmpl,
                childEmployeeEmpl,
                leaderEmpl
            }.AsQueryable());

            _orgUnitMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                leaderOrg,
                childOrg,
            }.AsQueryable());



            _reportRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Person = leader,
                    Employment = leaderEmpl,
                    PersonId = 1,
                },
                new DriveReport()
                {
                    Person = childLeader,
                    Employment = childLeaderEmpl,
                    PersonId = 2
                },
                new DriveReport()
                {
                    Person = childEmployee,
                    Employment = childEmployeeEmpl,
                    PersonId = 3
                }
            }.AsQueryable());

            var res = _uut.FilterByLeader(_reportRepoMock.AsQueryable(), 1);
            Assert.AreEqual(1, res.Count());
        }
    }
}
