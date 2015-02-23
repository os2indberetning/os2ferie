using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Core.DomainServices.Ínterfaces;
using NSubstitute;
using Substitute = NSubstitute.Substitute;

namespace ApplicationServices.Test.ReimbursementCalculatorTest
{
    public class ReimbursementCalculatorBaseTest
    {
        protected readonly string reportMethodIsRead = "Read";
        protected readonly string reportMethodIscalculated = "Calculated";
        protected readonly string reportMethodIscalculatedwithoutallowance = "CalculatedWithoutAllowance";

        protected IGenericRepository<PersonalAddress> GetPersonalAddressRepository()
        {
            var repo = Substitute.For<IGenericRepository<PersonalAddress>>();

            repo.AsQueryable().Returns(info => new List<PersonalAddress>().AsQueryable());

            return repo;
        }

        protected IReimbursementCalculator GetCalculator()
        {
            return new ReimbursementCalculator(GetRouter(), GetPersonalAddressRepository());
        }

        protected IRoute GetRouter()
        {
            var router = Substitute.For<IRoute>();

            router.GetRoute(Arg.Any<List<Address>>()).Returns(GetRouteInformation());

            return router;
        }

        protected RouteInformation GetRouteInformation()
        {
            return new RouteInformation()
            {
                 Duration = 1337,
                 EndStreet = "",
                 StartStreet = "",
                 GeoPoints = "",
                 Length = 42
            };
        }

        protected DriveReport GetDriveReport()
        {
            return new DriveReport()
            {
                KmRate = 1337,
                Distance = 42,
                Person = new Person() { DistanceFromHomeToBorder = 2, WorkDistanceOverride = 20 },
                PersonId = 1
            };
        }
    }
}