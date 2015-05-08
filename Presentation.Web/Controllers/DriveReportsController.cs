using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
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
        private readonly IGenericRepository<Employment> _employmentRepo;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DriveReportsController(IGenericRepository<DriveReport> repo, IDriveReportService driveService, IGenericRepository<Person> personRepo , IGenericRepository<Employment> employmentRepo)
            : base(repo, personRepo)
        {
            _driveService = driveService;
            _employmentRepo = employmentRepo;
        }

        // GET: odata/DriveReports
        [EnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<DriveReport> queryOptions, string status = "", int leaderId = 0, bool getReportsWhereSubExists = false)
        {
            var queryable = GetQueryable(queryOptions);

            ReportStatus reportStatus;
            if (ReportStatus.TryParse(status, true, out reportStatus))
            {
                queryable = queryable.Where(dr => dr.Status == reportStatus);
            }

            if (leaderId != 0)
              {
                queryable = _driveService.FilterByLeader(queryable, leaderId, getReportsWhereSubExists);
            }

            var result = _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(queryable));

            return Ok(result);
        }

        //GET: odata/DriveReports(5)
        public IHttpActionResult GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            var res = _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(GetQueryable(key, queryOptions)));

            var report = res.AsQueryable().FirstOrDefault();

            if (report == null)
            {
                return NotFound();
            }

            if (CurrentUser.Id.Equals(report.PersonId))
            {
                return Ok(res);
            }
            if (CurrentUser.IsAdmin)
            {
                return Ok(res);
            }
            
            if (CurrentUser.Employments.Any(x => x.IsLeader && x.OrgUnitId.Equals(report.Employment.OrgUnitId)))
            {
                return Ok(res);
            }
            return StatusCode(HttpStatusCode.Forbidden);
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
            if (!CurrentUser.Id.Equals(driveReport.PersonId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = _driveService.Create(driveReport);

            return Ok(result);
        }

        // PATCH: odata/DriveReports(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<DriveReport> delta)
        {

            var report = Repo.AsQueryable().SingleOrDefault(x => x.Id == key);

            var leader = _driveService.GetResponsibleLeaderForReport(report);
            
            if (leader == null)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (!CurrentUser.Id.Equals(leader.Id))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            if (report == null)
            {
                return NotFound();
            }
            // Return Unauthorized if the status is not pending when trying to patch.
            // User should not be allowed to change a Report which has been accepted or rejected.
            if (report.Status != ReportStatus.Pending)
            {
                Logger.Info("Forsøg på at redigere indberetning med anden status end afventende.");
                return StatusCode(HttpStatusCode.Forbidden);
            }


            _driveService.SendMailIfRejectedReport(key, delta);
            return base.Patch(key, delta);
        }

        // DELETE: odata/DriveReports(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            if (CurrentUser.IsAdmin)
            {
                return base.Delete(key);
            }
            var report = Repo.AsQueryable().SingleOrDefault(x => x.Id.Equals(key));
            if (report == null)
            {
                return NotFound();
            }
            return report.PersonId.Equals(CurrentUser.Id) ? base.Delete(key) : Unauthorized();
        }
    }
}
