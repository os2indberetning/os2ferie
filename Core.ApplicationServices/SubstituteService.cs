
using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices
{
    public class SubstituteService : ISubstituteService
    {
        private readonly IGenericRepository<Substitute> _subRepo;

        public SubstituteService(IGenericRepository<Substitute> subRepo)
        {
            _subRepo = subRepo;
        }

        public void ScrubCprFromPersons(IQueryable<Substitute> subs)
        {
            foreach (var sub in subs.ToList())
            {
                sub.Sub.CprNumber = "";
                sub.Leader.CprNumber = "";
                sub.Person.CprNumber = "";

            }
        }

        public long GetStartOfDayTimestamp(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0).ToUniversalTime();
            var unixTimestamp = (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public long GetEndOfDayTimestamp(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).Date.AddDays(1).AddTicks(-1);
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999).ToUniversalTime();
            var unixTimestamp = (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public bool CheckIfNewSubIsAllowed(Substitute newSub)
        {
            // newSub is a substitute
            if (newSub.PersonId.Equals(newSub.LeaderId))
            {
                if (_subRepo.AsQueryable().Any(x => x.OrgUnitId.Equals(newSub.OrgUnitId) &&
                    ((newSub.StartDateTimestamp >= x.StartDateTimestamp && newSub.StartDateTimestamp <= x.EndDateTimestamp) ||
                    (newSub.StartDateTimestamp <= x.StartDateTimestamp && newSub.EndDateTimestamp >= x.StartDateTimestamp))))
                {
                    return false;
                }
            }
            // newSub is a personal approver
            else
            {
                if (_subRepo.AsQueryable().Any(x => x.PersonId.Equals(newSub.PersonId) && !x.PersonId.Equals(x.LeaderId) &&
                    ((newSub.StartDateTimestamp >= x.StartDateTimestamp && newSub.StartDateTimestamp <= x.EndDateTimestamp) ||
                    (newSub.StartDateTimestamp <= x.StartDateTimestamp && newSub.EndDateTimestamp >= x.StartDateTimestamp))))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
