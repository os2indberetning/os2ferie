using System;
using System.Linq;
using System.Xml.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService;
using Core.DomainModel;
using Infrastructure.DataAccess;

namespace MailNotifier
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ms = new MailerService();

            var repo = new GenericRepository<DriveReport>(new DataContext());

            var pendingReports = repo.AsQueryable().ToList().Where(r => r.Status == 0);
            foreach (var report in pendingReports)
            {
                Console.WriteLine(report.Person.FirstName);
            }
            Console.ReadLine();
        }
    }
}
