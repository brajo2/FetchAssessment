using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchAssessment
{
    class Program
    {
        static void Main(string[] args)
        {
            // stuff
            string user = ConfigurationManager.AppSettings["user"];
            string password = ConfigurationManager.AppSettings["password"];
            string host = ConfigurationManager.AppSettings["host"];
            string database = ConfigurationManager.AppSettings["database"];

            writeToTbl(user, password, host, database, "brand", @"C:\Users\josia\source\repos\FetchAssessment\FetchAssessment\brands.json");
            writeToTbl(user, password, host, database, "receipts", @"C:\Users\josia\source\repos\FetchAssessment\FetchAssessment\receipts.json");
            writeToTbl(user, password, host, database, "users", @"C:\Users\josia\source\repos\FetchAssessment\FetchAssessment\users.json");

        }


        static void writeToTbl(string user, string password, string host, string database, string sqlTable, string filePath)
        {
            var cs = $"Host={host};Username={user};Password={password};Database={database}";

            using var con = new NpgsqlConnection(cs);

            con.Open();


            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    string[] params_ = { };
                    while (!reader.EndOfStream)
                    {
                        string sql = $"INSERT INTO \"public\".{sqlTable} (info) VALUES (@info);";

                        using var cmd = new NpgsqlCommand(sql, con);

                        cmd.CommandText = sql;

                        var line = reader.ReadLine();

                        // parameter
                        var param = cmd.CreateParameter();
                        param.ParameterName = "info";
                        param.Value = line;
                        param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
                        cmd.Parameters.Add(param);

                        // insert statement send
                        cmd.ExecuteNonQuery();
                    }
                }

                con.Close();
            }

        }
    }



}
