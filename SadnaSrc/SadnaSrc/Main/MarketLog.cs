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
        private static readonly List<string> publishedLogsIDs = new List<string>();
        private static readonly Random random = new Random();

        private static string GenerateLogID()
        {
            return ((char) random.Next(97, 123)) + "" + ((char)random.Next(97,123)) + "" + random.Next(1000, 10000);
        }
        public static void Log(string moduleName, string description)
        {
            string logID = GenerateLogID();
            InsertLog(logID,moduleName,description);
            publishedLogsIDs.Add(logID);
        }

        private static void InsertLog(string logID, string moduleName, string description)
        {
            var dbConnection = MarketDB.Instance;
            dbConnection.InsertTable("System_Log", "LogID,Date,ModuleName,Description",
                new[] { "@idValue", "@dateValue", "@moduleParam", "@descriptionParam" },
                new object[] { logID, DateTime.Now, moduleName, description });
        }
        public static void RemoveLogs()
        {
            var dbConnection = MarketDB.Instance;
            foreach (var logID in publishedLogsIDs)
            {
                dbConnection.DeleteFromTable("System_Log","LogID = '"+logID+"'");
            }
            publishedLogsIDs.Clear();
        }
    }
}
