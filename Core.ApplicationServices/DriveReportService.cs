using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System.Runtime.InteropServices;
=======
>>>>>>> ba0c7ba0f4780a1f87ac78a47fedf66621011f4c
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
<<<<<<< HEAD
using Infrastructure.DataAccess;
=======
>>>>>>> ba0c7ba0f4780a1f87ac78a47fedf66621011f4c

namespace Core.ApplicationServices
{
    public class DriveReportService
    {
<<<<<<< HEAD
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
=======

>>>>>>> ba0c7ba0f4780a1f87ac78a47fedf66621011f4c
    }
}
