using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class DriveReportsController : BaseController<DriveReport>
    {
        private readonly DriveReportService _driveService = new DriveReportService();

        public DriveReportsController(IGenericRepository<DriveReport> repo) : base(repo) {}

        // GET: odata/DriveReports
        [EnableQuery]
        public IQueryable<DriveReport> Get(ODataQueryOptions<DriveReport> queryOptions)
        {
            return _driveService.AddFullName(GetQueryable(queryOptions));
        }

        //GET: odata/DriveReports(5)
        public IQueryable<DriveReport> GetDriveReport([FromODataUri] int key, ODataQueryOptions<DriveReport> queryOptions)
        {
            return _driveService.AddFullName(GetQueryable(key, queryOptions));  
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
            return base.Post(driveReport);
        }

        // PATCH: odata/DriveReports(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<DriveReport> delta)
        {
            return base.Patch(key, delta);
        }

        // DELETE: odata/DriveReports(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
