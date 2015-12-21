using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly IGenericRepository<Employment> _emplRepo;
        private readonly IGenericRepository<OrgUnit> _orgRepo;

        public OrgUnitService(IGenericRepository<Employment> emplRepo, IGenericRepository<OrgUnit> orgRepo)
        {
            _emplRepo = emplRepo;
            _orgRepo = orgRepo;
        }

        public List<OrgUnit> GetWhereUserIsResponsible(int personId)
        {
            var result = new List<OrgUnit>();

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var leaderEmpls = _emplRepo.AsQueryable().Where(e => e.IsLeader && e.PersonId == personId && (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp)).ToList(); // ToList to force close the datareader
            foreach (var employment in leaderEmpls)
            {
                var empl = employment;
                result.Add(empl.OrgUnit);
                result.AddRange(GetChildOrgsWithoutLeader(empl.OrgUnitId));
            }
            return result;
        }

        public IEnumerable<OrgUnit> GetChildOrgsWithoutLeader(int parentOrgId)
        {
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var result = new List<OrgUnit>();
            var childOrgs = _orgRepo.AsQueryable().Where(org => org.ParentId == parentOrgId).ToList(); // ToList to force close the datareader.
            foreach (var childOrg in childOrgs)
            {
                var org = childOrg;

                // Add all orgs that dont have an active leader.
                if (!_emplRepo.AsQueryable().Any(e => e.IsLeader && e.OrgUnitId == org.Id && e.StartDateTimestamp < currentTimestamp && (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp)))
                {
                    result.Add(org);
                    result.AddRange(GetChildOrgsWithoutLeader(org.Id));
                }
            }
            return result;
        }

        public Person GetLeaderOfOrg(int orgUnitId)
        {
            var orgUnit = _orgRepo.AsQueryable().Single(x => x.Id == orgUnitId);
            var empl = _emplRepo.AsQueryable().FirstOrDefault(x => x.OrgUnitId == orgUnitId && x.IsLeader);
            if (empl == null)
            {
                var parent = orgUnit.Parent;
                empl = _emplRepo.AsQueryable().FirstOrDefault(x => x.IsLeader && x.OrgUnitId == parent.Id);
                while (empl == null && parent.Level > 0)
                {
                    parent = parent.Parent;
                    empl = _emplRepo.AsQueryable().FirstOrDefault(x => x.IsLeader && x.OrgUnitId == parent.Id);
                }
            }
            if (empl == null)
            {
                return new Person
                {
                    FullName = "Leder ikke fundet"
                };
            }
            return empl.Person;
        }

        public IEnumerable<int> GetIdsOfLeadersInImmediateChildOrgs(int parentOrgId)
        {
            var childOrgsToSearch = new HashSet<int>();
            var result = new List<int>();
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            // Get all employments of childorgs
            var immediateChildEmpls = _emplRepo.AsQueryable()
                .Where(e => e.OrgUnit.ParentId == parentOrgId && e.StartDateTimestamp < currentTimestamp && (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp)).ToList();

            // Return in case we have reached an end node.
            if (!immediateChildEmpls.Any())
            {
                return new List<int>();
            }

            // Get orgunits belonging to these employments
            var immediateChildOrgs = immediateChildEmpls.Select(e => e.OrgUnit);
            
            // Add to the resultset the ids of leaders in immediate child orgs.
            result.AddRange(immediateChildEmpls.Where(e => e.IsLeader).Select(e => e.PersonId));

            // Iterate each childorg, and add the ones with no leader to the list of orgs to be searched recursively.
            foreach (var org in immediateChildOrgs)
            {
                if (!immediateChildEmpls.Any(e => e.IsLeader && e.OrgUnitId == org.Id))
                {
                    // ChildOrgsToSearch is a HashSet so duplicate values will be ignored.
                    childOrgsToSearch.Add(org.Id);
                }
            }

            // Recursively search each child org with no leader.
            foreach (var org in childOrgsToSearch)
            {
                result.AddRange(GetIdsOfLeadersInImmediateChildOrgs(org));
            }
            
            return result;
        }
    }
}
