using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices
{
    public class LicensePlateService : ILicensePlateService
    {
        private readonly IGenericRepository<LicensePlate> _repo;

        public LicensePlateService(IGenericRepository<LicensePlate> repo)
        {
            _repo = repo;
        }

        public bool MakeLicensePlatePrimary(int plateId)
        {
            // Get the plate to make primary.
            var plate = _repo.AsQueryable().SingleOrDefault(p => p.Id == plateId);
            
            if (plate == null)
            {
                // Return false if that plate doesnt exist.
                return false;
            }

            // Make all plates belonging to user not primary.
            var allPlatesForPerson = _repo.AsQueryable().Where(p => p.PersonId == plate.PersonId);
            foreach (var licensePlate in allPlatesForPerson)
            {
                licensePlate.IsPrimary = false;
            }
            // Make the requested plate primary.
            plate.IsPrimary = true;
            return true;

        }
    }
}
