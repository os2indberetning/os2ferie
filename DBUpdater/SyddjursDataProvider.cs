﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUpdater.Models;

namespace DBUpdater
{
    public class SyddjursDataProvider : IDbUpdaterDataProvider
    {
        public IQueryable<Employee> GetEmployeesAsQueryable()
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