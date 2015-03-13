using System;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class FileController : ApiController

{
    private readonly IGenericRepository<DriveReport> _repo;

    public FileController(IGenericRepository<DriveReport> repo)
    {
        _repo = repo;
    }

    //GET: Generate KMD File
    public IHttpActionResult Get()
    {
        try
        {
            new ReportGenerator(_repo, new ReportFileWriter()).WriteRecordsToFileAndAlterReportStatus();
            return Ok();
        }
        catch (Exception)
        {
            return InternalServerError();
        }
    }
}
}