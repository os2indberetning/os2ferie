using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using DBUpdater.Models;
using Infrastructure.AddressServices.Interfaces;

namespace DBUpdater
{
    public class UpdateService
    {
        
        public UpdateService()
        {
      
        }

        public IQueryable<Organisation> GetOrganisationsAsQueryable()
        {
            var result = new List<Organisation>();
            using (var sqlConnection1 =new SqlConnection("data source=706sofd01.intern.syddjurs.dk;initial catalog=MDM;persist security info=True;user id=sofdeindberetning;password=soa2ieCh>e"))
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
                    var currentRow = new Organisation();
                    currentRow.LOSOrgId = SafeGetInt32(reader, 0);
                    currentRow.ParentLosOrgId = SafeGetInt32(reader, 1);
                    currentRow.KortNavn = SafeGetString(reader, 2);
                    currentRow.Navn = SafeGetString(reader, 3);
                    currentRow.Gade = SafeGetString(reader, 4);
                    currentRow.Stednavn = SafeGetString(reader, 5);
                    currentRow.Postnr = SafeGetInt16(reader, 6);
                    currentRow.By = SafeGetString(reader, 7);
                    currentRow.Omkostningssted = SafeGetInt64(reader, 8);
                    currentRow.Level = SafeGetInt32(reader, 9);
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
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
                    var currentRow = new Employee();
                    currentRow.MaNr = SafeGetInt32(reader,0);
                    currentRow.AnsaettelsesDato = SafeGetDate(reader,1);
                    currentRow.OphoersDato = SafeGetDate(reader,2);
                    currentRow.Fornavn = SafeGetString(reader,3);
                    currentRow.Efternavn = SafeGetString(reader,4);
                    currentRow.ADBrugerNavn = SafeGetString(reader,5);
                    currentRow.Adresse = SafeGetString(reader,6);
                    currentRow.Stednavn = SafeGetString(reader,7);
                    currentRow.PostNr = int.Parse(SafeGetString(reader,8));
                    currentRow.By = SafeGetString(reader,9);
                    currentRow.Land = SafeGetString(reader,10);
                    currentRow.Email = SafeGetString(reader,11);
                    currentRow.CPR = SafeGetString(reader,12);
                    currentRow.LOSOrgId = SafeGetInt32(reader,13);
                    currentRow.Leder = reader.GetBoolean(14);
                    currentRow.Stillingsbetegnelse = SafeGetString(reader,15);
                    currentRow.Omkostningssted = SafeGetInt64(reader,16);
                    currentRow.AnsatForhold = SafeGetString(reader,17);
                    currentRow.EkstraCiffer = SafeGetInt16(reader,18);
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
