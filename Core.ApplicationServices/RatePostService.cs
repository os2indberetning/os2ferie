using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Classes;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;


namespace Core.ApplicationServices
{
    public class RatePostService
    {
        private readonly IRoute<RouteInformation> _route;
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<DriveReportPoint> _driveReportPointRepository;
        private readonly IGenericRepository<DriveReport> _driveReportRepository;
        private readonly IGenericRepository<LicensePlate> _licensePlateRepository;
        private readonly ReimbursementCalculator _calculator;

        public RatePostService()
        {
        }

    
       public void DeactivateExistingRate(IQueryable<Rate> repo, Rate Rate)
       {
           if (!repo.AsQueryable().Any(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active)) return;

           var res = repo.AsQueryable().First(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active);
           res.Active = false;
       }


    }
}
