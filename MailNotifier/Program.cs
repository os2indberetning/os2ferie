using System;
using System.Linq;
using System.Xml.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService;
using Core.ApplicationServices.MailerService.Impl;
using Core.DomainModel;
using Infrastructure.DataAccess;

namespace MailNotifier
{
    public class Program
    {
        static void Main(string[] args)
        {
            var mailService = new MailService(new GenericRepository<DriveReport>(), new GenericRepository<Substitute>());
            mailService.GetLeadersWithPendingReportsMails();
            Console.ReadLine();
        }
    }
}
