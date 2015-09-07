using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using NSubstitute;
using NUnit.Framework;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DBUpdater.Test
{
    [TestFixture]
    public class CreateEmploymentTest
    {
        private UpdateService _uut;
        private IGenericRepository<Employment> _emplRepoMock;
        private IGenericRepository<OrgUnit> _orgUnitRepoMock;
        private IGenericRepository<Person> _personRepoMock;
        private IGenericRepository<CachedAddress> _cachedAddressRepoMock;
        private IGenericRepository<PersonalAddress> _personalAddressRepoMock;
        private IAddressLaunderer _actualLaunderer;
        private IAddressCoordinates _coordinates;
        private IGenericRepository<WorkAddress> _workAddressRepoMock;
        private IDbUpdaterDataProvider _dataProvider;
        private IMailSender _mailSenderMock;

        [SetUp]
        public void SetUp()
        {
            var emplList = new List<Employment>();

            var emplIdCount = 0;

            _emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            _personRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _cachedAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            _personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            _actualLaunderer = NSubstitute.Substitute.For<IAddressLaunderer>();
            _coordinates = NSubstitute.Substitute.For<IAddressCoordinates>();
            _dataProvider = NSubstitute.Substitute.For<IDbUpdaterDataProvider>();
            _workAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<WorkAddress>>();
            _mailSenderMock = NSubstitute.Substitute.For<IMailSender>();

            _emplRepoMock.AsQueryable().Returns(emplList.AsQueryable());

            _emplRepoMock.Insert(new Employment()).ReturnsForAnyArgs(x => x.Arg<Employment>()).AndDoes(x => emplList.Add(x.Arg<Employment>())).AndDoes(x => x.Arg<Employment>().Id = emplIdCount).AndDoes(x => emplIdCount++);

            _orgUnitRepoMock.AsQueryable().Returns(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Level = 0,
                    OrgId = 1,
                    LongDescription = "IT Minds, Aarhus",
                    ShortDescription = "ITM",
                    HasAccessToFourKmRule = false,
                    Id = 1,
                }
            }.AsQueryable());

            _personRepoMock.AsQueryable().Returns(new List<Person>()
            {
                new Person()
                {
                    Id = 1,
                }
            }.AsQueryable());

            _uut = new UpdateService(_emplRepoMock, _orgUnitRepoMock, _personRepoMock, _cachedAddressRepoMock,
                _personalAddressRepoMock, _actualLaunderer, _coordinates, _dataProvider, _mailSenderMock);

        }

        [Test]
        public void CreateEmployment_WithEmployeeAndId_ShouldCreateEmployment()
        {
            var empl = new Employee()
            {
                LOSOrgId = 1,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                OphoersDato = new DateTime(2015, 5, 12)
            };

            _uut.CreateEmployment(empl, 1);
            var res = _emplRepoMock.AsQueryable();

            Assert.That(res.ElementAt(0).OrgUnitId.Equals(1));
            Assert.That(res.ElementAt(0).ExtraNumber.Equals(1));
            Assert.That(res.ElementAt(0).IsLeader.Equals(true));
            Assert.That(res.ElementAt(0).Position.Equals("Udvikler"));
            Assert.That(res.ElementAt(0).StartDateTimestamp.Equals(1430179200));
            Assert.That(res.ElementAt(0).EndDateTimestamp.Equals(1431388800));
            Assert.That(res.ElementAt(0).PersonId.Equals(1));
            Assert.That(res.ElementAt(0).EmploymentType.Equals(1));

        }

        [Test]
        public void CreateEmployment_WithEmployeeAndId_NoEndDate_ShouldCreateEmploymentWithEndTime0()
        {
            var empl = new Employee()
            {
                LOSOrgId = 1,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
            };

            _uut.CreateEmployment(empl, 1);
            var res = _emplRepoMock.AsQueryable();

            Assert.That(res.ElementAt(0).OrgUnitId.Equals(1));
            Assert.That(res.ElementAt(0).ExtraNumber.Equals(1));
            Assert.That(res.ElementAt(0).IsLeader.Equals(true));
            Assert.That(res.ElementAt(0).Position.Equals("Udvikler"));
            Assert.That(res.ElementAt(0).StartDateTimestamp.Equals(1430179200));
            Assert.That(res.ElementAt(0).EndDateTimestamp.Equals(0));
            Assert.That(res.ElementAt(0).PersonId.Equals(1));
            Assert.That(res.ElementAt(0).EmploymentType.Equals(1));

        }

        [Test]
        public void CreateEmployment_WithNonExistingOrgUnit_ShouldThrowException()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                OphoersDato = new DateTime(2015, 5, 12)
            };

            Assert.Throws<Exception>(() => _uut.CreateEmployment(empl, 10));
        }



    }
}
