using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIndberetningMigration.Models;

namespace EIndberetningMigration
{
    public class DataProvider
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DBUpdaterConnection"].ConnectionString;

        public IQueryable<PersonalAddress> GetPersonalAddressesAsQueryable()
        {
            var result = new List<PersonalAddress>();



            using (var sqlConnection1 = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand
                {
                    // CommandText = "SELECT * FROM information_schema.tables",
                    CommandText = "select Description, StreetName, StreetNr, ZipCode, Town, CprNr from Koerselsindberetning.dbo.PersonalAddresses left join Koerselsindberetning.dbo.Profiles as p on Profile_Id = p.Id",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection1
                };

                sqlConnection1.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new PersonalAddress
                    {
                        Description = SafeGetString(reader, 0),
                        StreetName = SafeGetString(reader, 1),
                        StreetNumber = SafeGetString(reader, 2),
                        ZipCode = Convert.ToInt32(SafeGetString(reader, 3)),
                        Town = SafeGetString(reader, 4),
                        CprNumber = SafeGetString(reader, 5)
                    };
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        private string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            return null;
        }
    }
}
