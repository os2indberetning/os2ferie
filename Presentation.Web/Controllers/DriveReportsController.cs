using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Migrations;
using Microsoft.OData.Core;
using Ninject;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<DriveReport>("DriveReports");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class DriveReportsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<DriveReport> _repo;

        private readonly DriveReportService _driveService;

        public DriveReportsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
            _driveService = new DriveReportService();
            _repo = new GenericRepository<DriveReport>(new DataContext());
        }

        // GET: odata/DriveReports
        [EnableQuery]
        public IQueryable<DriveReport> Get(ODataQueryOptions<DriveReport> queryOptions)
        {
<<<<<<< HEAD
            var res = _driveService.AddFullName(_repo.AsQueryable());
            return res;
=======
            // ToList otherwise the foreach loop causes an exception
            var driveReports = _repo.AsQueryable().ToList();

            // Add fullname and human readable timestamp to the resultset
            foreach (var driveReport in driveReports)
            {               
                driveReport.Fullname = driveReport.Person.FirstName;

                if (!string.IsNullOrEmpty(driveReport.Person.MiddleName))
                {
                    driveReport.Fullname += " " + driveReport.Person.MiddleName;
                }
                driveReport.Fullname += " " + driveReport.Person.LastName;

                driveReport.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(driveReport.CreatedDateTimestamp).ToShortDateString();
            }


            // Back to AsQueryable for Kendo
            return driveReports.AsQueryable();
            //return StatusCode(HttpStatusCode.NotImplemented);
>>>>>>> ba0c7ba0f4780a1f87ac78a47fedf66621011f4c
        }

        //GET: odata/DriveReports(5)
        public IQueryable<DriveReport> GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            var result = _repo.AsQueryable().FirstOrDefault(rep => rep.Id == key);


            return new List<DriveReport>()
            {
                result
            }.AsQueryable();
        }

        // PUT: odata/DriveReports(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<DriveReport> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Put(driveReport);

            // TODO: Save the patched entity.

            // return Updated(driveReport);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // POST: odata/DriveReports
        public IHttpActionResult Post(DriveReport driveReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Add create logic here.

            // return Created(driveReport);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PATCH: odata/DriveReports(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<DriveReport> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Patch(driveReport);

            // TODO: Save the patched entity.

            // return Updated(driveReport);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // DELETE: odata/DriveReports(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            // TODO: Add delete logic here.

            // return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
