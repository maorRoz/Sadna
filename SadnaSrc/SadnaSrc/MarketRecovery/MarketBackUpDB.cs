using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketRecovery
{
    class MarketBackUpDB : IMarketBackUpDB
    {
        private static MarketBackUpDB _instance;

        public static MarketBackUpDB Instance => _instance ?? (_instance = new MarketBackUpDB());
        private SQLiteConnection _dbConnection;
        private MarketBackUpDB()
        {
            InitiateDb();
            CreateTables();
        }
        private void InitiateDb()
        {

            _dbConnection = new SQLiteConnection();
            _dbConnection.Open();
        }
        private void CreateTables()
        {
            string[] createTableStrings =
            {
                CreateSystemLogTable(),
                CreateSystemErrorsTable(),
            };

            for (var i = 0; i < createTableStrings.Length; i++)
            {

                var createTableCommand = new SQLiteCommand(createTableStrings[i], _dbConnection);
                createTableCommand.ExecuteNonQuery();
            }

        }
        public void CleanByForce()
        {
            var tableNames = new[]
            {
                "System_Log",
                "System_Errors",
            };

            for (int i = 0; i < tableNames.Length; i++)
            {
                var deleateTableCommand = new SQLiteCommand("Delete FROM " + tableNames[i],
                    _dbConnection);
                deleateTableCommand.ExecuteNonQuery();
            }
        }

        private static string CreateSystemLogTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [System_Log] (
                                    [LogID]         VARCHAR(256),
                                    [LogDate]       DATETIME,
                                    [ModuleName]    VARCHAR(256),
                                    [Description]   VARCHAR(256),
                                    PRIMARY KEY([LogID])
                                    )";
        }

        private static string CreateSystemErrorsTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [System_Errors] ( 
                                    [ErrorID]       VARCHAR(256),
                                    [ModuleName]    VARCHAR(256),
                                    [Description]   VARCHAR(256),
                                    PRIMARY KEY([ErrorID])
                                    )";
        }

        public void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values)
        {
            throw new NotImplementedException();
        }

        public SQLiteDataReader SelectFromTable(string table, string toSelect)
        {
            throw new NotImplementedException();
        }
    }
}
