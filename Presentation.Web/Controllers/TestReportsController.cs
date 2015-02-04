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
using Core.DomainModel.Example;

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

        public TestReportsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
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
                    ReportedDate = DateTime.Now,
                    Type = "Rumraket"
                },
                new TestReport()
                {          
                    Id = 2,
                    Name = "Gunner Testesen",
                    Purpose = "OData Web API Test",
                    ReportedDate = DateTime.Now,
                    Type = "Sutsko"                
                },
                new TestReport()
                {
                    Id = 3,
                    Name = "Bertram Didriksen",
                    Purpose = "Server Paging Test",
                    ReportedDate = DateTime.Now,
                    Type = "Flyver"
                },
                new TestReport()
                {
                    Id = 4,
                    Name = "Oluf Petersen",
                    Purpose = "Test",
                    ReportedDate = DateTime.Now,
                    Type = "Båd"
                },
                new TestReport()
                {
                    Id = 5,
                    Name = "Alfred Butler",
                    Purpose = "Opvartning",
                    ReportedDate = DateTime.Now,
                    Type = "Batmobil"
                },
                new TestReport()
                {
                    Id = 6,
                    Name = "Bruce Wayne",
                    Purpose = "Flyve",
                    ReportedDate = DateTime.Now,
                    Type = "Vinger"
                },
                new TestReport()
                {
                    Id = 7,
                    Name = "John Snow",
                    Purpose = "To know something",
                    ReportedDate = DateTime.Now,
                    Type = "Wolf"
                }
            }.AsQueryable();

            return testReports;
        }
    }
}
