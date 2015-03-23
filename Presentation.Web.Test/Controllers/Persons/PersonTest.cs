using System;
using System.Collections.Generic;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.Persons
{
    //Notice that the controller scrubs away the cpr number
    //before serving the person entity.

    [TestFixture]
    public class PersonTest : BaseControllerTest<Person>
    {
        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<Person>), typeof (PersonRepositoryMock)),
                new KeyValuePair<Type, Type>(typeof(IPersonService), typeof(PersonService)),
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<Employment>), typeof(GenericRepository<Employment>)),
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<LicensePlate>), typeof(GenericRepository<LicensePlate>)),
                new KeyValuePair<Type, Type>(typeof(IGenericRepository<PersonalAddress>), typeof(GenericRepository<PersonalAddress>)),
                new KeyValuePair<Type, Type>(typeof(IRoute<RouteInformation>), typeof(BestRoute)),
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
