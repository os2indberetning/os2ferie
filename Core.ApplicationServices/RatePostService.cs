using System.Linq;
using Core.DomainModel;

namespace Core.ApplicationServices
{
    public class RatePostService
    {
        /// <summary>
        /// Deactivates any existing rate with the same year and type as Rate.
        /// </summary>
        /// <param name="repo">Existing rates</param>
        /// <param name="Rate">New Rate</param>
        /// <returns>True if one existed already. False otherwise.</returns>
        public bool DeactivateExistingRate(IQueryable<Rate> repo, Rate Rate)
        {
            if (!repo.AsQueryable().Any(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active)) return false;

            var res = repo.AsQueryable().First(r => r.Year == Rate.Year && r.TypeId == Rate.TypeId && r.Active);
            res.Active = false;
            return true;
        }


    }
}
