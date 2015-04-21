using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DataAccess;

namespace DBUpdater
{
    class Program
    {
        static void Main(string[] args)
        {

            using (SqlConnection sqlConnection1 = new SqlConnection("data source=706sofd01.intern.syddjurs.dk;initial catalog=MDM;persist security info=True;user id=sofdeindberetning;password=XXXXXXXX")) { 
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "SELECT * FROM eindberetning.medarbejder";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();

                foreach (var x in reader)
                {

                }
            }
        }
            
    }
}
