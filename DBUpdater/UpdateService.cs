using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;
using MoreLinq;
using IAddressCoordinates = Core.DomainServices.IAddressCoordinates;
using Core.ApplicationServices.Interfaces;
using VacationBalance = Core.DomainModel.VacationBalance;

namespace DBUpdater
{
    public class UpdateService
    {
        private readonly IGenericRepository<Employment> _emplRepo;
        private readonly IGenericRepository<OrgUnit> _orgRepo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly IGenericRepository<CachedAddress> _cachedRepo;
        private readonly IGenericRepository<PersonalAddress> _personalAddressRepo;
        private readonly IGenericRepository<Substitute> _subRepo;
        private readonly IAddressLaunderer _actualLaunderer;
        private readonly IAddressCoordinates _coordinates;
        private readonly IDbUpdaterDataProvider _dataProvider;
        private readonly IMailSender _mailSender;
        private readonly IAddressHistoryService _historyService;

        private readonly IGenericRepository<Report> _reportRepo;
        private readonly IReportService<Report> _reportService;

        private readonly IGenericRepository<VacationBalance> _vacationBalanceRepo;
        private readonly ISubstituteService _subService;

        public UpdateService(IGenericRepository<Employment> emplRepo,
            IGenericRepository<OrgUnit> orgRepo,
            IGenericRepository<Person> personRepo,
            IGenericRepository<CachedAddress> cachedRepo,
            IGenericRepository<PersonalAddress> personalAddressRepo,
            IAddressLaunderer actualLaunderer,
            IAddressCoordinates coordinates,
            IDbUpdaterDataProvider dataProvider,
            IMailSender mailSender,
            IAddressHistoryService historyService,
            IGenericRepository<Report> reportRepo,
            IReportService<Report> reportService,
            ISubstituteService subService,
            IGenericRepository<Substitute> subRepo,
            IGenericRepository<VacationBalance> vacationBalanceRepo)
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
            _historyService = historyService;
            _reportRepo = reportRepo;
            _reportService = reportService;
            _subService = subService;
            _subRepo = subRepo;
            _vacationBalanceRepo = vacationBalanceRepo;
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
                    orgToInsert.HasAccessToFourKmRule = false;
                    orgToInsert.HasAccessToVacation = false;
                    orgToInsert.DefaultKilometerAllowance = KilometerAllowance.Calculated;
                }

                orgToInsert.Level = org.Level;
                orgToInsert.LongDescription = org.Navn;
                orgToInsert.ShortDescription = org.KortNavn;
                orgToInsert.OrgId = org.LOSOrgId;

                var addressChanged = false;

                if(workAddress != orgToInsert.Address)
                {
                    addressChanged = true;
                    orgToInsert.Address = workAddress;
                }



                if (orgToInsert.Level > 0)
                {
                    orgToInsert.ParentId = _orgRepo.AsQueryable().Single(x => x.OrgId == org.ParentLosOrgId).Id;
                }
                _orgRepo.Save();

