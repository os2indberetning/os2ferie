using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressHistoryMigration.Models;

namespace AddressHistoryMigration
{
    public class DataProvider
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DBUpdaterConnection"].ConnectionString;

        /// <summary>
        /// Reads employee AddressHistories from Kommune database and returns them asqueryable.
        /// </summary>
        /// <returns></returns>
        public IQueryable<AddressHistory> GetAddressHistoriesAsQueryable()
        {
            var result = new List<AddressHistory>();

           

            using (var sqlConnection1 = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandText = "SELECT * FROM eindberetning.Medarbejder_Temp",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection1
                };

                sqlConnection1.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new AddressHistory
                    {
                        AktivFra = SafeGetDate(reader,0) ?? DateTime.Now,
                        AktivTil = SafeGetDate(reader, 1) ?? DateTime.Now,
                        MaNr = SafeGetInt32(reader, 2) ?? 0,
                        Navn = SafeGetString(reader, 3),
                        HjemmeAdresse = SafeGetString(reader, 4),
                        HjemmePostNr = Convert.ToInt32(SafeGetString(reader, 5)),
                        HjemmeBy = SafeGetString(reader, 6),
                        HjemmeLand = SafeGetString(reader, 7),
                        ArbejdsAdresse =  SafeGetString(reader,8),
                        ArbejdsPostNr = SafeGetInt16(reader, 9) ?? 0,
                        ArbejdsBy = SafeGetString(reader, 10),
                    };
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        private DateTime? SafeGetDate(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDateTime(colIndex);
            }
            return null;
        }

        private string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            return null;
        }

        private int? SafeGetInt16(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt16(colIndex);
            }
            return null;
        }

        private int? SafeGetInt32(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            return null;
        }

        private long? SafeGetInt64(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt64(colIndex);
            }
            return null;
        }
    }
}
