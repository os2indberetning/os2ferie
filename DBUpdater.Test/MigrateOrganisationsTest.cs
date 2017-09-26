using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.DataAccess;
using NSubstitute;
using NUnit.Framework;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;
using Core.ApplicationServices.Interfaces;

namespace DBUpdater.Test
{
    [TestFixture]
    public class MigrateOrganisationsTest
    {
        private UpdateService _uut;
        private IGenericRepository<Employment> _emplRepoMock;
        private IGenericRepository<OrgUnit> _orgUnitRepoMock;
        private IGenericRepository<Person> _personRepoMock;
        private IGenericRepository<CachedAddress> _cachedAddressRepoMock;
        private IGenericRepository<PersonalAddress> _personalAddressRepoMock;
        private IReportService<Report> _repotService;
        private IGenericRepository<Report> _reportRepo;
        private ISubstituteService _subservice;
        private IGenericRepository<Core.DomainModel.Substitute> _subRepo;
        private IAddressLaunderer _actualLaunderer;
        private IAddressCoordinates _coordinates;
        private IDbUpdaterDataProvider _dataProvider;
        private IMailSender _mailSender;
        private IGenericRepository<Core.DomainModel.VacationBalance> _vacationBalanceRepo;

        [SetUp]
        public void SetUp()
        {
            var orgList = new List<OrgUnit>();

            var orgIdCount = 0;

            var cachedAddressList = new List<CachedAddress>();
            var cachedIdCount = 0;

            _emplRepoMock = NSubstitute.Substitute.For<IGenericRepository<Employment>>();
            _orgUnitRepoMock = NSubstitute.Substitute.For<IGenericRepository<OrgUnit>>();
            _personRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _cachedAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<CachedAddress>>();
            _personalAddressRepoMock = NSubstitute.Substitute.For<IGenericRepository<PersonalAddress>>();
            _actualLaunderer = NSubstitute.Substitute.For<IAddressLaunderer>();
            _coordinates = NSubstitute.Substitute.For<IAddressCoordinates>();
            _dataProvider = NSubstitute.Substitute.For<IDbUpdaterDataProvider>();
            _mailSender = NSubstitute.Substitute.For<IMailSender>();
            _vacationBalanceRepo = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.VacationBalance>>();

            _orgUnitRepoMock.AsQueryable().Returns(orgList.AsQueryable());

            _orgUnitRepoMock.Insert(new OrgUnit()).ReturnsForAnyArgs(x => x.Arg<OrgUnit>()).AndDoes(x => orgList.Add(x.Arg<OrgUnit>())).AndDoes(x => x.Arg<OrgUnit>().Id = orgIdCount).AndDoes(x => orgIdCount++);

            _cachedAddressRepoMock.Insert(new CachedAddress()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>()).AndDoes(x => cachedAddressList.Add(x.Arg<CachedAddress>())).AndDoes(x => x.Arg<CachedAddress>().Id = cachedIdCount).AndDoes(x => cachedIdCount++);

            _cachedAddressRepoMock.AsQueryable().Returns(cachedAddressList.AsQueryable());

            _subRepo = NSubstitute.Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();
            _reportRepo = NSubstitute.Substitute.For<IGenericRepository<Report>>();
            _repotService = NSubstitute.Substitute.For<IReportService<Report>>();
            _subservice = NSubstitute.Substitute.For<ISubstituteService>();

            _actualLaunderer.Launder(new Address()).ReturnsForAnyArgs(x => x.Arg<CachedAddress>());

            _uut = new UpdateService(_emplRepoMock, _orgUnitRepoMock, _personRepoMock, _cachedAddressRepoMock,
                _personalAddressRepoMock, _actualLaunderer, _coordinates, _dataProvider, _mailSender, NSubstitute.Substitute.For<IAddressHistoryService>(), _reportRepo, _repotService, _subservice, _subRepo, _vacationBalanceRepo);

        }

        [Test]
        public void TwoOrganisations_NoChildren_ShouldAddOrgUnits()
        {
            _dataProvider.GetOrganisationsAsQueryable().Returns(new List<Organisation>()
            {
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 1,
                    Gade = "Katrinebjergvej 93b",
                    Stednavn = "",
                    Postnr = 8200,
                    KortNavn = "ITM",
                    Navn = "IT Minds, Aarhus",
                    Level = 0,
                },
                new Organisation()
                {
                    By = "Erslev",
                    LOSOrgId = 2,
                    Gade = "Præstbrovej 163",
                    Stednavn = "",
                    Postnr = 7950,
                    KortNavn = "ES",
                    Navn = "Erslev Slagter",
                    Level = 0,
                }
            }.AsQueryable());


            _uut.MigrateOrganisations();
            var res = _orgUnitRepoMock.AsQueryable();

