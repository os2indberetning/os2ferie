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
    public class SaveAddressTest
    {
        private UpdateService _uut;
        private IGenericRepository<Employment> _emplRepoMock;
        private IGenericRepository<OrgUnit> _orgUnitRepoMock;
        private IGenericRepository<Person> _personRepoMock;
        private IGenericRepository<CachedAddress> _cachedAddressRepoMock;
        private IGenericRepository<PersonalAddress> _personalAddressRepoMock;
        private IAddressLaunderer _actualLaundererMock;
        private IAddressCoordinates _coordinatesMock;
        private IDbUpdaterDataProvider _dataProviderMock;
        private IGenericRepository<WorkAddress> _workAddressRepoMock;
        private IMailSender _mailSenderMock;

        [SetUp]
        public void SetUp()
        {
            var personList = new List<Person>();
            var emplList = new List<Employment>();

            var emplIdCount = 0;
            var personIdCount = 0;

            var cachedAddressList = new List<CachedAddress>();
            var cachedIdCount = 0;
            var personalAddressList = new List<PersonalAddress>();
            var personalIdCount = 0;

            _emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            _personRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _cachedAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            _personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            _actualLaundererMock = NSubstitute.Substitute.For<IAddressLaunderer>();
            _coordinatesMock = NSubstitute.Substitute.For<IAddressCoordinates>();
            _dataProviderMock = NSubstitute.Substitute.For<IDbUpdaterDataProvider>();
            _workAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<WorkAddress>>();
            _mailSenderMock = NSubstitute.Substitute.For<IMailSender>();

            _personRepoMock.AsQueryable().Returns(personList.AsQueryable());

            _personRepoMock.Insert(new Person()).ReturnsForAnyArgs(x => x.Arg<Person>()).AndDoes(x => personList.Add(x.Arg<Person>())).AndDoes(x => x.Arg<Person>().Id = personIdCount).AndDoes(x => personIdCount++);

            _emplRepoMock.AsQueryable().Returns(emplList.AsQueryable());

            _emplRepoMock.Insert(new Employment()).ReturnsForAnyArgs(x => x.Arg<Employment>()).AndDoes(x => emplList.Add(x.Arg<Employment>())).AndDoes(x => x.Arg<Employment>().Id = emplIdCount).AndDoes(x => emplIdCount++);

            _cachedAddressRepoMock.Insert(new CachedAddress()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>()).AndDoes(x => cachedAddressList.Add(x.Arg<CachedAddress>())).AndDoes(x => x.Arg<CachedAddress>().Id = cachedIdCount).AndDoes(x => cachedIdCount++);

            cachedAddressList.Add(new CachedAddress()
            {
                Id = 999,
                StreetName = "Katrinebjergvej",
                StreetNumber = "93B",
                ZipCode = 8200,
                Town = "Aarhus N",
                DirtyString = "Katrinebjergvej 93B, 8200 Aarhus N",
            });

            _cachedAddressRepoMock.AsQueryable().Returns(cachedAddressList.AsQueryable());

            _personalAddressRepoMock.Insert(new PersonalAddress()).ReturnsForAnyArgs(x => x.Arg<PersonalAddress>()).AndDoes(x => personalAddressList.Add(x.Arg<PersonalAddress>())).AndDoes(x => x.Arg<PersonalAddress>().Id = personalIdCount).AndDoes(x => personalIdCount++);

            _personalAddressRepoMock.AsQueryable().Returns(personalAddressList.AsQueryable());

            _actualLaundererMock.Launder(new Address()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>());

            _uut = new UpdateService(_emplRepoMock, _orgUnitRepoMock, _personRepoMock, _cachedAddressRepoMock,
                _personalAddressRepoMock, _actualLaundererMock, _coordinatesMock, _dataProviderMock, _mailSenderMock, NSubstitute.Substitute.For<IAddressHistoryService>());

            _orgUnitRepoMock.AsQueryable().ReturnsForAnyArgs(new List<OrgUnit>()
            {
                new OrgUnit()
                {
                    Id = 1,
                    OrgId = 1,
                    ShortDescription = "ITM",
                    LongDescription = "IT Minds, Aarhus",
                    Level = 0,
                    HasAccessToFourKmRule = false,
                },
                new OrgUnit()
                {
                    Id = 2,
                    OrgId = 2,
                    ShortDescription = "ITMB",
                    LongDescription = "IT Minds, Aarhus child",
                    Level = 1,
                    ParentId = 1,
                    HasAccessToFourKmRule = false,
                }
            }.AsQueryable());

            personList.Add(new Person()
            {
                Id = 1,
            });
        }

        [Test]
        public void SaveHomeAddress_WithNonExistingPerson_ShouldThrowException()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            Assert.Throws<Exception>(() => _uut.UpdateHomeAddress(empl, 10));
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCallActualLaunderer()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.UpdateHomeAddress(empl,1);

            _actualLaundererMock.ReceivedWithAnyArgs().Launder(new Address());
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCallCoordinates()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.UpdateHomeAddress(empl, 1);

            _coordinatesMock.ReceivedWithAnyArgs().GetAddressCoordinates(new Address());
        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldCacheAddress()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.UpdateHomeAddress(empl, 1);

            var res = _cachedAddressRepoMock.AsQueryable().First(a => a.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetNumber.Equals("44"));
            Assert.That(res.ZipCode.Equals(8210));
            Assert.That(res.Town.Equals("Aarhus V"));
            Assert.That(res.IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithCachedAddress_ShouldNotCallActualLaunderer()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };
            
            _cachedAddressRepoMock.Insert(new CachedAddress()
            {
                IsDirty = false,
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V",
                DirtyString = "Jens Baggesens Vej 44, 8210 Aarhus V"
            });

            _uut.UpdateHomeAddress(empl, 1);

            _actualLaundererMock.DidNotReceiveWithAnyArgs().Launder(new Address());

            var res = _cachedAddressRepoMock.AsQueryable().First(a => a.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetNumber.Equals("44"));
            Assert.That(res.ZipCode.Equals(8210));
            Assert.That(res.Town.Equals("Aarhus V"));
            Assert.That(res.IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithCachedAddress_WithoutCoordinates_ShouldCallCoordinates()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _cachedAddressRepoMock.Insert(new CachedAddress()
            {
                IsDirty = true,
                StreetName = "Jens Baggesens Vej",
                StreetNumber = "44",
                ZipCode = 8210,
                Town = "Aarhus V",
                DirtyString = "Jens Baggesens Vej 44, 8210 Aarhus V"
            });

            _uut.UpdateHomeAddress(empl, 1);

            var res = _cachedAddressRepoMock.AsQueryable().First(a => a.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.StreetNumber.Equals("44"));
            Assert.That(res.ZipCode.Equals(8210));
            Assert.That(res.Town.Equals("Aarhus V"));
            Assert.That(res.IsDirty.Equals(false));

        }

        [Test]
        public void SaveHomeAddress_WithNonCachedAddress_ShouldInsertAddressInto_PersonalAddresses()
        {
            var empl = new Employee()
            {
                LOSOrgId = 10,
                AnsatForhold = "1",
                EkstraCiffer = 1,
                Leder = true,
                Stillingsbetegnelse = "Udvikler",
                AnsaettelsesDato = new DateTime(2015, 4, 28),
                Adresse = "Jens Baggesens Vej 44",
                PostNr = 8210,
                By = "Aarhus V"
            };

            _uut.UpdateHomeAddress(empl, 1);

            var res = _personalAddressRepoMock.AsQueryable();
            Assert.That(res.ElementAt(0).StreetName.Equals("Jens Baggesens Vej"));
            Assert.That(res.ElementAt(0).StreetNumber.Equals("44"));
            Assert.That(res.ElementAt(0).ZipCode.Equals(8210));
            Assert.That(res.ElementAt(0).Town.Equals("Aarhus V"));
            Assert.That(res.ElementAt(0).Type.Equals(PersonalAddressType.Home));
            Assert.That(res.ElementAt(0).PersonId.Equals(1));

        }
    }
}
