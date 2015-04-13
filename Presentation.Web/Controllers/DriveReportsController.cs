using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using log4net;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class DriveReportsController : BaseController<DriveReport>
    {
        private readonly IDriveReportService _driveService;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DriveReportsController(IGenericRepository<DriveReport> repo, IDriveReportService driveService)
            : base(repo)
        {
            _driveService = driveService;
        }

        // GET: odata/DriveReports
        [EnableQuery]
        public IQueryable<DriveReport> Get(ODataQueryOptions<DriveReport> queryOptions, string status = "", int leaderId = 0, bool getReportsWhereSubExists = false)
        {
            var queryably = GetQueryable(queryOptions);

            ReportStatus reportStatus;
            if (ReportStatus.TryParse(status, true, out reportStatus))
            {
                queryably = queryably.Where(dr => dr.Status == reportStatus);
            }

            if (leaderId != 0)
            {
                queryably = _driveService.FilterByLeader(queryably, leaderId, getReportsWhereSubExists);
            }

            return _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(queryably));
        }

        //GET: odata/DriveReports(5)
        public IQueryable<DriveReport> GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            var res = _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(GetQueryable(key, queryOptions)));
            return res;
        }

        // PUT: odata/DriveReports(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<DriveReport> delta)
        {
            return base.Put(key, delta);
        }

        // POST: odata/DriveReports
        [EnableQuery]
        public new IHttpActionResult Post(DriveReport driveReport)
        {
            //try
            //{
            var result = _driveService.Create(driveReport);

            return Ok(result);
            //}
            //catch (Exception e)
            //{
            //    return BadRequest("DriveReport has some invalid parameters");
            //}
        }

        // PATCH: odata/DriveReports(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<DriveReport> delta)
        {

           
            

            var report = Repo.AsQueryable().SingleOrDefault(x => x.Id == key);
            if (report == null)
            {
                return NotFound();
            }
            // Return Unauthorized if the status is not pending when trying to patch.
            // User should not be allowed to change a Report which has been accepted or rejected.
            if (report.Status != ReportStatus.Pending)
            {
                Logger.Info("Forsøg på at redigere indberetning med anden status end afventende.");
                return Unauthorized();
            }


            _driveService.SendMailIfRejectedReport(key, delta);
            return base.Patch(key, delta);
        }

        // DELETE: odata/DriveReports(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
