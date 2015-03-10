using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Ninject;
using OS2Indberetning;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<DriveReport>>();

            new ReportGenerator(repo, new ReportFileWriter()).WriteRecordsToFileAndAlterReportStatus();
        }
    }
}
