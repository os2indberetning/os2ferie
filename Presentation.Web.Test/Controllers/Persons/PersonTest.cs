using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Ninject;
using NUnit.Framework;
using Presentation.Web.Test.Controllers.DriveReports;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using NSubstitute;

namespace Presentation.Web.Test.Controllers.Persons
{
    //Notice that the controller scrubs away the cpr number
    //before serving the person entity.

    class PersonServiceMock : PersonService
    {
        public PersonServiceMock(IGenericRepository<PersonalAddress> addressRepo, IRoute<RouteInformation> route) : base(addressRepo, route)
        {
        }

        public override PersonalAddress GetHomeAddress(Person person)
        {
            return  new PersonalAddress()
                {
                    Description = "TestHomeAddress",
                    Id = 1,
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "46",
                    ZipCode = 8210,
                    Town = "Aarhus"
                };
        }

        public override PersonalAddress GetWorkAddress(Person person)
        {
            return  new PersonalAddress()
                {
                    Description = "TestWorkAddress",
                    Id = 2,
                    Type = PersonalAddressType.Work,
                    PersonId = 1,
                    StreetName = "Katrinebjergvej",
                    StreetNumber = "95",
                    ZipCode = 8200,
                    Town = "Aarhus"
                };
        }

        public double GetDistanceFromHomeToWork(Person p)
        {
            return 0;
        }
    }

    [TestFixture]
    public class PersonTest : BaseControllerTest<Person>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
              

            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Person>), typeof (PersonRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof (IMailSender), typeof (MailSenderMock)),
                    new KeyValuePair<Type, Type>(typeof(IPersonService), typeof(PersonServiceMock))
            };
        }

        protected override Person GetReferenceEntity1()
        {
            return new Person
            {
                Id = 1,
                CprNumber = "",
                FirstName = "Morten",
                LastName = "Rasmussen"
            };
        }

        protected override Person GetReferenceEntity2()
        {
            return new Person
            {
                Id = 2,
                CprNumber = "",
                FirstName = "Morten",
                LastName = "Jørgensen"
            };
        }

        protected override Person GetReferenceEntity3()
        {
            return new Person
            {
                Id = 3,
                CprNumber = "",
                FirstName = "Morten",
                LastName = "Foo"
            };
        }

        protected override Person GetPostReferenceEntity()
        {
            return new Person
            {
                Id = 4,
                CprNumber = "",
                FirstName = "Morten",
                LastName = "Bar"
            };
        }

        protected override Person GetPatchReferenceEntity()
        {
            return new Person
            {
                Id = 3,
                CprNumber = "",
                FirstName = "Niels",
                LastName = "Patcher"
            };
        }

        protected override void AsssertEqualEntities(Person entity1, Person entity2)
        {
            Assert.AreEqual(entity1.Id, entity2.Id, "Id of the two persons should be the same");
            Assert.AreEqual(entity1.CprNumber, entity2.CprNumber, "CprNumber of the two persons should be the same");
            Assert.AreEqual(entity1.FirstName, entity2.FirstName, "FirstName of the two persons should be the same");
            Assert.AreEqual(entity1.LastName, entity2.LastName, "LastName of the two persons should be the same");
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id' : 4,   
                        'CprNumber' : '8754875412',
                        'FirstName' : 'Morten',
                        'LastName' : 'Bar'
                    }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'FirstName' : 'Niels',
                        'LastName' : 'Patcher'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/Person";
        }

        protected override void ReSeed()
        {
            new PersonRepositoryMock().ReSeed();
        }
    }
}
