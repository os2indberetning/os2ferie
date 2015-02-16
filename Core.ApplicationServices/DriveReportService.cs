using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace Core.ApplicationServices
{
    public class DriveReportService
    {
       public IQueryable<DriveReport> AddFullName(IQueryable<DriveReport> repo)
       {
           var set = repo.ToList(); 

            // Add fullname to the resultset
            foreach (var driveReport in set)
            {
                driveReport.Fullname = driveReport.Person.FirstName;

                if (!string.IsNullOrEmpty(driveReport.Person.MiddleName))
                {
                    driveReport.Fullname += " " + driveReport.Person.MiddleName;
                }

                driveReport.Fullname += " " + driveReport.Person.LastName;
            }
            return set.AsQueryable();
        } 
    }
}
