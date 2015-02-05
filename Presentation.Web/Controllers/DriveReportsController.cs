using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Routing;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Microsoft.Data.OData;

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

        public DriveReportsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
        }

        // GET: odata/DriveReports
        [EnableQuery]
        public IHttpActionResult GetDriveReports(ODataQueryOptions<DriveReport> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            //var driveReports = new GenericRepositoryImpl<DriveReport>(new DataContext()).AsQueryable();

            //return Ok<IEnumerable<DriveReport>>(driveReports);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // GET: odata/DriveReports(5)
        public IHttpActionResult GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            // return Ok<DriveReport>(driveReport);
            return StatusCode(HttpStatusCode.NotImplemented);
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
