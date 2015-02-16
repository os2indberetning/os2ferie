using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;

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

        private readonly IGenericRepository<DriveReport> _repo = new GenericRepository<DriveReport>(new DataContext());

        private readonly DriveReportService _driveService = new DriveReportService();

        public DriveReportsController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
        }

        // GET: odata/DriveReports
        [EnableQuery]
        public IQueryable<DriveReport> Get(ODataQueryOptions<DriveReport> queryOptions)
        {
            var res = _driveService.AddFullName(_repo.AsQueryable());
            return res;
        }

        //GET: odata/DriveReports(5)
        public IQueryable<DriveReport> GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            var result = _repo.AsQueryable().FirstOrDefault(rep => rep.Id == key);

            _driveService.AddFullName(result);

            return new List<DriveReport>
            {
                result
            }.AsQueryable();
        }

        // PUT: odata/DriveReports(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<DriveReport> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        // POST: odata/DriveReports
        public IHttpActionResult Post(DriveReport driveReport)
        {
            Validate(driveReport);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var report = _repo.Insert(driveReport);
                _repo.Save();
                return Created(report);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

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

            var report = _repo.AsQueryable().FirstOrDefault(r => r.Id == key);
            if (report == null)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            try
            {
                delta.Patch(report);

                _repo.Save();
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Updated(report);
        }

        // DELETE: odata/DriveReports(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var report = _repo.AsQueryable().FirstOrDefault(r => r.Id == key);
            if (report == null)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
            try
            {
                _repo.Delete(report);
                _repo.Save();
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return StatusCode(HttpStatusCode.OK);
        }
    }
}
