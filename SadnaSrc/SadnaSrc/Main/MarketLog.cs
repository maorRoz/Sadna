using System;

namespace SadnaSrc.Main
{
    public static class MarketLog
    {
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
        }

        private static bool InsertLog(string logID, string moduleName, string description)
        {
            try
            {
                _dbConnection.InsertTable("System_Log", "LogID,LogDate,ModuleName,Description",
                    new[] {"@idValue", "@dateValue", "@moduleParam", "@descriptionParam"},
                    new object[] {logID, DateTime.Now, moduleName, description});
                return true;
            }
            catch (MarketException)
            {
                return false;
            }
        }
    }
}
