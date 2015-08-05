using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using EIndberetningMigration.Models;

namespace EIndberetningMigration
{
    public class DataProvider
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["DBUpdaterConnection"].ConnectionString;

        public static IQueryable<EIndPersonalAddress> GetPersonalAddressesAsQueryable()
        {
            var result = new List<EIndPersonalAddress>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandText = "select Description, StreetName, StreetNr, ZipCode, Town, CprNr from Koerselsindberetning.dbo.PersonalAddresses left join Koerselsindberetning.dbo.Profiles as p on Profile_Id = p.Id",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                };

                sqlConnection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new EIndPersonalAddress
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

        public static IQueryable<EindDriveReport> GetDriveReports()
        {
            var result = new List<EindDriveReport>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandText = @"select report.Id, Date, Purpose, report.VehicleRegistrationNr, EmploymentId, AmountToReimburse, ManualEntryRemark, IsExtraDistance, ReimburseableDistance, ApproverEmploymentId, ApprovalDate, Rate_Id, CreationDate, Approved, Rejected, Reimbursed, ReimbursementDate, CprNr, RouteDescription 
                                    from Koerselsindberetning.dbo.DriveReports as report
                                    LEFT JOIN Koerselsindberetning.dbo.Status as status
                                    ON report.Id = status.DriveReport_Id
									JOIN Koerselsindberetning.dbo.Profiles as profile
									ON profile.Id = report.Profile_Id
									JOIN Koerselsindberetning.dbo.Routes as routes
									ON routes.DriveReport_Id = report.id
									ORDER BY CprNr, EmploymentId",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                };

                sqlConnection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new EindDriveReport
                    {
                        Id = reader.GetInt32(0),
                        Date = reader.GetDateTime(1),
                        Purpose = SafeGetString(reader, 2),
                        RegistrationNumber = SafeGetString(reader, 3),
                        EmploymentID = SafeGetIntFromString(reader, 4),
                        AmmountToReimburse = SafeGetDoubleFromString(reader ,5),
                        ManualEntryRemark = SafeGetString(reader, 6),
                        IsExtraDistance = SafeGetBool(reader, 7),
                        ReimbursableDistance = SafeGetDoubleFromString(reader, 8),
                        ApproverEmploymentID = SafeGetIntFromString(reader, 9),
                        ApprovalDate = SafeGetDateTime(reader, 10),
                        RateID = reader.GetInt32(11),
                        CreationDate = reader.GetDateTime(12),
                        Approved = SafeGetBool(reader, 13),
                        Rejected = SafeGetBool(reader, 14),
                        Reimbursed = SafeGetBool(reader, 15),
                        ReimbursementDate = SafeGetDateTime(reader, 16),
                        CPR = SafeGetString(reader, 17),
                        RouteDescription = SafeGetString(reader ,18)
                    };

                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        public static ICollection<DriveReportPoint> GetDriveReportPoints(int reportId)
        {
            var result = new List<DriveReportPoint>();

            var commandText = @"SELECT StreetName, StreetNr, Zipcode, Town   
	                            FROM Koerselsindberetning.dbo.DriveReports as reports 
			                        INNER JOIN Koerselsindberetning.dbo.Routes as routes 
			                        ON reports.Id = routes.DriveReport_Id 
			                        JOIN Koerselsindberetning.dbo.RouteAddress as ra
			                        ON routes.id = ra.routes_id
			                        JOIN Koerselsindberetning.dbo.Addresses as addresses
			                        ON ra.addresses_id = addresses.Id
                                WHERE reports.Id = @ID
	                            ORDER BY addresses.Id";
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand(commandText, sqlConnection);
                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = reportId;
                
                sqlConnection.Open();

                var reader = cmd.ExecuteReader();

                var i = 0;
                while (reader.Read())
                {
                    result.Add(new DriveReportPoint
                    {
                        Description = "Migrated point",
                        Latitude = "",
                        Longitude = "",
                        StreetName = SafeGetString(reader, 0),
                        StreetNumber = SafeGetString(reader, 1),
                        ZipCode = SafeGetIntFromString(reader, 2),
                        Town = SafeGetString(reader, 3)
                    });
                }
            }

            for (var i = 0; i < result.Count; i++)
            {
                if (i > 0)
                {
                    result[i].PreviousPoint = result[i-1];
                }
                if (i < result.Count - 1)
                {
                    result[i].NextPoint = result[i+1];
                }
            }

            return result;
        }

        public static IQueryable<EindRate> GetRates()
        {
            var result = new List<EindRate>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandText = @"select Id, KmRate, TFCode from Koerselsindberetning.dbo.Rates",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                };

                sqlConnection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new EindRate
                    {
                        Id = reader.GetInt32(0),
                        KmRate = (float)SafeGetDoubleFromString(reader, 1),
                        TfCode = SafeGetString(reader, 2)
                    };
                    result.Add(currentRow);
                }
            }
            return result.AsQueryable();
        }

        private static bool SafeGetBool(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetBoolean(colIndex);
            }
            return false;
        }

        private static int SafeGetIntFromString(SqlDataReader reader, int colIndex)
        {
            var str = SafeGetString(reader, colIndex);
            if (string.IsNullOrEmpty(str) || str == "null" )//Yes the db actually does have "null" as a value some places...
            {
                return 0;
            }
            return Convert.ToInt32(str);
        }

        private static Double SafeGetDoubleFromString(SqlDataReader reader, int colIndex)
        {
            var str = SafeGetString(reader, colIndex);
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            str = str.Replace(".", "");
            str = str.Replace(',', '.');
            if (str.StartsWith("."))
            {
                str = "0" + str;
            }
            return Convert.ToDouble(str);
        }

        private static DateTime? SafeGetDateTime(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDateTime(colIndex);
            }
            return null;
        }

        private static string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            return null;
        }
    }
}
