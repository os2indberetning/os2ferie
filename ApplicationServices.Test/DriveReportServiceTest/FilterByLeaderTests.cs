using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.DomainModel;
using Core.DomainServices;
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
        [Test]
        public void t()
        {
            var driveRepoMock = NSubstitute.Substitute.For<IGenericRepository<DriveReport>>();
            var orgRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            var emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();

            var leader1 = new Person()
            {
                Id = 1,
                FirstName = "Jacob",
                LastName = "Jensen",
                Initials = "JJ",
            };

            var leader2 = new Person()
            {
                Id = 2,
                FirstName = "Morten",
                LastName = "Rasmussen",
                Initials = "MR",
            };

            var org1 = new OrgUnit()
            {
                Id = 1,
                Level = 0,
            };

            var org2 = new OrgUnit()
            {
                Id = 2,
                Level = 1,
                ParentId = 1
            };

            driveRepoMock.AsQueryable().ReturnsForAnyArgs(new List<DriveReport>()
            {
                new DriveReport()
                {
                    Id = 1,
                    Person = leader2,
                    Comment = "TestComment"
                }
            }.AsQueryable());

            orgRepoMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                org1,org2
                
            }.AsQueryable());

            emplRepoMock.AsQueryable().ReturnsForAnyArgs(new List<Employment>()
            {
                new Employment()
                {
                    Person = leader1,
                    OrgUnit = org1,
                    IsLeader = true

                },
                new Employment()
                {
                    Person = leader2,
                    OrgUnit = org2,
                    IsLeader = true

                }
            }.AsQueryable());

            var uut = new DriveReportService(new MailSender(), driveRepoMock,
                NSubstitute.Substitute.For<IReimbursementCalculator>(), orgRepoMock, emplRepoMock,
               NSubstitute.Substitute.For<IGenericRepository<Substitute>>());

            var res = uut.FilterByLeader(driveRepoMock.AsQueryable(), 1);
            Assert.AreEqual(1, res.Count());
        }
    }
}
