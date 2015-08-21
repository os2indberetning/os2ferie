using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

        /// <summary>
        /// Makes the license plate identified by plateId the primary license plate.
        /// Makes all other license plates for the same person not primary.
        /// </summary>
        /// <param name="plateId"></param>
        /// <returns>True if successfull, false if the plate doesnt exist.</returns>
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
            _repo.Save();
            return true;

        }

        public LicensePlate HandlePost(LicensePlate plate)
        {
            if (!_repo.AsQueryable().Any(lp => lp.PersonId == plate.PersonId))
            {
                plate.IsPrimary = true;
            }
            return plate;
        }

        public void HandleDelete(LicensePlate plate)
        {
            if (plate != null && plate.IsPrimary)
            {
                // Find a new plate to make primary.
                var newPrimary = _repo.AsQueryable().FirstOrDefault(lp => lp.PersonId == plate.PersonId && lp.Id != plate.Id);
                if (newPrimary != null)
                {
                    MakeLicensePlatePrimary(newPrimary.Id);
                }
            }
        }
    }
}
