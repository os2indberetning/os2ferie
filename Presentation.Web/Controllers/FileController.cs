using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Optimization;
using Core.ApplicationServices;
using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using Core.DomainServices;
using log4net;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class FileController : BaseController<DriveReport>
    {
        private readonly IGenericRepository<DriveReport> _repo;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        public FileController(IGenericRepository<DriveReport> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo)
        {
            _repo = repo;
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
                Logger.Info("Fil til KMD genereret.");
                return Ok();
            }
            catch (Exception e)
            {
                Logger.Error("Fejl ved generering af fil til KMD", e);
                return InternalServerError();
            }
        }


    }
}