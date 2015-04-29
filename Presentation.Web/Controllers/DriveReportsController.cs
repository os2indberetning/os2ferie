﻿using System;
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

            var result = _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(queryably));

            // Return result if CurrentUser is Admin
            if (CurrentUser.IsAdmin)
            {
                return Ok(result);
            }

            // Return result if currentUser is leader and responsible for the reports in the result. 
            if (leaderId.Equals(CurrentUser.Id))
            {
                return Ok(result);
            }

            // Return if result doesnt contain reports belonging to someone else than currentuser
            if (!result.Any(rep => !rep.PersonId.Equals(CurrentUser.Id)))
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        //GET: odata/DriveReports(5)
        public IHttpActionResult GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            var res = _driveService.AddApprovedByFullName(_driveService.AttachResponsibleLeader(GetQueryable(key, queryOptions)));

            var ra = res.AsQueryable().FirstOrDefault();

            if (ra == null)
            {
                return NotFound();
            }

            if (CurrentUser.Id.Equals(ra.PersonId))
            {
                return Ok(res);
            }
            if (CurrentUser.IsAdmin)
            {
                return Ok(res);
            }
            if (CurrentUser.Employments.Any(x => x.IsLeader && x.OrgUnitId.Equals(ra.Employment.OrgUnitId)))
            {
                return Ok(res);
            }
            return Unauthorized();
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
            if (!CurrentUser.Id.Equals(driveReport.PersonId))
            {
                return Unauthorized();
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
            var leader = _employmentRepo.AsQueryable().FirstOrDefault(x => x.IsLeader && x.OrgUnitId.Equals(report.Employment.OrgUnitId));
            
            
            if (leader == null)
            {
                return Unauthorized();
            }

            if (!CurrentUser.Id.Equals(leader.Id))
            {
                return Unauthorized();
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
                return Unauthorized();
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
