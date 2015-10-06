using System.Collections.Generic;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IOrgUnitService
    {
        List<OrgUnit> GetWhereUserIsResponsible(int personId);
        IEnumerable<OrgUnit> GetChildOrgsWithoutLeader(int parentId);
        IEnumerable<int> GetIdsOfLeadersInImmediateChildOrgs(int parentOrgId);
    }
}
