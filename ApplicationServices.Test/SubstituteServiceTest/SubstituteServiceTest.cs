using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ApplicationServices.Test.FileGenerator;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;
using Substitute = Core.DomainModel.Substitute;

namespace ApplicationServices.Test.SubstituteServiceTest
{
    [TestFixture]
    public class SubstituteServiceTest
    {

        private SubstituteService _uut;
        private List<Substitute> _repo;
        private IGenericRepository<Substitute> _repoMock;
        private IOrgUnitService _orgService;
        private IReportService<Report> _reportService;
        private IGenericRepository<Report> _reportRepo;

        [SetUp]
        public void SetUp()
        {
            _orgService = NSubstitute.Substitute.For<IOrgUnitService>();
            _repoMock = NSubstitute.Substitute.For<IGenericRepository<Substitute>>();

            _reportRepo = NSubstitute.Substitute.For<IGenericRepository<Report>>();
            _reportService = NSubstitute.Substitute.For<IReportService<Report>>();


            _repo = new List<Substitute>
            {
                new Substitute()
                {
                    Sub = new Person()
                    {
                        CprNumber = "123123",
                        FirstName = "Jacob",
                        LastName = "Jensen",
                        Initials = "JOJ"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen",
                       Initials = "MR"
                    },
                    Person =
                        new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Morten",
                            LastName = "Rasmussen",
                            Initials = "MR"

                        },
                },
                new Substitute()
                {
                    Sub = new Person()
                    {
                        CprNumber = "123123",
                        FirstName = "Jacob",
                        LastName = "Jensen",
                        Initials = "JOJ"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen",
                       Initials = "MR"
                    },
                    Person = new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Jacob",
                            LastName = "Jensen",
                            Initials = "JOJ"
                        },
                }
            };

            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);

        }

        [Test]
        public void ScrubCPR_ShouldRemoveCPR_FromLeaderAndSubAndPersons()
        {

            // Precondition

            Assert.AreEqual("123123", _repo[0].Leader.CprNumber);
            Assert.AreEqual("123123", _repo[0].Sub.CprNumber);
            Assert.AreEqual("123123", _repo[0].Person.CprNumber);


            Assert.AreEqual("123123", _repo[1].Leader.CprNumber);
            Assert.AreEqual("123123", _repo[1].Sub.CprNumber);
            Assert.AreEqual("123123", _repo[1].Person.CprNumber);

            // Act
            _uut.ScrubCprFromPersons(_repo.AsQueryable());

            // Postcondition
            Assert.AreEqual("", _repo[0].Leader.CprNumber);
            Assert.AreEqual("", _repo[0].Sub.CprNumber);

            Assert.AreEqual("", _repo[0].Person.CprNumber);


            Assert.AreEqual("", _repo[1].Leader.CprNumber);
            Assert.AreEqual("", _repo[1].Sub.CprNumber);

            Assert.AreEqual("", _repo[1].Person.CprNumber);
        }

        [Test]
        public void GetStartOfDayTimestamp_shouldreturn_correctvalue()
        {
            var res = _uut.GetStartOfDayTimestamp(1431341025);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(res).ToLocalTime();
            Assert.AreEqual(11, dateTime.Day);
            Assert.AreEqual(0, dateTime.Hour);
            Assert.AreEqual(0, dateTime.Minute);
            Assert.AreEqual(0, dateTime.Second);

        }

        [Test]
        public void GetEndOfDayTimestamp_shouldreturn_correctvalue()
        {
            var res = _uut.GetEndOfDayTimestamp(1431304249);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(res).ToLocalTime();
            Assert.AreEqual(11, dateTime.Day);
            Assert.AreEqual(23, dateTime.Hour);
            Assert.AreEqual(59, dateTime.Minute);
            Assert.AreEqual(59, dateTime.Second);
        }

        [Test]
        public void CheckIfNewSubIsAllowed_NoExistingSubs_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 12,
                EndDateTimestamp = 2000,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12
            };

            _repo = new List<Substitute>();
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);

            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSubBeforeNewSub_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1432166400,
                EndDateTimestamp = 1432252800,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 12
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);

            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSubAfterNewSub_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432166400,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 12
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSub_SamePeriod_DifferentOrg_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 13
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSub_SamePeriod_SameOrg_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12,
                Id = 1,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 12,
                    Id = 2
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSub_OverlappingPeriod_SameOrg_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432166400,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12,
                Id = 1
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432080000,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 12,
                    Id = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingSub_OverlappingPeriod2_SameOrg_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1432166400,
                EndDateTimestamp = 1432339200,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12,
                Id = 1,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432080000,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                    OrgUnitId = 12,
                    Id = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_NoExistingApprovers_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 12,
                EndDateTimestamp = 2000,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
                OrgUnitId = 12
            };

            _repo = new List<Substitute>();
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApproverBeforeNewApprover_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1432166400,
                EndDateTimestamp = 1432252800,
                PersonId = 1,
                LeaderId = 3,
                SubId = 2,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 1,
                    LeaderId = 3,
                    SubId = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApproverAfterNewApprover_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 1,
                LeaderId = 3,
                SubId = 2,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432166400,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 3,
                    SubId = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApprover_SamePeriod_DifferentPerson_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 5,
                LeaderId = 3,
                SubId = 2,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 1,
                    LeaderId = 1,
                    SubId = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApprover_SamePeriod_SamePerson_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432080000,
                PersonId = 3,
                LeaderId = 1,
                SubId = 2,
                Id = 1,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1431993600,
                    EndDateTimestamp = 1432080000,
                    PersonId = 3,
                    LeaderId = 1,
                    SubId = 2,
                    Id = 2,
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApprover_OverlappingPeriod_SamePerson_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1431993600,
                EndDateTimestamp = 1432166400,
                PersonId = 1,
                LeaderId = 3,
                SubId = 2,
                Id = 1,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432080000,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 3,
                    SubId = 2,
                    Id = 2
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApprover_OverlappingPeriod2_SamePerson_ShouldReturnFalse()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1432166400,
                EndDateTimestamp = 1432339200,
                PersonId = 1,
                LeaderId = 3,
                SubId = 2,
                Id = 1
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432080000,
                    EndDateTimestamp = 1432252800,
                    PersonId = 1,
                    LeaderId = 3,
                    SubId = 2,
                    Id = 2
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsFalse(_uut.CheckIfNewSubIsAllowed(substitute));
        }

        [Test]
        public void CheckIfNewSubIsAllowed_ExistingApprover_SamePeriod_AddSub_ShouldReturnTrue()
        {
            var substitute = new Substitute()
            {
                StartDateTimestamp = 1432166400,
                EndDateTimestamp = 1432339200,
                PersonId = 1,
                LeaderId = 1,
                SubId = 2,
            };

            _repo = new List<Substitute>()
            {
                new Substitute()
                {
                    StartDateTimestamp = 1432166400,
                    EndDateTimestamp = 1432339200,
                    PersonId = 1,
                    LeaderId = 3,
                    SubId = 2,
                    OrgUnitId = 12
                },
            };
            _repoMock.AsQueryable().ReturnsForAnyArgs(_repo.AsQueryable());
            _uut = new SubstituteService(_repoMock, _orgService, _reportRepo, _reportService);
            Assert.IsTrue(_uut.CheckIfNewSubIsAllowed(substitute));
        }

    }
}
