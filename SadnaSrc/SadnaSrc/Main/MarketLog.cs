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
        private static IMarketDB _dbConnection = MarketDB.Instance;


        public static void SetDB(IMarketDB dbConnection)
        {
            _dbConnection = dbConnection;
        }
        private static string GenerateLogID()
        {
            return ((char) random.Next(97, 123)) + "" + ((char)random.Next(97,123)) + "" + random.Next(1000, 10000);
        }
        public static void Log(string moduleName, string description)
        {
            string logId = GenerateLogID();
            while (!InsertLog(logId, moduleName, description))
            {
                logId = GenerateLogID();
            }

            publishedLogsIDs.Add(logId);
        }

        private static bool InsertLog(string logID, string moduleName, string description)
        {
            try
            {
                _dbConnection.InsertTable("System_Log", "LogID,Date,ModuleName,Description",
                    new[] {"@idValue", "@dateValue", "@moduleParam", "@descriptionParam"},
                    new object[] {logID, DateTime.Now, moduleName, description});
                return true;
            }
            catch (MarketException)
            {
                return false;
            }
        }
        public static void RemoveLogs()
        {
            foreach (var logID in publishedLogsIDs)
            {
                _dbConnection.DeleteFromTable("System_Log","LogID = '"+logID+"'");
            }
            publishedLogsIDs.Clear();
        }
    }
}
