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
    class GetChildOrgsWithoutLeaderTests
    {
        private OrgUnitService _uut;

        [Test]
        public void AllChildOrgs_HaveLeader_ShouldReturn_Nothing()
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
                    Level = 1,
                    ParentId = 1
                }
            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 3,
                },
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 2
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(0,res.Count());
        }

        [Test]
        public void AllChildOrgs_HaveNoLeader_ShouldReturn_ChildOrgs()
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
                    Level = 1,
                    ParentId = 1
                }
            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(2, res.Count());
        }

        [Test]
        public void OneChildOrgWithLeaderOneWithout_ShouldReturn_OneChildOrg()
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
                    Level = 1,
                    ParentId = 1
                }
            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 3,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(1, res.Count());
        }

        [Test]
        public void ChildAndGrandChildOrg_NoLeader_ShouldReturn_Both()
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
                }
            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 2
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(2, res.Count());
        }

        [Test]
        public void ChildWithLeaderAndGrandChild_NoLeader_ShouldReturn_Nothing()
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
                }
            }.AsQueryable());

            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3,
                },
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 2
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public void SevenNestedOrgs_NoLeader_ShouldReturn_All()
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
                    IsLeader = false,
                    OrgUnitId = 2,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 4
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 5
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 6
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 7
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(6, res.Count());
        }

        [Test]
        public void SevenNestedOrgs_LeaderOnLevel4_ShouldReturn_3()
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
                    IsLeader = false,
                    OrgUnitId = 2,
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 3
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 4
                },
                new Employment()
                {
                    IsLeader = true,
                    OrgUnitId = 5
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 6
                },
                new Employment()
                {
                    IsLeader = false,
                    OrgUnitId = 7
                }
            }.AsQueryable());

            _uut = new OrgUnitService(emplRepoMock, orgUnitRepoMock);

            var res = _uut.GetChildOrgsWithoutLeader(1);

            Assert.AreEqual(3, res.Count());
        }
    }
}
