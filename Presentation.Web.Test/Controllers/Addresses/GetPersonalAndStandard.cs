using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using OS2Indberetning;
using Owin;
using Presentation.Web.Test.Controllers.Models;

namespace Presentation.Web.Test.
    Controllers.Addresses
{
    [TestFixture]
    class GetPersonalAndStandard
    {
        private AddressRepositoryMock repo;
        protected TestServer Server;


        [SetUp]
        public void SetUp()
        {
            Server = TestServer.Create(app =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                config.DependencyResolver = new NinjectDependencyResolver(NinjectTestInjector.CreateKernel(GetInjections()));
                app.UseWebApi(config);
            });
            ReSeed();

            EmploymentRepositoryMock.employment = new Employment();
        }

        public void ReSeed()
        {
            new AddressRepositoryMock().ReSeed();
        }

        public List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<Address>),typeof(AddressRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<Person>),typeof(PersonRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<Employment>),typeof(EmploymentRepositoryMock)),

            };
        }

     

    [Test]
        public async void OnlyPersonalAddresses_With2IncorrectPerson_ShouldReturn_One()
        {
            AddressRepositoryMock.addresses = new List<Address>()
            {
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "1",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 1
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "2",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 2
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 2
                }
            };
            ReSeed();
            var response = await Server.CreateRequest("/odata/Addresses/Service.GetPersonalAndStandard?personId=1").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<Address>>();
            Assert.AreEqual(1, result.value.Count);
        }

        [Test]
        public async void MixedStandardAndPersonal_WithIncorrectId_ShouldReturn_StandardAndCorrect()
        {
            AddressRepositoryMock.addresses = new List<Address>
            {
                new Address()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "1",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Address()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "2",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 1
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "4",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 2
                }
            };
            ReSeed();
            var response = await Server.CreateRequest("/odata/Addresses/Service.GetPersonalAndStandard?personId=1").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<Address>>();
            Assert.AreEqual(3, result.value.Count);
        }


        [Test]
        public async void MixedPointAndDriveReportPoint_ShouldReturn_None()
        {
            AddressRepositoryMock.addresses = new List<Address>()
            {
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "1",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "2",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new DriveReportPoint()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new DriveReportPoint()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "4",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                }
            };
            ReSeed();
            var response = await Server.CreateRequest("/odata/Addresses/Service.GetPersonalAndStandard?personId=1").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<Address>>();
            Assert.AreEqual(1, result.value.Count);
        }

        [Test]
        public async void MixedPointAndDriveReportPointAndStandardAndPersonal_ShouldReturn_StandardAndPersonal()
        {


            AddressRepositoryMock.addresses = new List<Address>()
            {
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "1",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "2",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new DriveReportPoint()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Address()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "4",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "5",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 1
                }
            };
            ReSeed();
            var response = await Server.CreateRequest("/odata/Addresses/Service.GetPersonalAndStandard?personId=1").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<Address>>();
            Assert.AreEqual(2, result.value.Count);
        }

        [Test]
        public async void MixedPointAndDriveReportPointAndStandardAndTwoPersonal_ShouldReturn_StandardAndPersonal()
        {
            AddressRepositoryMock.addresses = new List<Address>()
            {
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "1",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Point()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "2",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new DriveReportPoint()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new Address()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 1
                },
                new PersonalAddress()
                {
                    StreetName = "TestStreet",
                    StreetNumber = "3",
                    ZipCode = 8210,
                    Town = "Aarhus V",
                    PersonId = 2
                }
            };
            ReSeed();
            var response = await Server.CreateRequest("/odata/Addresses/Service.GetPersonalAndStandard?personId=1").GetAsync();
            var result = await response.Content.ReadAsAsync<ODataResponse<Address>>();
            Assert.AreEqual(2, result.value.Count);
        }

    }
}


