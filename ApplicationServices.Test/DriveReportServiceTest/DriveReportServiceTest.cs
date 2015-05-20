using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
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
    [TestFixture]
    public class DriveReportServiceTest : DriveReportMock
    {
        [Test]
        public void Create_WithPersonID0_ShouldThrowException()
        {
            var testReport = new DriveReport()
            {
                PersonId = 0
            };

            Assert.Throws<Exception>(() => NinjectWebKernel.CreateKernel().Get<DriveReportService>().Create(testReport));
        }
    }

    [TestFixture] //TODO rewrite tests, did not catch that the person was always set as responsible leader
    /** Things to test: 
     *      person is an employee
     *      person is a leader (approver is leader of next level
     *      Person is leader on two levels
     *      Person has personal approver
     *      persons leader has substitute
     */

    public class AttachResponsibleLeaderTests
    {
        private IGenericRepository<Employment> _emplMock;
        private IGenericRepository<OrgUnit> _orgUnitMock;
        private IGenericRepository<Core.DomainModel.Substitute> _subMock;
        private IDriveReportService _uut;

        [SetUp]
        public void SetUp()
        {
            _emplMock = Substitute.For<IGenericRepository<Employment>>();
            _orgUnitMock = Substitute.For<IGenericRepository<OrgUnit>>();
            _subMock = Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            _uut = new DriveReportService(Substitute.For<IMailSender>(), Substitute.For<IGenericRepository<DriveReport>>(), Substitute.For<IReimbursementCalculator>(), _orgUnitMock, _emplMock, _subMock);
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
                    
                },
                Purpose = "Test"
            };

            var res = _uut.Validate(report);
            Assert.IsTrue(res);
        }
    }
}
