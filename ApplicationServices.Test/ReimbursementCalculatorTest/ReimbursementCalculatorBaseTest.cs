using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using Infrastructure.DataAccess;
using NSubstitute;
using Substitute = NSubstitute.Substitute;

namespace ApplicationServices.Test.ReimbursementCalculatorTest
{
    public class ReimbursementCalculatorBaseTest
    {


        protected IPersonService GetPersonServiceMock()
        {



            var personService = Substitute.For<IPersonService>();

            personService.GetHomeAddress(new Person()).ReturnsForAnyArgs(info =>
                new PersonalAddress()
                {
                    Description = "TestHomeAddress",
                    Id = 1,
                    Type = PersonalAddressType.Home,
                    PersonId = 1,
                    StreetName = "Jens Baggesens Vej",
                    StreetNumber = "46",
                    ZipCode = 8210,
                    Town = "Aarhus"
                });

            return personService;
        }

        protected IGenericRepository<Person> GetPersonRepository()
        {
            var repo = Substitute.For<IGenericRepository<Person>>();

            repo.AsQueryable().Returns(info => new List<Person>()
            {
                new Person()
                {
                    Id = 1,
                    FirstName = "Jacob",
                    MiddleName = "Overgaard",
                    LastName = "Jensen",
                    DistanceFromHomeToBorder = 2,
                }
            }.AsQueryable());

            return repo;
        } 

        protected IReimbursementCalculator GetCalculator()
        { //TODO changed to make the code compile
            return new ReimbursementCalculator(new RouterMock(), GetPersonServiceMock(), GetPersonRepository(), new GenericRepository<Employment>(new DataContext()));
        }

        protected DriveReport GetDriveReport()
        {
            return new DriveReport()
            {
                KmRate = 1337,
                Distance = 42,
                PersonId = 1,
                DriveReportPoints = new List<DriveReportPoint>()
                {
                    new DriveReportPoint()
                    {
                        Id = 1
                    },
                    new DriveReportPoint()
                    {
                        Id = 2
                    }
                }
            };
        }
    }

    class RouterMock : IRoute<RouteInformation>
    {
        public RouteInformation GetRoute(IEnumerable<Address> addresses)
        {
            return new RouteInformation()
            {
                Duration = 1337,
                EndStreet = "Katrinebjergvej 95, 8200 Aarhus",
                StartStreet = "Katrinebjergvej 40, 8200 Aarhus",
                GeoPoints = "",
                Length = 42
            };
        }
    }
}