            _orgUnitRepoMock.ReceivedWithAnyArgs().Insert(new OrgUnit());
            Assert.That(res.ElementAt(0).LongDescription.Equals("IT Minds, Aarhus"));
            Assert.That(res.ElementAt(0).ShortDescription.Equals("ITM"));
            Assert.That(res.ElementAt(0).OrgId.Equals(1));
            Assert.That(res.ElementAt(0).Level.Equals(0));
            Assert.That(res.ElementAt(0).ParentId.Equals(null));
            Assert.That(res.ElementAt(0).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(1).LongDescription.Equals("Erslev Slagter"));
            Assert.That(res.ElementAt(1).ShortDescription.Equals("ES"));
            Assert.That(res.ElementAt(1).OrgId.Equals(2));
            Assert.That(res.ElementAt(1).Level.Equals(0));
            Assert.That(res.ElementAt(1).ParentId.Equals(null));
            Assert.That(res.ElementAt(1).HasAccessToFourKmRule.Equals(false));

        }

        [Test]
        public void TwoOrganisations_ParentAndChild_ShouldAddOrgUnits()
        {
            _dataProvider.GetOrganisationsAsQueryable().Returns(new List<Organisation>()
            {
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 1,
                    Gade = "Katrinebjergvej 93b",
                    Stednavn = "",
                    Postnr = 8200,
                    KortNavn = "ITM",
                    Navn = "IT Minds, Aarhus",
                    Level = 0,
                },
                new Organisation()
                {
                    By = "Erslev",
                    LOSOrgId = 2,
                    Gade = "Præstbrovej 163",
                    Stednavn = "",
                    Postnr = 7950,
                    KortNavn = "ES",
                    Navn = "Erslev Slagter",
                    Level = 1,
                    ParentLosOrgId = 1
                }
            }.AsQueryable());


            _uut.MigrateOrganisations();
            var res = _orgUnitRepoMock.AsQueryable();


