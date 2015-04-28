using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using Ninject;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;

namespace DBUpdater
{
    public class UpdateService
    {
        private readonly IGenericRepository<Employment> _emplRepo;
        private readonly IGenericRepository<OrgUnit> _orgRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly IGenericRepository<CachedAddress> _cachedRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private readonly IAddressLaunderer _actualLaunderer;
        private readonly IAddressCoordinates _coordinates;
        private readonly IDBUpdaterDataProvider _dataProvider;

        public UpdateService(IGenericRepository<Employment> emplRepo, IGenericRepository<OrgUnit> orgRepo, IGenericRepository<Person> personRepo, IGenericRepository<CachedAddress> cachedRepo, IGenericRepository<PersonalAddress> personalAddressRepo, IAddressLaunderer actualLaunderer, IAddressCoordinates coordinates, IDBUpdaterDataProvider dataProvider)
        {
            _emplRepo = emplRepo;
            _orgRepo = orgRepo;
            _personRepo = personRepo;
            _cachedRepo = cachedRepo;
            _personalAddressRepo = personalAddressRepo;
            _actualLaunderer = actualLaunderer;
            _coordinates = coordinates;
            _dataProvider = dataProvider;
        }

        public List<String> SplitAddressOnNumber(string address)
        {
            var result = new List<string>();
            var index = address.IndexOfAny("0123456789".ToCharArray());
            result.Add(address.Substring(0, index - 1));
            result.Add(address.Substring(index, address.Length - index));
            return result;
        }
      
        public void MigrateOrganisations()
        {
            var orgs = _dataProvider.GetOrganisationsAsQueryable().OrderBy(x => x.Level);


            foreach (var org in orgs)
            {
                var orgToInsert = _orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId == org.LOSOrgId);

                if (orgToInsert == null)
                {
                    orgToInsert = _orgRepo.Insert(new OrgUnit());
                }

                orgToInsert.Level = org.Level;
                orgToInsert.LongDescription = org.Navn;
                orgToInsert.ShortDescription = org.KortNavn;
                orgToInsert.HasAccessToFourKmRule = false;
                orgToInsert.OrgId = org.LOSOrgId;

                if (orgToInsert.Level > 0)
                {
                    orgToInsert.ParentId = _orgRepo.AsQueryable().Single(x => x.OrgId == org.ParentLosOrgId).Id;
                }

                _orgRepo.Save();
            }
        }

        public void MigrateEmployees()
        {
            var empls = _dataProvider.GetEmployeesAsQueryable();

            foreach (var employee in empls)
            {
                var personToInsert = _personRepo.AsQueryable().FirstOrDefault(x => x.PersonId == employee.MaNr);

                if (personToInsert == null)
                {
                    personToInsert = _personRepo.Insert(new Person());
                }

                personToInsert.CprNumber = employee.CPR;
                personToInsert.RecieveMail = false;
                personToInsert.FirstName = employee.Fornavn;
                personToInsert.LastName = employee.Efternavn;
                personToInsert.IsAdmin = false;
                personToInsert.Initials = employee.ADBrugerNavn ?? " ";
                personToInsert.Mail = employee.Email ?? "";
                personToInsert.PersonId = employee.MaNr ?? default(int);

                _personRepo.Save();

                CreateEmployment(employee, personToInsert.Id);
                SaveHomeAddress(employee, personToInsert.Id);
            }
        }


        public Employment CreateEmployment(Employee empl, int personId)
        {
            var orgUnit = _orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId == empl.LOSOrgId);

            if (orgUnit == null)
            {
                throw new Exception("OrgUnit does not exist.");
            }

            if (!_personRepo.AsQueryable().Any(x => x.Id == personId))
            {
                throw new Exception("Person does not exist.");
            }

            var employment = _emplRepo.AsQueryable().FirstOrDefault(x => x.OrgUnitId == orgUnit.Id && x.PersonId == personId);

            if (employment == null)
            {
                employment = _emplRepo.Insert(new Employment());
            }

            employment.OrgUnitId = orgUnit.Id;
            employment.Position = empl.Stillingsbetegnelse ?? "";
            employment.IsLeader = empl.Leder;
            employment.PersonId = personId;
            var startDate = empl.AnsaettelsesDato ?? new DateTime();
            employment.StartDateTimestamp = (Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            employment.ExtraNumber = empl.EkstraCiffer ?? 0;
            employment.EmploymentType = int.Parse(empl.AnsatForhold);

            if (empl.OphoersDato != null)
            {
                employment.EndDateTimestamp = (Int32)(((DateTime)empl.OphoersDato).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            else
            {
                employment.EndDateTimestamp = 0;
            }

            _emplRepo.Save();
            return employment;


        }

        public void SaveHomeAddress(Employee empl, int personId)
        {

            if (!_personRepo.AsQueryable().Any(x => x.Id == personId))
        {
                throw new Exception("Person does not exist.");
        }

            var launderer = new CachedAddressLaunderer(_cachedRepo, _actualLaunderer, _coordinates);

            var addressToLaunder = new Address
            {
                StreetName = SplitAddressOnNumber(empl.Adresse).ElementAt(0),
                StreetNumber = SplitAddressOnNumber(empl.Adresse).ElementAt(1),
                ZipCode = empl.PostNr ?? 0,
                Town = empl.By,
            };
            addressToLaunder = launderer.Launder(addressToLaunder);

            var launderedAddress = new PersonalAddress()
        {
                PersonId = personId,
                Type = PersonalAddressType.Home,
                StreetName = addressToLaunder.StreetName,
                StreetNumber = addressToLaunder.StreetNumber,
                ZipCode = addressToLaunder.ZipCode,
                Town = addressToLaunder.Town,
                Latitude = addressToLaunder.Latitude ?? "",
                Longitude = addressToLaunder.Longitude ?? "",
            };

            _personalAddressRepo.Insert(launderedAddress);
            _personalAddressRepo.Save();
        }
    }
}
