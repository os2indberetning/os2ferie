using System;
using System.Net;
using System.Web.Http;
using Core.ApplicationServices.FileGenerator;
using Core.ApplicationServices.Logger;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers.Drive
{
    public class FileController : BaseController<DriveReport>
    {
        private readonly IGenericRepository<DriveReport> _repo;

        private readonly ILogger _logger;

        public FileController(IGenericRepository<DriveReport> repo, IGenericRepository<Person> personRepo, ILogger logger) : base(repo, personRepo)
        {
            _repo = repo;
            _logger = logger;
        }

        //GET: Generate KMD File
        /// <summary>
        /// Generates KMD file.
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            if (!CurrentUser.IsAdmin)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            try
            {
                new ReportGenerator(_repo, new ReportFileWriter()).WriteRecordsToFileAndAlterReportStatus();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Log("Fejl ved generering af fil til KMD. Filen blev ikke genereret.", "web",e,1);
                return InternalServerError();
            }
        }


    }
}
