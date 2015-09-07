using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using MoreLinq;
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
        private readonly IDbUpdaterDataProvider _dataProvider;
        private readonly IMailSender _mailSender;

        public UpdateService(IGenericRepository<Employment> emplRepo, IGenericRepository<OrgUnit> orgRepo, IGenericRepository<Person> personRepo, IGenericRepository<CachedAddress> cachedRepo, IGenericRepository<PersonalAddress> personalAddressRepo, IAddressLaunderer actualLaunderer, IAddressCoordinates coordinates, IDbUpdaterDataProvider dataProvider, IMailSender mailSender)
        {
            _emplRepo = emplRepo;
            _orgRepo = orgRepo;
            _personRepo = personRepo;
            _cachedRepo = cachedRepo;
            _personalAddressRepo = personalAddressRepo;
            _actualLaunderer = actualLaunderer;
            _coordinates = coordinates;
            _dataProvider = dataProvider;
            _mailSender = mailSender;
        }

        /// <summary>
        /// Splits an address represented as "StreetName StreetNumber" into a list of "StreetName" , "StreetNumber"
        /// </summary>
        /// <param name="address">String to split</param>
        /// <returns>List of StreetName and StreetNumber</returns>
        public List<String> SplitAddressOnNumber(string address)
        {
            var result = new List<string>();
            var index = address.IndexOfAny("0123456789".ToCharArray());
            if (index == -1)
            {
                result.Add(address);
            }
            else
            {
                result.Add(address.Substring(0, index - 1));
                result.Add(address.Substring(index, address.Length - index));
            }
            return result;
        }

        /// <summary>
        /// Migrate organisations from Kommune database to OS2 database.
        /// </summary>
        public void MigrateOrganisations()
        {
            var orgs = _dataProvider.GetOrganisationsAsQueryable().OrderBy(x => x.Level);

            var i = 0;
            foreach (var org in orgs)
            {
                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Migrating organisation " + i + " of " + orgs.Count() + ".");
                }

                var orgToInsert = _orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId == org.LOSOrgId);

                var workAddress = GetWorkAddress(org);
                if (workAddress == null)
                {
                    continue;
                }

                if (orgToInsert == null)
                {
                    orgToInsert = _orgRepo.Insert(new OrgUnit());
                }

                orgToInsert.Level = org.Level;
                orgToInsert.LongDescription = org.Navn;
                orgToInsert.ShortDescription = org.KortNavn;
                orgToInsert.HasAccessToFourKmRule = false;
                orgToInsert.OrgId = org.LOSOrgId;

                orgToInsert.Address = workAddress;

                if (workAddress.Id != 0)
                {
                    orgToInsert.Address = null;
                    orgToInsert.AddressId = workAddress.Id;
                }



                if (orgToInsert.Level > 0)
                {
                    orgToInsert.ParentId = _orgRepo.AsQueryable().Single(x => x.OrgId == org.ParentLosOrgId).Id;
                }
                _orgRepo.Save();
            }

            Console.WriteLine("Done migrating organisations.");
        }

        /// <summary>
        /// Migrate employees from kommune database to OS2 database.
        /// </summary>
        public void MigrateEmployees()
        {
            foreach (var person in _personRepo.AsQueryable())
            {
                person.IsActive = false;
            }
            _personRepo.Save();

            var empls = _dataProvider.GetEmployeesAsQueryable();

            var i = 0;
            var distinctEmpls = empls.DistinctBy(x => x.CPR).ToList();
            foreach (var employee in distinctEmpls)
            {
                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Migrating person " + i + " of " + distinctEmpls.Count() + ".");
                }


                var personToInsert = _personRepo.AsQueryable().FirstOrDefault(x => x.CprNumber.Equals(employee.CPR));

                if (personToInsert == null)
                {
                    personToInsert = _personRepo.Insert(new Person());
                    personToInsert.IsAdmin = false;
                    personToInsert.RecieveMail = true;
                }

                personToInsert.CprNumber = employee.CPR ?? "ikke opgivet";
                personToInsert.FirstName = employee.Fornavn ?? "ikke opgivet";
                personToInsert.LastName = employee.Efternavn ?? "ikke opgivet";
                personToInsert.Initials = employee.ADBrugerNavn ?? " ";
                personToInsert.FullName = personToInsert.FirstName + " " + personToInsert.LastName + " [" + personToInsert.Initials + "]";
                personToInsert.Mail = employee.Email ?? "";
                personToInsert.IsActive = true;
            }
            _personRepo.Save();

            /**
             * We need the person id before we can attach personal addresses
             * so we loop through the distinct employees once again and
             * look up the created persons
             */
            i = 0;
            foreach (var employee in distinctEmpls)
            {
                if (i%50 == 0)
                {
                    Console.WriteLine("Adding home address to person " + i + " out of " + distinctEmpls.Count());
                }
                i++;
                var personToInsert = _personRepo.AsQueryable().First(x => x.CprNumber == employee.CPR);
                UpdateHomeAddress(employee, personToInsert.Id);
                if (i % 500 == 0)
                {
                    _personalAddressRepo.Save();
                }
            }
            _personalAddressRepo.Save();

            //Sets all employments to end now in the case there was
            //one day where the updater did not run and the employee
            //has been removed from the latest MDM view we are working on
            //The end date will be adjusted in the next loop
            foreach (var employment in _emplRepo.AsQueryable())
            {
                employment.EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            _emplRepo.Save();

            i = 0;
            foreach (var employee in empls)
            {
                i++;
                if (i % 10 == 0)
                {
                    Console.WriteLine("Adding employment to person " + i + " of " + empls.Count());
                }
                var personToInsert = _personRepo.AsQueryable().First(x => x.CprNumber == employee.CPR);

                CreateEmployment(employee, personToInsert.Id);
                if (i%500 == 0)
                {
                    _emplRepo.Save();
                }
            }
            _personalAddressRepo.Save();
            _emplRepo.Save();

            Console.WriteLine("Done migrating employees");
            var dirtyAddressCount = _cachedRepo.AsQueryable().Count(x => x.IsDirty);
            if (dirtyAddressCount > 0)
            {
                foreach (var admin in _personRepo.AsQueryable().Where(x => x.IsAdmin))
                {
                    _mailSender.SendMail(admin.Mail, "Der er adresser der mangler at blive vasket", "Der mangler at blive vasket " + dirtyAddressCount + "adresser");
                }
            }
        }

        /// <summary>
        /// Create employment in OS2 database for person identified by personId
        /// </summary>
        /// <param name="empl"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Employment CreateEmployment(Employee empl, int personId)
        {
            if (empl.AnsaettelsesDato == null)
            {
                return null;
            }

            var orgUnit = _orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId == empl.LOSOrgId);

            if (orgUnit == null)
            {
                throw new Exception("OrgUnit does not exist.");
            }

            var employment = _emplRepo.AsQueryable().FirstOrDefault(x => x.OrgUnitId == orgUnit.Id && x.EmploymentId == empl.MaNr);

            //It is ok that we do not save after inserting untill
            //we are done as we loop over employments from the view, and 
            //two view employments will not share an employment in the db. 
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
            employment.CostCenter = empl.Omkostningssted;
            employment.EmploymentId = empl.MaNr ?? 0;

            if (empl.OphoersDato != null)
            {
                employment.EndDateTimestamp = (Int32)(((DateTime)empl.OphoersDato).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            else
            {
                employment.EndDateTimestamp = 0;
            }

            return employment;


        }

        /// <summary>
        /// Updates home address for person identified by personId.
        /// </summary>
        /// <param name="empl"></param>
        /// <param name="personId"></param>
        public void UpdateHomeAddress(Employee empl, int personId)
        {
            if (empl.Adresse == null)
            {
                return;
            }

            var person = _personRepo.AsQueryable().FirstOrDefault(x => x.Id == personId);
            if (person == null)
            {
                throw new Exception("Person does not exist.");
            }

            var launderer = new CachedAddressLaunderer(_cachedRepo, _actualLaunderer, _coordinates);

            var splitStreetAddress = SplitAddressOnNumber(empl.Adresse);

            var addressToLaunder = new Address
            {
                Description = person.FirstName + " " + person.LastName + " [" + person.Initials + "]",
                StreetName = splitStreetAddress.ElementAt(0),
                StreetNumber = splitStreetAddress.Count > 1 ? splitStreetAddress.ElementAt(1) : "1",
                ZipCode = empl.PostNr ?? 0,
                Town = empl.By ?? "",
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

            var homeAddr = _personalAddressRepo.AsQueryable().FirstOrDefault(x => x.PersonId.Equals(personId) &&
                x.Type == PersonalAddressType.Home);

            if (homeAddr == null)
            {
                _personalAddressRepo.Insert(launderedAddress);
            }
            else
            {
                homeAddr.StreetName = launderedAddress.StreetName;
                homeAddr.StreetNumber = launderedAddress.StreetNumber;
                homeAddr.ZipCode = launderedAddress.ZipCode;
                homeAddr.Town = addressToLaunder.Town;
                homeAddr.Latitude = addressToLaunder.Latitude ?? "";
                homeAddr.Longitude = addressToLaunder.Longitude ?? "";
            }
        }

        /// <summary>
        /// Gets work address wor organisation.
        /// </summary>
        /// <param name="org"></param>
        /// <returns>WorkAddress</returns>
        public WorkAddress GetWorkAddress(Organisation org)
        {
            var launderer = new CachedAddressLaunderer(_cachedRepo, _actualLaunderer, _coordinates);

            if (org.Gade == null)
            {
                return null;
            }

            var splitStreetAddress = SplitAddressOnNumber(org.Gade);

            var addressToLaunder = new Address
            {
                StreetName = splitStreetAddress.ElementAt(0),
                StreetNumber = splitStreetAddress.Count > 1 ? splitStreetAddress.ElementAt(1) : "1",
                ZipCode = org.Postnr ?? 0,
                Town = org.By,
                Description = org.Navn
            };

            addressToLaunder = launderer.Launder(addressToLaunder);

            var launderedAddress = new WorkAddress()
            {
                StreetName = addressToLaunder.StreetName,
                StreetNumber = addressToLaunder.StreetNumber,
                ZipCode = addressToLaunder.ZipCode,
                Town = addressToLaunder.Town,
                Latitude = addressToLaunder.Latitude ?? "",
                Longitude = addressToLaunder.Longitude ?? "",
                Description = org.Navn
            };

            var existingOrg = _orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId.Equals(org.LOSOrgId));

            if (existingOrg != null)
            {
                launderedAddress.Id = existingOrg.AddressId;
            }

            return launderedAddress;
        }
    }
}
