using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface ISubstituteService
    {
        void ScrubCprFromPersons(IQueryable<Substitute> subs);
        long GetEndOfDayTimestamp(long timestamp);
        long GetStartOfDayTimestamp(long timestamp);
        bool CheckIfNewSubIsAllowed(Substitute newSub);
    }
}
