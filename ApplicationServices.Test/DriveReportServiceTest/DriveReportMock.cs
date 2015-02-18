using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    public class DriveReportMock
    {
        private List<DriveReport> getDriveReports()
        {

            return new List<DriveReport>
            {

                new DriveReport()
                {
                    Person = new Person()
                    {
                        FirstName = "Jacob",
                        MiddleName = "Overgaard",
                        LastName = "Jensen"
                    }
                },
                new DriveReport()
                {
                    Person = new Person()
                    {
                        FirstName = "Morten",
                        MiddleName = "",
                        LastName = "Rasmussen"
                    }
                }
            };
        }

        protected IQueryable<DriveReport> GetDriveReportAsQueryable()
        {
            return getDriveReports().AsQueryable();
        }
    }
}
