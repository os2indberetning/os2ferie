using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices.Interfaces;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;

namespace Core.ApplicationServices
{
    
    public class ReimbursementCalculator : IReimbursementCalculator
    {
        private readonly IRoute _route;
        private readonly IGenericRepository<PersonalAddress> _addressRepo;
        private readonly IGenericRepository<Person> _personRepo; 

        public ReimbursementCalculator()
        {
            _route = new BestRoute();
            _addressRepo = new GenericRepository<PersonalAddress>(new DataContext());
            _personRepo = new GenericRepository<Person>(new DataContext());
        }

        public ReimbursementCalculator(IRoute route, IGenericRepository<PersonalAddress> addressRepo, IGenericRepository<Person> personRepo)
        {
            _route = route;
            _addressRepo = addressRepo;
            _personRepo = personRepo;
        }

        /// <summary>
        /// Takes a DriveReport as input and returns it with data.
        /// 
        /// FourKmRule: If a user has set the FourKmRule to be used, the distance between 
        /// the users home and municipality is used in the correction of the driven distance.
        /// If the rule is not used, the distance between the users home and work address are 
        /// calculated and used, provided that the user has not set a override for this value.
        /// 
        /// Calculated: The driven route is calculated, and based on whether the user starts 
        /// and/or ends at home, the driven distance is corrected by subtracting the distance 
        /// between the users home address and work address. 
        /// Again, this is dependend on wheter the user has overridden this value.
        /// 
        /// Calculated without extra distance: If this method is used, the driven distance is 
        /// still calculated, but the distance is not corrected with the distance between the 
        /// users home address and work address. The distance calculated from the service is 
        /// therefore used directly in the calculation of the amount to reimburse
        /// 
        /// </summary>
        public DriveReport Calculate(DriveReport report, string reportMethod)
        {            
            //Check if user has manually provided a distance between home address and work address
            var homeWorkDistance = 0.0;

            var person = _personRepo.AsQueryable().First(x => x.Id == report.PersonId);

            if (person.WorkDistanceOverride > 0)
            {
                homeWorkDistance = person.WorkDistanceOverride;
            }
            else
            {
                var homeAddress = GetHomeAddress(report);
                var workAddress = GetWorkAddress(report);
                homeWorkDistance = _route.GetRoute(new List<Address>() { homeAddress, workAddress }).Length;    
            }
            
            //Calculate distance to subtract
            double toSubtract = 0;

            //If user indicated to use the Four Km Rule
            if (report.FourKmRule)
            {
                //Take users provided distance from home to border of municipality
                var borderDistance = report.Person.DistanceFromHomeToBorder * 1000;

                //Adjust distance based on if user starts or ends at home
                if (report.StartsAtHome)
                {
                    toSubtract += borderDistance;
                }

                if (report.EndsAtHome)
                {
                    toSubtract += borderDistance;
                }

                //Subtract 4 km because reasons.
                toSubtract += 4000;
            }
            else
            {
                //Same logic as above, but uses calculated distance between home and work
                if (report.StartsAtHome)
                {
                    toSubtract += homeWorkDistance;
                }

                if (report.EndsAtHome)
                {
                    toSubtract += homeWorkDistance;
                }
            }

            // If user manually provided a driven distance
            if (reportMethod.ToLower() == "read")
            {
                //Take distance from report
                var manuallyProvidedDrivenDistance = report.Distance;                               

                report.Distance = manuallyProvidedDrivenDistance - toSubtract;
            }
            // Use route service to calculate the driven route
            else if (reportMethod.ToLower() == "calculated")
            {
                //Calculate the driven route
                var drivenRoute = _route.GetRoute(report.DriveReportPoints);

                double drivenDistance = drivenRoute.Length;

                //Adjust distance based on FourKmRule and if user start and/or ends at home
                var correctDistance = drivenDistance - toSubtract;

                //Set distance to corrected
                report.Distance = correctDistance;
            }
            // Use route service to calculate the driven route, but with no correction of the length of the route
            else if (reportMethod.ToLower() == "calculatedwithoutextradistance")
            {
                //Calculate the driven route
                var drivenRoute = _route.GetRoute(report.DriveReportPoints);

                report.Distance = drivenRoute.Length;
            }
            else
            {
                throw new Exception("No calculation method provided");
            }

            //Calculate the actual amount to reimburse
            report.AmountToReimburse = (report.Distance / 1000) * (report.KmRate / 100);

            report.Distance = report.Distance/1000;

            return report;
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