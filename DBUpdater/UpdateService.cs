﻿using System;
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

namespace DBUpdater
{
    public class UpdateService
    {

        public UpdateService()
        {

        }

        public List<String> SplitAddressOnNumber(string address)
        {
            var result = new List<string>();
            var index = address.IndexOfAny("0123456789".ToCharArray());
            result.Add(address.Substring(0, index));
            result.Add(address.Substring(index, address.Length - index));
            return result;
        }

        public IQueryable<Organisation> GetOrganisationsAsQueryable()
        {
            var result = new List<Organisation>();
            using (var sqlConnection1 = new SqlConnection("data source=706sofd01.intern.syddjurs.dk;initial catalog=MDM;persist security info=True;user id=sofdeindberetning;password=soa2ieCh>e"))
            {
                var cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM eindberetning.organisation",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection1
                };

                sqlConnection1.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new Organisation
                    {
                        LOSOrgId = reader.GetInt32(0),
                        ParentLosOrgId = SafeGetInt32(reader, 1),
                        KortNavn = SafeGetString(reader, 2),
                        Navn = SafeGetString(reader, 3),
                        Gade = SafeGetString(reader, 4),
                        Stednavn = SafeGetString(reader, 5),
                        Postnr = SafeGetInt16(reader, 6),
                        By = SafeGetString(reader, 7),
                        Omkostningssted = SafeGetInt64(reader, 8),
                        Level = reader.GetInt32(9)
                    };
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        public void MigrateOrganisations()
        {
            var orgs = GetOrganisationsAsQueryable().OrderBy(x => x.Level);

            var repo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<OrgUnit>>();

            foreach (var org in orgs)
            {
                var orgToInsert = repo.AsQueryable().FirstOrDefault(x => x.OrgId == org.LOSOrgId);

                if (orgToInsert == null)
                {
                    orgToInsert = repo.Insert(new OrgUnit());
                }

                orgToInsert.Level = org.Level;
                orgToInsert.LongDescription = org.Navn;
                orgToInsert.ShortDescription = org.KortNavn;
                orgToInsert.HasAccessToFourKmRule = false;
                orgToInsert.OrgId = org.LOSOrgId;

                if (orgToInsert.Level > 0)
                {
                    orgToInsert.ParentId = repo.AsQueryable().Single(x => x.OrgId == org.ParentLosOrgId).Id;
                }

                repo.Save();
            }
        }

        public void MigrateEmployees()
        {
            var empls = GetEmployeesAsQueryably();

            var personRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Person>>();
            var emplRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Employment>>();

            foreach (var employee in empls)
            {
                var personToInsert = personRepo.AsQueryable().FirstOrDefault(x => x.PersonId == employee.MaNr);

                if (personToInsert == null)
                {
                    personToInsert = personRepo.Insert(new Person());
                }

                personToInsert.CprNumber = employee.CPR;
                personToInsert.RecieveMail = false;
                personToInsert.FirstName = employee.Fornavn;
                personToInsert.LastName = employee.Efternavn;
                personToInsert.IsAdmin = false;
                personToInsert.Initials = employee.ADBrugerNavn ?? " ";
                personToInsert.Mail = employee.Email ?? "";
                personToInsert.PersonId = employee.MaNr ?? default(int);

                personRepo.Save();

                CreateEmployment(employee, personToInsert.Id);
                SaveHomeAddress(employee, personToInsert.Id);
            }
        }


        private Employment CreateEmployment(Employee empl, int personId)
        {
            var orgRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<OrgUnit>>();
            var emplRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Employment>>();

            var orgUnit = orgRepo.AsQueryable().FirstOrDefault(x => x.OrgId == empl.LOSOrgId);

            var employment = emplRepo.AsQueryable().FirstOrDefault(x => x.OrgUnitId == orgUnit.Id && x.PersonId == personId);

            if (employment == null)
            {
                employment = emplRepo.Insert(new Employment());
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

            emplRepo.Save();
            return employment;


        }

        private void SaveHomeAddress(Employee empl, int personId)
        {
            var launderer = new CachedAddressLaunderer(NinjectWebKernel.CreateKernel().Get<IGenericRepository<CachedAddress>>(), NinjectWebKernel.CreateKernel().Get<IAddressLaunderer>());

            var addressRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<PersonalAddress>>();
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

            addressRepo.Insert(launderedAddress);
            addressRepo.Save();
        }

        public IQueryable<Employee> GetEmployeesAsQueryably()
        {
            var result = new List<Employee>();

            using (var sqlConnection1 = new SqlConnection("data source=706sofd01.intern.syddjurs.dk;initial catalog=MDM;persist security info=True;user id=sofdeindberetning;password=soa2ieCh>e"))
            {
                var cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM eindberetning.medarbejder",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection1
                };

                sqlConnection1.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new Employee
                    {
                        MaNr = SafeGetInt32(reader, 0),
                        AnsaettelsesDato = SafeGetDate(reader, 1),
                        OphoersDato = SafeGetDate(reader, 2),
                        Fornavn = SafeGetString(reader, 3),
                        Efternavn = SafeGetString(reader, 4),
                        ADBrugerNavn = SafeGetString(reader, 5),
                        Adresse = SafeGetString(reader, 6),
                        Stednavn = SafeGetString(reader, 7),
                        PostNr = int.Parse(SafeGetString(reader, 8)),
                        By = SafeGetString(reader, 9),
                        Land = SafeGetString(reader, 10),
                        Email = SafeGetString(reader, 11),
                        CPR = SafeGetString(reader, 12),
                        LOSOrgId = SafeGetInt32(reader, 13),
                        Leder = reader.GetBoolean(14),
                        Stillingsbetegnelse = SafeGetString(reader, 15),
                        Omkostningssted = SafeGetInt64(reader, 16),
                        AnsatForhold = SafeGetString(reader, 17),
                        EkstraCiffer = SafeGetInt16(reader, 18)
                    };
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        public DateTime? SafeGetDate(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDateTime(colIndex);
            }
            return null;
        }

        public string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            return null;
        }

        public int? SafeGetInt16(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt16(colIndex);
            }
            return null;
        }

        public int? SafeGetInt32(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            return null;
        }

        public long? SafeGetInt64(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt64(colIndex);
            }
            return null;
        }
    }
}
