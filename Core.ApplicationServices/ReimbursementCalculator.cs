using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;

namespace Core.ApplicationServices
{
    public class ReimbursementCalculator : IReimbursementCalculator
    {
        private readonly IRoute _route;
        private readonly IGenericRepository<PersonalAddress> _addressRepo;

        public ReimbursementCalculator()
        {
            _route = new BestRoute();
        }

        public DriveReport Calculate(DriveReport report, string reportMethod)
        {
            var result = report;

            var startAddress = report.DriveReportPoints.First(x => x.PreviousPointId == null);
            var endAddress = report.DriveReportPoints.First(x => x.NextPointId == null);

            var homeAddress = GetHomeAddress(report);
            var workAddress = GetWorkAddress(report);

            if (reportMethod.ToLower() == "aflæst")
            {

            }
            else
            {
                if (report.FourKmRule)
                {
                    
                }
                else
                {
                    
                }
            }



            var route = _route.GetRoute(report.DriveReportPoints);





            return result;
        }

        private PersonalAddress GetHomeAddress(DriveReport report)
        {
            var hasAlternative = _addressRepo.AsQueryable()
                    .First(x => x.PersonId == report.PersonId && x.Type == PersonalAddressType.AlternativeHome);

            if (hasAlternative != null)
            {
                return hasAlternative;
            }

            var home = _addressRepo.AsQueryable()
                    .First(x => x.PersonId == report.PersonId && x.Type == PersonalAddressType.Home);

            return home;
        }

        private PersonalAddress GetWorkAddress(DriveReport report)
        {
            var hasAlternative = _addressRepo.AsQueryable()
                    .First(x => x.PersonId == report.PersonId && x.Type == PersonalAddressType.AlternativeWork);

            if (hasAlternative != null)
            {
                return hasAlternative;
            }

            var work = _addressRepo.AsQueryable()
                    .First(x => x.PersonId == report.PersonId && x.Type == PersonalAddressType.Work);

            return work;
        }
    }
}