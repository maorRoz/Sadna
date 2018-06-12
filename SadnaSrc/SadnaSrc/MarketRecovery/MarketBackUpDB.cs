using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

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
            var pathsDirectories = Directory.GetCurrentDirectory().Split(new[]{"\\"},StringSplitOptions.RemoveEmptyEntries);
            var newPathItems = new List<string>();
            int i = 0;
            while (pathsDirectories[i] != "SadnaSrc")
            {
                newPathItems.Add(pathsDirectories[i]);
                i++;
            }
            newPathItems.Add(pathsDirectories[i]);
            var newPath = string.Join("\\", newPathItems.ToArray()) +"\\MarketBackup.db";
            _dbConnection = new SQLiteConnection("FullUri=file:"+newPath);
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
                                    [LogID]         TEXT,
                                    [LogDate]       DATE,
                                    [ModuleName]    TEXT,
                                    [Description]   TEXT,
                                    PRIMARY KEY([LogID])
                                    )";
        }

        private static string CreateSystemErrorsTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [System_Errors] ( 
                                    [ErrorID]       TEXT,
                                    [ModuleName]    TEXT,
                                    [Description]   TEXT,
                                    PRIMARY KEY([ErrorID])
                                    )";
        }

        public void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values)
        {
            var insertRequest = "INSERT INTO " + table + " (" + tableColumns + ") VALUES (" + string.Join(",", valuesNames)
                                + ")";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            for (int i = 0; i < values.Length; i++)
            {
                commandDb.Parameters.AddWithValue(valuesNames[i], values[i]);
            }

            commandDb.ExecuteNonQuery();
        }

        public SQLiteDataReader SelectFromTable(string table, string toSelect)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table;
            return new SQLiteCommand(selectRequest, _dbConnection).ExecuteReader();
        }
    }
}
