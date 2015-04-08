using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Core.DomainModel;
using Core.DomainModel.Example;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using OS2Indberetning.Controllers;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<TestReport>("TestReports");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TestReportsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<Person> _genericPersonRepo;
        private readonly IGenericRepository<DriveReport> _genericDriveReportRepo;
        private readonly IGenericRepository<OrgUnit> _genericOrgUnitRepo;
        private readonly IGenericRepository<Employment> _genericEmploymentRepo;
        private readonly IGenericRepository<PersonalRoute> _genericPersonalRouteRepo;
        private readonly IGenericRepository<Point> _genericPointRepo;
        

        public TestReportsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
            _genericPersonRepo = new GenericRepository<Person>(new DataContext());
            _genericDriveReportRepo = new GenericRepository<DriveReport>(new DataContext());
            _genericOrgUnitRepo = new GenericRepository<OrgUnit>(new DataContext());
            _genericEmploymentRepo = new GenericRepository<Employment>(new DataContext());
            _genericPersonalRouteRepo = new GenericRepository<PersonalRoute>(new DataContext());
            _genericPointRepo = new GenericRepository<Point>(new DataContext());
        }

        // GET: odata/TestReports
        [EnableQuery]
        public IQueryable<TestReport> Get(ODataQueryOptions queryOptions)
        {
            IQueryable<TestReport> testReports = new List<TestReport>()
            {
                new TestReport()
                {
                    Id = 1,
                    Name = "Test Testesen",
                    Purpose = "Kendo UI Grid Test",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Rumraket"
                },
                new TestReport()
                {          
                    Id = 2,
                    Name = "Gunner Testesen",
                    Purpose = "OData Web API Test",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Sutsko"                
                },
                new TestReport()
                {
                    Id = 3,
                    Name = "Bertram Didriksen",
                    Purpose = "Server Paging Test",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Flyver"
                },
                new TestReport()
                {
                    Id = 4,
                    Name = "Oluf Petersen",
                    Purpose = "Test",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Båd"
                },
                new TestReport()
                {
                    Id = 5,
                    Name = "Alfred Butler",
                    Purpose = "Opvartning",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Batmobil"
                },
                new TestReport()
                {
                    Id = 6,
                    Name = "Bruce Wayne",
                    Purpose = "Flyve",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Vinger"
                },
                new TestReport()
                {
                    Id = 7,
                    Name = "John Snow",
                    Purpose = "To know something",
                    Timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Type = "Wolf"
                }
            }.AsQueryable();

            return testReports;
        }

        // POST: odata/TestReports
        [EnableQuery]
        public void Post(Person person)
        {
            var personResult = _genericPersonRepo.Insert(new Person()
            {
                CprNumber = "1234567890",
                FirstName = "Test",
                MiddleName = "Tester",
                LastName = "Testesen",
                Mail = "123@456.78",
                PersonId = 1234,
                WorkDistanceOverride = 0,
            });

            _genericPersonRepo.Save();


            var personalRouteResult = _genericPersonalRouteRepo.Insert(new PersonalRoute()
            {
                Description = "TestRoute",
                PersonId = personResult.Id
            });

            _genericPersonalRouteRepo.Save();

            var point1Result = _genericPointRepo.Insert(new Point()
            {
                Description = "Test1",
                StreetName = "Testvej",
                StreetNumber = "42A",
                Town = "Testby vester",
                ZipCode = 1234,
                Latitude = "1234567890",
                Longitude = "1234567890",
                PersonalRouteId = personalRouteResult.Id
            });

            _genericPointRepo.Save();

            var point2Result = _genericPointRepo.Insert(new Point()
            {
                Description = "Test2",
                StreetName = "Testvej2",
                StreetNumber = "A42",
                Town = "Testby øster",
                ZipCode = 4321,
                Latitude = "0987654321",
                Longitude = "0987654321",
                PersonalRouteId = personalRouteResult.Id,
                PreviousPointId = point1Result.Id
            });

            _genericPointRepo.Save();

            point1Result.NextPointId = point2Result.Id;

            _genericPointRepo.Update(point1Result);

            _genericPointRepo.Save();

            //var driveReportResult = _genericDriveReportRepo.Insert(new DriveReport()
            //{
            //    KmRate = 42,
            //    AmountToReimburse = 42,                
            //    CreatedDateTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
            //    LicensePlate = "AH 12 345",
            //    PersonId = personResult.Id,
            //    Purpose = "Test",


            //});
        }
    }
}
