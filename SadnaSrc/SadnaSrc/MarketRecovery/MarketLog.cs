using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace SadnaSrc.Main
{
    public static class MarketLog
    {
        private static readonly Random random = new Random();
        private static IMarketBackUpDB _dbConnection = MarketBackUpDB.Instance;


        public static void SetDB(IMarketBackUpDB dbConnection)
        {
            _dbConnection = dbConnection;
        }
        private static string GenerateLogID()
        {
            return ((char) random.Next(97, 123)) + "" + ((char)random.Next(97,123)) + "" + random.Next(1000, 10000);
        }
        public static void Log(string moduleName, string description)
        {
            var allLogIds = GetAllLogsIds();
            var logId = GenerateLogID();
            while (allLogIds.Contains(logId))
            {
                logId = GenerateLogID();
            }

            InsertLog(logId,moduleName,description);
        }

        private static void InsertLog(string logID, string moduleName, string description)
        {
            _dbConnection.InsertTable("System_Log", "LogID,LogDate,ModuleName,Description",
                new[] {"@idValue", "@dateValue", "@moduleParam", "@descriptionParam"},
                new object[] {logID, DateTime.Now, moduleName, description});

        }

        private static string[] GetAllLogsIds()
        {
            var ids = new List<string>();
            using (var dbReader = _dbConnection.SelectFromTable("System_Log", "LogID"))
            {
                while (dbReader != null && dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        ids.Add(dbReader.GetString(0));
                    }
                }
            }

            return ids.ToArray();
        }

    }
}
