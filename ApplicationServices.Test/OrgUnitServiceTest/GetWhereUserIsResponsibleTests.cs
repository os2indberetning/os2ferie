using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.OrgUnitServiceTest
{
    [TestFixture]
    class GetWhereUserIsResponsibleTests
    {
        private OrgUnitService _uut;

        [Test]
        public void SevenNestedOrgs_OnlyLeaderOnLevel0_ShouldReturn_All()
        {
            var orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitRepoMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0,
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1
                },
                new OrgUnit()
                {
                    Id = 3,
                    Level = 2,
                    ParentId = 2
                },
                new OrgUnit()
                {
                    Id = 4,
                    Level = 3,
                    ParentId = 3
                },
                new OrgUnit()
                {
                    Id = 5,
                    Level = 4,
                    ParentId = 4
                },
                new OrgUnit()
                {
                    Id = 6,
                    Level = 5,
                    ParentId = 5
                },
                new OrgUnit()
                {
                    Id = 7,
                    Level = 6,
                    ParentId = 6
                }

            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 1,
                    PersonId = 1,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2,
                    PersonId = 20,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                    PersonId = 2,

                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 4,
                    PersonId = 3,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 5,
                    PersonId = 4,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 6,
                    PersonId = 5,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 7,
                    PersonId = 6,
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetWhereUserIsResponsible(1);

            Assert.AreEqual(7, res.Count());
        }

        [Test]
        public void SevenNestedOrgs_LeaderOnLevel4_ShouldReturn_4()
        {
            var orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitRepoMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0,
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1
                },
                new OrgUnit()
                {
                    Id = 3,
                    Level = 2,
                    ParentId = 2
                },
                new OrgUnit()
                {
                    Id = 4,
                    Level = 3,
                    ParentId = 3
                },
                new OrgUnit()
                {
                    Id = 5,
                    Level = 4,
                    ParentId = 4
                },
                new OrgUnit()
                {
                    Id = 6,
                    Level = 5,
                    ParentId = 5
                },
                new OrgUnit()
                {
                    Id = 7,
                    Level = 6,
                    ParentId = 6
                }

            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 1,
                    PersonId = 1,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2,
                    PersonId = 20,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                    PersonId = 2,

                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 4,
                    PersonId = 3,
                },
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 5,
                    PersonId = 4,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 6,
                    PersonId = 5,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 7,
                    PersonId = 6,
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetWhereUserIsResponsible(1);

            Assert.AreEqual(4, res.Count());
        }

        [Test]
        public void SevenNestedOrgs_LeaderOnLevel4_CalledWithLeaderOfOrgOnLevel4_ShouldReturn_3()
        {
            var orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            orgUnitRepoMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    Level = 0,
                },
                new OrgUnit()
                {
                    Id = 2,
                    Level = 1,
                    ParentId = 1
                },
                new OrgUnit()
                {
                    Id = 3,
                    Level = 2,
                    ParentId = 2
                },
                new OrgUnit()
                {
                    Id = 4,
                    Level = 3,
                    ParentId = 3
                },
                new OrgUnit()
                {
                    Id = 5,
                    Level = 4,
                    ParentId = 4
                },
                new OrgUnit()
                {
                    Id = 6,
                    Level = 5,
                    ParentId = 5
                },
                new OrgUnit()
                {
                    Id = 7,
                    Level = 6,
                    ParentId = 6
                }

            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 1,
                    PersonId = 1,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2,
                    PersonId = 20,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                    PersonId = 2,

                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 4,
                    PersonId = 3,
                },
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 5,
                    PersonId = 4,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 6,
                    PersonId = 5,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 7,
                    PersonId = 6,
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetWhereUserIsResponsible(4);

            Assert.AreEqual(3, res.Count());
        }

    }
}
