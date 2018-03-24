using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    class MarketLog
    {
        private static SQLiteConnection _dbConnection;

        public static void InsertDbConnector(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public static void Log(string moduleName, string description)
        {
            var insertRequest = "INSERT INTO System_Log (Date,ModuleName,Description) VALUES (@dateValue,@moduleParam,@descriptionParam)";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            commandDb.Parameters.AddWithValue("@dateValue", DateTime.Now);
            commandDb.Parameters.AddWithValue("@moduleParam", moduleName);
            commandDb.Parameters.AddWithValue("@descriptionParam", description);
            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to log system action in DB, returned error message :" + e.Message);
            }
        }
    }
}
