using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Ninject;

namespace Core.ApplicationServices
{

    public class ReimbursementCalculator : IReimbursementCalculator
    {
        private readonly IRoute<RouteInformation> _route;
        private readonly IPersonService _personService;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly IGenericRepository<Employment> _emplrepo;
        private readonly IGenericRepository<AddressHistory> _addressHistoryRepo;
        private const int FourKmAdjustment = 4;
        // Coordinate threshold is the amount two gps coordinates can differ and still be considered the same address.
        // Third decimal is 100 meters, so 0.001 means that addresses within 100 meters of each other will be considered the same when checking if route starts or ends at home.
        private const double CoordinateThreshold = 0.001;

        public ReimbursementCalculator(IRoute<RouteInformation> route, IPersonService personService, IGenericRepository<Person> personRepo, IGenericRepository<Employment> emplrepo, IGenericRepository<AddressHistory> addressHistoryRepo)
        {
            _route = route;
            _personService = personService;
            _personRepo = personRepo;
            _emplrepo = emplrepo;
            _addressHistoryRepo = addressHistoryRepo;
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
        public DriveReport Calculate(RouteInformation drivenRoute, DriveReport report)
        {
            //Check if user has manually provided a distance between home address and work address
            var homeWorkDistance = 0.0;

            var person = _personRepo.AsQueryable().First(x => x.Id == report.PersonId);

            var homeAddress = _personService.GetHomeAddress(person);

            // Get Work and Homeaddress of employment at time of DriveDateTimestamp for report.
            var addressHistory = _addressHistoryRepo.AsQueryable().SingleOrDefault(x => x.EmploymentId == report.EmploymentId && x.StartTimestamp < report.DriveDateTimestamp && x.EndTimestamp > report.DriveDateTimestamp);

            if (homeAddress.Type != PersonalAddressType.AlternativeHome)
            {
                if (addressHistory != null && addressHistory.HomeAddress != null)
                {
                    // If user doesn't have an alternative address set up then use the homeaddress at the time of DriveDateTimestamp
                    // If the user does have an alternative address then always use that.
                    homeAddress = addressHistory.HomeAddress;
                }
            }


            var employment = _emplrepo.AsQueryable().FirstOrDefault(x => x.Id.Equals(report.EmploymentId));
            
            Address workAddress = employment.OrgUnit.Address;

            if (addressHistory != null && addressHistory.WorkAddress != null)
            {
                // If an AddressHistory.WorkAddress exists, then use that.
                workAddress = addressHistory.WorkAddress;
            }

            
            if (employment.AlternativeWorkAddress != null)
            {
                // Overwrite workaddress if an alternative work address exists.
                workAddress = employment.AlternativeWorkAddress;
            }

            if (report.KilometerAllowance != KilometerAllowance.Read)
            {

                //Check if drivereport starts at users home address.
                report.StartsAtHome = areAddressesCloseToEachOther(homeAddress, report.DriveReportPoints.First());
                //Check if drivereport ends at users home address.
                report.EndsAtHome = areAddressesCloseToEachOther(homeAddress, report.DriveReportPoints.Last());
            }


            homeWorkDistance = employment.WorkDistanceOverride;

            if (homeWorkDistance <= 0)
            {
                homeWorkDistance = _route.GetRoute(DriveReportTransportType.Car, new List<Address>() { homeAddress, workAddress }).Length;
            }




            //Calculate distance to subtract
            double toSubtract = 0;

            //If user indicated to use the Four Km Rule
            if (report.FourKmRule)
            {
                //Take users provided distance from home to border of municipality
                var borderDistance = person.DistanceFromHomeToBorder;

                //Adjust distance based on if user starts or ends at home
                if (report.StartsAtHome)
                {
                    toSubtract += borderDistance;
                }

                if (report.EndsAtHome)
                {
                    toSubtract += borderDistance;
                }
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

            switch (report.KilometerAllowance)
            {
                case KilometerAllowance.Calculated:
                    {
                        if ((report.StartsAtHome || report.EndsAtHome) && !report.FourKmRule)
                        {
                            report.IsExtraDistance = true;
                        }


                        double drivenDistance = drivenRoute.Length;

                        //Adjust distance based on FourKmRule and if user start and/or ends at home
                        var correctDistance = drivenDistance - toSubtract;

                        //Set distance to corrected
                        report.Distance = correctDistance;

                        //Save RouteGeometry
                        report.RouteGeometry = drivenRoute.GeoPoints;

                        break;
                    }
                case KilometerAllowance.CalculatedWithoutExtraDistance:
                    {
                        report.Distance = drivenRoute.Length;

                        //Save RouteGeometry
                        report.RouteGeometry = drivenRoute.GeoPoints;


                        break;
                    }

                case KilometerAllowance.Read:
                    {
                        if ((report.StartsAtHome || report.EndsAtHome) && !report.FourKmRule)
                        {
                            report.IsExtraDistance = true;
                        }

                        //Take distance from report
                        var manuallyProvidedDrivenDistance = report.Distance;

                        report.Distance = manuallyProvidedDrivenDistance - toSubtract;

                        break;
                    }
                default:
                    {
                        throw new Exception("No calculation method provided");
                    }
            }

            //Calculate the actual amount to reimburse

            if (report.Distance < 0)
            {
                report.Distance = 0;
            }

            // Multiply the distance by two if the report is a return trip
            if (report.IsRoundTrip == true)
            {
                report.Distance *= 2;
            }

            if (report.FourKmRule)
            {
                report.Distance -= FourKmAdjustment;
            }

             SetAmountToReimburse(report);

            return report;
        }

        private void SetAmountToReimburse(DriveReport report)
        {
            // report.KmRate / 100 to go from ører to kroner.
            report.AmountToReimburse = (report.Distance) * (report.KmRate / 100);

            if (report.AmountToReimburse < 0)
            {
                report.AmountToReimburse = 0;
            }
        }

        /// <summary>
        /// Checks that two addresses are within 100 meters, in
        /// which case we assume they are the same when regarding
        /// if a person starts or ends their route at home.
        /// Longitude and latitude is called different things depending on
        /// whether we get the information from the backend or from septima
        /// </summary>
        /// <param name="address1">First address</param>
        /// <param name="address2">Second address</param>
        /// <returns>true if the two addresses are within 100 meters of each other.</returns>
        private bool areAddressesCloseToEachOther(Address address1, Address address2)
        {
            var long1 = Convert.ToDouble(address1.Longitude, CultureInfo.InvariantCulture);
            var long2 = Convert.ToDouble(address2.Longitude, CultureInfo.InvariantCulture);
            var lat1 = Convert.ToDouble(address1.Latitude, CultureInfo.InvariantCulture);
            var lat2 = Convert.ToDouble(address2.Latitude, CultureInfo.InvariantCulture);

            var longDiff = Math.Abs(long1 - long2);
            var latDiff = Math.Abs(lat1 - lat2);
            return longDiff < CoordinateThreshold && latDiff < CoordinateThreshold;
        }


    }
}