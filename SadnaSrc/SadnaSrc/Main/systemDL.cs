using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public class SystemDL
    {
        private static SQLiteConnection _dbConnection;

        protected SystemDL()
        {
        }
        public static void InsertDbConnector(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
            CreateTables();
        }

        public static void CreateTables()
        {
            string[] createTableStrings = {
                CreateSystemLogTable(),
                CreateSystemErrorsTable(),
                CreateUserTable(),
                CreateStoreTable(),
                CreateUserStatePolicyTable(),
                CreateUserStorePolicyTable(),  // should improve this one
                CreateCartItemTable(),
                CreatePurchaseHistoryTable(),
                //createTableStrings.Add(CreateProductTable());
                //createTableStrings.Add(CreateSaleTable());
                //createTableStrings.Add(CreateDiscountTable());
                CreateOrderTable(),
                CreateOrderItemTable()
            };

            for (var i = 0; i < createTableStrings.Length; i++)
            {
                var createTableCommand = new SQLiteCommand(createTableStrings[i], _dbConnection);
                createTableCommand.ExecuteNonQuery();
            }

            //TODO : delete this when The Right UseCase is implemented (Except the SystemAdmin since he is mandatory by constraint)
            string[] thingsToInsertByForce = 
            {
                "INSERT INTO Store (Name,Address,Status) VALUES ('X','Here 4','Active')",
                "INSERT INTO Store (Name,Address,Status) VALUES ('Y','Here 4','Active')",
                "INSERT INTO Store (Name,Address,Status) VALUES ('M','Here 4','Active')",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'X'",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'Y'",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'M'",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (1,'Arik1','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (2,'Arik2','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (3,'Arik3','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'SystemAdmin')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (2,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (3,'RegisteredUser')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'Y','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'M','StoreOwner')",
            };
            for (int i = 0; i < thingsToInsertByForce.Length; i++)
            {
                var insertStoreCommand = new SQLiteCommand(thingsToInsertByForce[i], _dbConnection);
                try
                {
                    insertStoreCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    //dont care
                }
            }
        }

        private static string CreateSystemLogTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [System_Log] (
                                    [LogID]         TEXT,
                                    [DATE]          TEXT,
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

        private static string CreateUserTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [User] (
                                    [SystemID]      INTEGER,
                                    [Name]          TEXT,
                                    [Address]       TEXT,
                                    [Password]      TEXT,
                                    PRIMARY KEY([SystemID])
                                    )";
        }

        private static string CreateStoreTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [Store] (
                                    [Name]          TEXT,
                                    [Address]       TEXT,
                                    [Status]        TEXT,
                                    PRIMARY KEY([Name])
                                    )";
        }

        private static string CreateUserStatePolicyTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [StatePolicy] (
                                    [SystemID]      INTEGER,
                                    [State]         TEXT,
                                    FOREIGN KEY([SystemID])     REFERENCES [USER]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[State])
                                    )";
        }

        private static string CreateUserStorePolicyTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [StoreManagerPolicy] (
                                    [SystemID]      INTEGER,
                                    [Store]         TEXT,
                                    [Action]        TEXT,
                                    FOREIGN KEY([SystemID])     REFERENCES [USER]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[Store],[Action])
                                    )";
        }

        private static string CreateCartItemTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [CartItem] (
                                    [SystemID]      INTEGER,
                                    [Name]          TEXT,
                                    [Store]         TEXT,
                                    [Quantity]      TEXT,
                                    [FinalPrice]    TEXT,
                                    [SaleType]      TEXT,
                                    FOREIGN KEY([SystemID])     REFERENCES [USER]([SystemID]) ON DELETE CASCADE,
                                    FOREIGN KEY([Store])        REFERENCES [Store]([Name]) ,
                                    PRIMARY KEY([SystemID],[Name],[Store])
                                    )";
        }

        private static string CreatePurchaseHistoryTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [PurchaseHistory] (
                                    [UserName]      TEXT,
                                    [Product]       TEXT,
                                    [Store]         TEXT,
                                    [SaleType]      TEXT,
                                    [Date]          TEXT,
                                    PRIMARY KEY([UserName],[Product],[Store],[SaleType],[Date])
                                    )";
            //TODO: add this to the string :   FOREIGN KEY([UserName])        REFERENCES [Product]([Name]), 
            //TODO: add this to the string :   FOREIGN KEY([Product])        REFERENCES [Product]([Name]), 
            //TODO: add this to the string :   FOREIGN KEY([Store])        REFERENCES [Product]([Name]), 
            //TODO: add this to the string :   FOREIGN KEY([SaleType])        REFERENCES something of sale table...? 
        }

        private static string CreateProductTable()
        {
            throw new NotImplementedException();
        }

        private static string CreateSaleTable()
        {
            throw new NotImplementedException();
        }

        private static string CreateDiscountTable()
        {
            throw new NotImplementedException();
        }

        private static string CreateOrderTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [Orders] (
                                    [OrderID]           INTEGER,
                                    [UserName]          TEXT,
                                    [ShippingAddress]   TEXT,
                                    [TotalPrice]        REAL,
                                    [Date]              TEXT,
                                    PRIMARY KEY([OrderID])
                                    )";
        }

        private static string CreateOrderItemTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [OrderItem] (
                                    [OrderID]       INTEGER,
                                    [Store]         TEXT,
                                    [Name]          TEXT,
                                    [Price]         REAL,
                                    [Quantity]      INTEGER,
                                    FOREIGN KEY([OrderID])      REFERENCES [Orders]([OrderID]) ON DELETE CASCADE,
                                    
                                    PRIMARY KEY([OrderID],[Store],[Name])
                                    )";
            //TODO: add this to the string :   FOREIGN KEY([Name])        REFERENCES [Product]([Name]), 
            //TODO: and this                   FOREIGN KEY([Store])        REFERENCES [Store]([Name]),
        }
        protected void InsertTable(string table,string tableColumns,string[] valuesNames,object[] values)
        {
            var insertRequest = "INSERT INTO "+table+" ("+ tableColumns + ") VALUES ("+ string.Join(",", valuesNames)
                                + ")";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            for (int i = 0; i < values.Length;i++)
            {
                commandDb.Parameters.AddWithValue(valuesNames[i], values[i]);
            }

            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new MarketException(MarketError.DbError,"Problem occured in the attempt to save system data in DB, returned error message :" +
                                  e.Message);
            }
        }

        protected SQLiteDataReader SelectFromTable(string table, string toSelect)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table;
            return new SQLiteCommand(selectRequest, _dbConnection).ExecuteReader();
        }

        protected SQLiteDataReader SelectFromTableWithCondition(string table, string toSelect, string condition)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table + " WHERE "+condition;
            return new SQLiteCommand(selectRequest, _dbConnection).ExecuteReader();
        }

        protected void UpdateTable(string table,string updateCondition,string[] columnNames, string[] valuesNames, object[] values)
        {
            string [] setString = new string[values.Length];
            for (int i = 0; i < setString.Length; i++)
            {
                setString[i] = columnNames[i] + " = " + valuesNames[i];
            }

            var updateCommand = "UPDATE " + table + " SET " + string.Join(", ", setString) + " WHERE " + updateCondition;
            var commandDb = new SQLiteCommand(updateCommand, _dbConnection);
            for (int i = 0; i < values.Length; i++)
            {
                commandDb.Parameters.AddWithValue(valuesNames[i], values[i]);
            }

            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new MarketException(MarketError.DbError,"Problem occured in the attempt to update system data in DB, returned error message :" + e.Message);
            }
        }

        protected void DeleteFromTable(string table,string deleteCondition)
        {
            var deleteCommand = "DELETE FROM " + table + " WHERE " + deleteCondition;
            var commandDb = new SQLiteCommand(deleteCommand, _dbConnection);
            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new MarketException(MarketError.DbError,"Problem occured in the attempt to delete system data in DB, returned error message :" + e.Message);
            }

        }

        protected SQLiteDataReader freeStyleSelect(string cmd)
        {
            return new SQLiteCommand(cmd, _dbConnection).ExecuteReader();
        }

    }
}
