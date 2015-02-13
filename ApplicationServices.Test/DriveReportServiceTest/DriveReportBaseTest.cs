using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    public class DriveReportBaseTest
    {
        protected DriveReport GetDriveReport()
        {
            return new DriveReport()
            {
                Person = new Person()
                {
                    FirstName = "Jacob",
                    MiddleName = "Overgaard",
                    LastName = "Jensen"
                }
            };
        }

        protected IQueryable<DriveReport> GetDriveReportAsQueryable()
        {
            return new List<DriveReport>()
            {
                GetDriveReport()
            }.AsQueryable();
        }

        protected DriveReportService GetDriveReportService()
        {
            return new DriveReportService();
        }
    }
}
