using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Core.ApplicationServices;
using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using Core.DomainServices;
using log4net;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class FileController : ApiController
    {
        private readonly IGenericRepository<DriveReport> _repo;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Person CurrentUser;

        private IGenericRepository<Person> _personRepo;

        protected override void Initialize(HttpControllerContext requestContext)
        {
            base.Initialize(requestContext);

#if DEBUG
            var httpUser = @"favrskov\FL".Split('\\'); // Fissirul Lehmann - administrator
#else
                string[] httpUser = User.Identity.Name.Split('\\');                
#endif

            if (httpUser.Length == 2 && String.Equals(httpUser[0], ConfigurationManager.AppSettings["AD_DOMAIN"], StringComparison.CurrentCultureIgnoreCase))
            {
                var initials = httpUser[1].ToLower();
                // DEBUG ON PRODUCTION. Set petsoe = lky
                if (initials == "petsoe" || initials == "itmind") { initials = "FL"; }
                // END DEBUG
                CurrentUser = _personRepo.AsQueryable().FirstOrDefault(p => p.Initials.ToLower().Equals(initials));
                if (CurrentUser == null)
                {
                    Logger.Error("AD-bruger ikke fundet i databasen (" + User.Identity.Name + ")");
                    throw new UnauthorizedAccessException("AD-bruger ikke fundet i databasen.");
                }
            }
            else
            {
                Logger.Info("Gyldig domænebruger ikke fundet (" + User.Identity.Name + ")");
                throw new UnauthorizedAccessException("Gyldig domænebruger ikke fundet.");
            }
        }

        public FileController(IGenericRepository<DriveReport> repo)
        {
            _repo = repo;
            _personRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Person>>();
        }

        //GET: Generate KMD File
        public IHttpActionResult Get()
        {
            if (!CurrentUser.IsAdmin)
            {
                return Unauthorized();
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