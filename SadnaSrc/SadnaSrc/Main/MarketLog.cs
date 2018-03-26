using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public class MarketLog
    {
        private static SQLiteConnection _dbConnection;
        private static List<string> publishedLogsIDs;
        public static void InsertDbConnector(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
            publishedLogsIDs = new List<string>();
        }

        private static string GenerateLogID()
        {
            var random = new Random();
            return ((char) random.Next(97, 123)) + "" + ((char)random.Next(97,123)) + "" + random.Next(1000, 10000);
        }
        public static void Log(string moduleName, string description)
        {
            var insertRequest = "INSERT INTO System_Log (LogID,Date,ModuleName,Description) VALUES (@idValue,@dateValue,@moduleParam,@descriptionParam)";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            string logID = GenerateLogID();
            commandDb.Parameters.AddWithValue("@idValue", logID);
            commandDb.Parameters.AddWithValue("@dateValue", DateTime.Now);
            commandDb.Parameters.AddWithValue("@moduleParam", moduleName);
            commandDb.Parameters.AddWithValue("@descriptionParam", description);
            try
            {
                commandDb.ExecuteNonQuery();
                publishedLogsIDs.Add(logID);


            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to communicate with the DB, returned error message :" + e.Message);
            }
        }

        public static string[] GetLogs()
        {
            return publishedLogsIDs.ToArray();
        }
        public static void RemoveLogs()
        {
            foreach (string logID in publishedLogsIDs)
            {
                var deleteRequest = "DELETE FROM System_Log WHERE LogID LIKE '%" + logID +"%'";
                var commandDb = new SQLiteCommand(deleteRequest, _dbConnection);
                try
                {
                    commandDb.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        "Problem occured in the attempt to communicate with the DB, returned error message :" +
                        e.Message);
                    break;
                }
            }
            publishedLogsIDs.Clear();
        }
    }
}
