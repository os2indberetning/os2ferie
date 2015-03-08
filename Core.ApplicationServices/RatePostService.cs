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

        public bool DeactivateExistingRate(IQueryable<Rate> repo, Rate Rate)
        {
            if (!repo.AsQueryable().Any(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active)) return false;

            var res = repo.AsQueryable().First(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active);
            res.Active = false;
            return true;
        }


    }
}