                if (addressChanged)
                {
                    workAddress.OrgUnitId = orgToInsert.Id;
                }

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
                foreach (var admin in _personRepo.AsQueryable().Where(x => x.IsAdmin && x.IsActive))
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
                employment.EndDateTimestamp = (Int32)(((DateTime)empl.OphoersDato).Subtract(new DateTime(1970, 1, 1)).Add(new TimeSpan(1,0,0,0))).TotalSeconds;
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
                Description = addressToLaunder.Description
            };

            var homeAddr = _personalAddressRepo.AsQueryable().FirstOrDefault(x => x.PersonId.Equals(personId) &&
                x.Type == PersonalAddressType.Home);

            if (homeAddr == null)
            {
                _personalAddressRepo.Insert(launderedAddress);
            }
            else
            {
                if (homeAddr != launderedAddress)
                {
                    // Address has changed
                    // Change type of current (The one about to be changed) home address to OldHome.
                    // Is done in loop because there was an error that created one or more home addresses for the same person.
                    // This will make sure all home addresses are set to old if more than one exists.
                    foreach (var addr in _personalAddressRepo.AsQueryable().Where(x => x.PersonId.Equals(personId) && x.Type == PersonalAddressType.Home).ToList())
                    {
                        addr.Type = PersonalAddressType.OldHome;;
                    }

                    // Update actual current home address.
                    _personalAddressRepo.Insert(launderedAddress);
                    _personalAddressRepo.Save();
                }
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

            // If the address hasn't changed then set the Id to be the same as the existing one.
            // That way a new address won't be created in the database.
            // If the address is not the same as the existing one,
            // Then the Id will be 0, and a new address will be created in the database.
            if (existingOrg != null
                && existingOrg.Address != null
                && existingOrg.Address.StreetName == launderedAddress.StreetName
                && existingOrg.Address.StreetNumber == launderedAddress.StreetNumber
                && existingOrg.Address.ZipCode == launderedAddress.ZipCode
                && existingOrg.Address.Town == launderedAddress.Town
                && existingOrg.Address.Latitude == launderedAddress.Latitude
                && existingOrg.Address.Longitude == launderedAddress.Longitude
                && existingOrg.Address.Description == launderedAddress.Description)
            {
                launderedAddress.Id = existingOrg.AddressId;
            }
            else
            {
                var a = 2;
            }

            return launderedAddress;
        }

        public void UpdateLeadersOnAllReports()
        {
            Console.WriteLine("Updating leaders on reports:");
            var reports = _reportRepo.AsQueryable().Where(x => x.Employment.OrgUnit.Level > 1).ToList();
            var i = 0;
            var max = reports.Count;
            foreach (var report in reports)
            {
                if (i % 100 == 0)
                {
                    Console.WriteLine("at " + i + " of " + max);
                }
                i++;
                report.ResponsibleLeaderId = _reportService.GetResponsibleLeaderForReport(report).Id;
                report.ActualLeaderId = _reportService.GetActualLeaderForReport(report).Id;
                if (i % 1000 == 0)
                {
                    Console.WriteLine("Saving to database");
                    _reportRepo.Save();
                }
            }
            Console.WriteLine("Saving to database");
            _reportRepo.Save();
        }

        /// <summary>
        /// Updates ResponsibleLeader on all reports that had a substitute which expired yesterday or became active today.
        /// </summary>
        public void UpdateLeadersOnExpiredOrActivatedSubstitutes()
        {
            // TODO Find something more generic for updating drive and vacation reports.
            var yesterdayTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds;
            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var endOfDayStamp = _subService.GetEndOfDayTimestamp(yesterdayTimestamp);
            var startOfDayStamp = _subService.GetStartOfDayTimestamp(currentTimestamp);

            var affectedSubstitutes = _subRepo.AsQueryable().Where(s => (s.EndDateTimestamp == endOfDayStamp) || (s.StartDateTimestamp == startOfDayStamp)).ToList();
            Console.WriteLine(affectedSubstitutes.Count() + " substitutes have expired or become active. Updating affected reports.");
            foreach(var sub in affectedSubstitutes)
            {
                _subService.UpdateReportsAffectedBySubstitute(sub);
            }
        }

        public void AddLeadersToReportsThatHaveNone()
        {
            // Fail-safe as some reports for unknown reasons have not had a leader attached
            Console.WriteLine("Adding leaders to drive reports that have none");
            var i = 0;
            var reports = _reportRepo.AsQueryable().Where(r => r.ResponsibleLeader == null || r.ActualLeader == null).ToList();
            foreach (var report in reports)
            {
                i++;
                Console.WriteLine("Adding leaders to  report " + i + " of " + reports.Count);
                report.ResponsibleLeaderId = _reportService.GetResponsibleLeaderForReport(report).Id;
                report.ActualLeaderId = _reportService.GetActualLeaderForReport(report).Id;
                if (i % 100 == 0)
                {
                    Console.WriteLine("Saving to database");
                    _reportRepo.Save();
                }
            }
            _reportRepo.Save();
        }

        public void UpdateVacationBalance()
        {

            Console.WriteLine("Adding vacation balance to employers");

            var i = 0;

            var balances = _dataProvider.GetVacationBalanceAsQueryable().ToList();


            foreach (var balance in balances)
            {
                i++;
                Console.WriteLine("Vacation balance " + i + " of " + balances.Count);

                var person = _personRepo.AsQueryable().FirstOrDefault(x => x.CprNumber.Equals(balance.SocialSecurityNumber));

                // The person is not stored in our database, so abort
                // In theory, this should never happen, since we sync people before vacation
                if (person == null) continue;

                int vacationYear;

                if (!int.TryParse(balance.VacationEarnedYear, out vacationYear)) continue;

                // The year the vacation is earned is stored in the database
                // But we're interested in the year the vacation is held, which is a year later
                vacationYear += 1;

                int employmentRelationshipNumber;

                if (!int.TryParse(balance.EmploymentRelationshipNumber, out employmentRelationshipNumber)) continue;

                var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var employment = _emplRepo.AsQueryable().FirstOrDefault(x => x.PersonId == person.Id && x.ExtraNumber == employmentRelationshipNumber && (x.EndDateTimestamp == 0 || x.EndDateTimestamp >= currentTimestamp));

                // The person's employment is not stored in our database, so abort
                // Also not likely to happen, but better safe than sorry.
                if (employment == null) continue;

                var vacation = _vacationBalanceRepo.AsQueryable().FirstOrDefault(x => x.PersonId == person.Id && x.EmploymentId == employment.Id && x.Year == vacationYear);

                var isNewBalance = vacation == null;

                if (isNewBalance)
                {
                    vacation = new VacationBalance
                    {
                        PersonId = person.Id,
                        EmploymentId = employment.Id,
                        Year = vacationYear
                    };

                    _vacationBalanceRepo.Insert(vacation);
                }

                vacation.FreeVacationHours = balance.FreeVacationHoursTotalDec ?? 0;
                vacation.TransferredHours = balance.TransferredVacationHoursDec ?? 0;
                vacation.VacationHours = balance.VacationHoursWithPayDec ?? 0;

                var updated = balance.UpdateDate ?? new DateTime();

                vacation.UpdatedAt = Convert.ToInt64((updated - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

                if (i % 100 != 0) continue;
                Console.WriteLine("Saving to database");
                _vacationBalanceRepo.Save();
            }

            _vacationBalanceRepo.Save();
        }

    }
}