            _orgUnitRepoMock.ReceivedWithAnyArgs().Insert(new OrgUnit());
            Assert.That(res.ElementAt(0).LongDescription.Equals("IT Minds, Aarhus"));
            Assert.That(res.ElementAt(0).ShortDescription.Equals("ITM"));
            Assert.That(res.ElementAt(0).OrgId.Equals(1));
            Assert.That(res.ElementAt(0).Level.Equals(0));
            Assert.That(res.ElementAt(0).ParentId.Equals(null));
            Assert.That(res.ElementAt(0).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(1).LongDescription.Equals("Erslev Slagter"));
            Assert.That(res.ElementAt(1).ShortDescription.Equals("ES"));
            Assert.That(res.ElementAt(1).OrgId.Equals(2));
            Assert.That(res.ElementAt(1).Level.Equals(1));
            Assert.That(res.ElementAt(1).ParentId.Equals(0));
            Assert.That(res.ElementAt(1).HasAccessToFourKmRule.Equals(false));

        }

        [Test]
        public void ThreeOrganisations_ParentTwoChildren_ShouldAddOrgUnits()
        {
            _dataProvider.GetOrganisationsAsQueryable().Returns(new List<Organisation>()
            {
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 1,
                    Gade = "Katrinebjergvej 93b",
                    Stednavn = "",
                    Postnr = 8200,
                    KortNavn = "ITM",
                    Navn = "IT Minds, Aarhus",
                    Level = 0,
                },
                new Organisation()
                {
                    By = "Erslev",
                    LOSOrgId = 2,
                    Gade = "Præstbrovej 163",
                    Stednavn = "",
                    Postnr = 7950,
                    KortNavn = "ES",
                    Navn = "Erslev Slagter",
                    Level = 1,
                    ParentLosOrgId = 1
                },
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 3,
                    Gade = "Jens Baggesens Vej 42",
                    Stednavn = "",
                    Postnr = 8210,
                    KortNavn = "GKF",
                    Navn = "Grankoglefabrik",
                    Level = 1,
                    ParentLosOrgId = 1
                }
            }.AsQueryable());


            _uut.MigrateOrganisations();
            var res = _orgUnitRepoMock.AsQueryable();


            _orgUnitRepoMock.ReceivedWithAnyArgs().Insert(new OrgUnit());
            Assert.That(res.ElementAt(0).LongDescription.Equals("IT Minds, Aarhus"));
            Assert.That(res.ElementAt(0).ShortDescription.Equals("ITM"));
            Assert.That(res.ElementAt(0).OrgId.Equals(1));
            Assert.That(res.ElementAt(0).Level.Equals(0));
            Assert.That(res.ElementAt(0).ParentId.Equals(null));
            Assert.That(res.ElementAt(0).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(1).LongDescription.Equals("Erslev Slagter"));
            Assert.That(res.ElementAt(1).ShortDescription.Equals("ES"));
            Assert.That(res.ElementAt(1).OrgId.Equals(2));
            Assert.That(res.ElementAt(1).Level.Equals(1));
            Assert.That(res.ElementAt(1).ParentId.Equals(0));
            Assert.That(res.ElementAt(1).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(2).LongDescription.Equals("Grankoglefabrik"));
            Assert.That(res.ElementAt(2).ShortDescription.Equals("GKF"));
            Assert.That(res.ElementAt(2).OrgId.Equals(3));
            Assert.That(res.ElementAt(2).Level.Equals(1));
            Assert.That(res.ElementAt(2).ParentId.Equals(0));
            Assert.That(res.ElementAt(2).HasAccessToFourKmRule.Equals(false));

        }

        [Test]
        public void ThreeOrganisations_ParentChildAndChildOfChild_ShouldAddOrgUnits()
        {
            _dataProvider.GetOrganisationsAsQueryable().Returns(new List<Organisation>()
            {
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 1,
                    Gade = "Katrinebjergvej 93b",
                    Stednavn = "",
                    Postnr = 8200,
                    KortNavn = "ITM",
                    Navn = "IT Minds, Aarhus",
                    Level = 0,
                },
                new Organisation()
                {
                    By = "Erslev",
                    LOSOrgId = 2,
                    Gade = "Præstbrovej 163",
                    Stednavn = "",
                    Postnr = 7950,
                    KortNavn = "ES",
                    Navn = "Erslev Slagter",
                    Level = 1,
                    ParentLosOrgId = 1
                },
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 3,
                    Gade = "Jens Baggesens Vej 42",
                    Stednavn = "",
                    Postnr = 8210,
                    KortNavn = "GKF",
                    Navn = "Grankoglefabrik",
                    Level = 2,
                    ParentLosOrgId = 2
                }
            }.AsQueryable());


            _uut.MigrateOrganisations();
            var res = _orgUnitRepoMock.AsQueryable();


            _orgUnitRepoMock.ReceivedWithAnyArgs().Insert(new OrgUnit());
            Assert.That(res.ElementAt(0).LongDescription.Equals("IT Minds, Aarhus"));
            Assert.That(res.ElementAt(0).ShortDescription.Equals("ITM"));
            Assert.That(res.ElementAt(0).OrgId.Equals(1));
            Assert.That(res.ElementAt(0).Level.Equals(0));
            Assert.That(res.ElementAt(0).ParentId.Equals(null));
            Assert.That(res.ElementAt(0).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(1).LongDescription.Equals("Erslev Slagter"));
            Assert.That(res.ElementAt(1).ShortDescription.Equals("ES"));
            Assert.That(res.ElementAt(1).OrgId.Equals(2));
            Assert.That(res.ElementAt(1).Level.Equals(1));
            Assert.That(res.ElementAt(1).ParentId.Equals(0));
            Assert.That(res.ElementAt(1).HasAccessToFourKmRule.Equals(false));

            Assert.That(res.ElementAt(2).LongDescription.Equals("Grankoglefabrik"));
            Assert.That(res.ElementAt(2).ShortDescription.Equals("GKF"));
            Assert.That(res.ElementAt(2).OrgId.Equals(3));
            Assert.That(res.ElementAt(2).Level.Equals(2));
            Assert.That(res.ElementAt(2).ParentId.Equals(1));
            Assert.That(res.ElementAt(2).HasAccessToFourKmRule.Equals(false));

        }

        [Test]
        public void InsertingOrganisation_ThatExists_ShouldNotCall_Insert()
        {
            _orgUnitRepoMock.Insert(new OrgUnit()
            {
                OrgId = 1
            });

            _orgUnitRepoMock.ClearReceivedCalls();

            _dataProvider.GetOrganisationsAsQueryable().Returns(new List<Organisation>()
            {
                new Organisation()
                {
                    By = "Aarhus",
                    LOSOrgId = 1,
                    Gade = "Katrinebjergvej 93b",
                    Stednavn = "",
                    Postnr = 8200,
                    KortNavn = "ITM",
                    Navn = "IT Minds, Aarhus",
                    Level = 0,
                }
            }.AsQueryable());


            _uut.MigrateOrganisations();
            var res = _orgUnitRepoMock.AsQueryable();


            _orgUnitRepoMock.DidNotReceiveWithAnyArgs().Insert(new OrgUnit());
            Assert.That(res.ElementAt(0).LongDescription.Equals("IT Minds, Aarhus"));
            Assert.That(res.ElementAt(0).ShortDescription.Equals("ITM"));
            Assert.That(res.ElementAt(0).OrgId.Equals(1));
            Assert.That(res.ElementAt(0).Level.Equals(0));
            Assert.That(res.ElementAt(0).ParentId.Equals(null));
            Assert.That(res.ElementAt(0).HasAccessToFourKmRule.Equals(false));
        }
    }
}
