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

        private static void CreateTables()
        {
            string[] createTableStrings = {
                CreateSystemLogTable(),
                CreateSystemErrorsTable(),
                CreateUserTable(),
                CreateProductTable(),
                CreateDiscountTable(), 
                CreateStockTable(), 
                CreateLotteryTable(), 
                CreateLotteryTicketsTable(), 
                CreateStoreTable(), 
                CreateUserStatePolicyTable(),
                CreateUserStorePolicyTable(),  
                CreateCartItemTable(),
                CreatePurchaseHistoryTable(),
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
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S1','X','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S2','Y','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S3','M','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S4','Cluckin Bell','Los Santos','Active')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P100', 'BOX', 100, 'this is a plastic box')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P101', 'Golden BOX', 1000, 'this is a golden box')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P102', 'DeleteMy BOX', 10, 'this is a trush')",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'X'",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'Y'",
                "UPDATE Store SET Status = 'Active' WHERE Name = 'M'",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (1,'Arik1','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (2,'Arik2','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (3,'Arik3','H3','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (4,'Big Smoke','Los Santos','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (5,'CJ','Los Santos','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO User (SystemID,Name,Address,Password) VALUES (6,'Ryder','Los Santos','202cb962ac59075b964b07152d234b70')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'SystemAdmin')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (2,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (3,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (4,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (5,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (6,'RegisteredUser')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#9','Cluckin Bell',2,5.00,10.00,'Lottery')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#9 Large','Cluckin Bell',1,7.00,7.00,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#6 Extra Dip','Cluckin Bell',1,8.50,8.50,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#7','Cluckin Bell',1,8.00,8.00,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#45','Cluckin Bell',1,16.00,16.00,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'#45 With Cheese','Cluckin Bell',1,18.00,18.00,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'Large Soda','Cluckin Bell',1,5.00,5.00,'Immediate')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType) VALUES (4,'Gun','M',3,25.00,75.00,'Immediate')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'Y','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'M','StoreOwner')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','Health Potion','X','Immediate',2,11.5,'Today')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','Mana Potion','Y','Lottery',3,12.0,'Yesterday')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','INT Potion','Y','Lottery',2,8.0,'Yesterday')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik3','STR Potion','Y','Immediate',1,4.0,'Today')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('CJ','#9','Cluckin Bell','Lottery',1,5.00,'Today')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Ryder','#9','Cluckin Bell','Lottery',1,5.00,'Today')",
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
                                    [SystemID]      TEXT,
                                    [Name]          TEXT  NOT NULL UNIQUE,
                                    [Address]       TEXT,
                                    [Status]        TEXT,
                                    PRIMARY KEY([SystemID])
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
                                    [Quantity]      INTEGER,
                                    [UnitPrice]     REAL,
                                    [FinalPrice]    REAL,
                                    [SaleType]      TEXT,
                                    FOREIGN KEY([SystemID])     REFERENCES [USER]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[Name],[Store],[UnitPrice],[SaleType])
                                    )";

            //TODO:                                     FOREIGN KEY([Store])        REFERENCES [Store]([Name])    ON DELETE CASCADE,
        }

        //TODO: this table is bad and should be deleted once OrderPool DB is finally ready
        private static string CreatePurchaseHistoryTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [PurchaseHistory] (
                                    [UserName]      TEXT,
                                    [Product]       TEXT,
                                    [Store]         TEXT,
                                    [SaleType]      TEXT,
                                    [Quantity]      INTEGER,
                                    [Price]         REAL,
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
            return @"CREATE TABLE IF NOT EXISTS [Products] (
                                    [SystemID]     TEXT,
                                    [Name]         TEXT,
                                    [BasePrice]    INTEGER,
                                    [Description]  TEXT,
                                    PRIMARY KEY([SystemID])
                                    )";
        }
        private static string CreateDiscountTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [Discount] (
                                    [DiscountCode]          TEXT,
                                    [DiscountType]          TEXT,
                                    [StartDate]             TEXT,
                                    [EndDate]               TEXT,
                                    [DiscountAmount]        INTEGER,
                                    [Percentages]           TEXT,
                                    PRIMARY KEY([DiscountCode])
                                    )";
        }
        private static string CreateStockTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [Stock] (
                                    [StockID]               TEXT,
                                    [ProductSystemID]       TEXT,
                                    [Quantity]              INTEGER,
                                    [Discount]              TEXT,
                                    [PurchaseWay]           TEXT, CHECK (PurchaseWay IN ('Immediate', 'Lottery')),
                                    PRIMARY KEY([StockID]),
                                    FOREIGN KEY([ProductSystemID]) REFERENCES [Products]([SystemID]) ON DELETE CASCADE,
                                    FOREIGN KEY([discount]) REFERENCES [Discount]([DiscountCode]) ON DELETE CASCADE
                                    )";
        }
        
        private static string CreateLotteryTicketsTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [LotteryTicket] (
                                    [myID]              TEXT,
                                    [LotteryID]         TEXT,
                                    [IntervalStart]     INTEGER,
                                    [IntervalEnd]       INTEGER,
                                    [Status]            TEXT, CHECK (Status IN ('WAITING', 'WINNING', 'LOSING', 'CANCEL')),
                                    PRIMARY KEY([myID]),
                                    FOREIGN KEY([LotteryID]) REFERENCES [LotteryTable]([SystemID]) ON DELETE CASCADE
                                    )";
        }

        private static string CreateLotteryTable()
        {
            return @"CREATE TABLE IF NOT EXISTS [LotteryTable] (
                                    [SystemID]              TEXT,
                                    [ProductSystemID]       TEXT,
                                    [ProductNormalPrice]    INTEGER,
                                    [TotalMoneyPayed]       INTEGER,
                                    [StartDate]             TEXT,
                                    [EndDate]               TEXT,
                                    [isActive]              TEXT,
                                    PRIMARY KEY([SystemID]),
                                    FOREIGN KEY([ProductSystemID]) REFERENCES [Products]([SystemID]) ON DELETE CASCADE
                                    )";
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
