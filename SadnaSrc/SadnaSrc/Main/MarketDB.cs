using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace SadnaSrc.Main
{
    public class MarketDB : IMarketDB
    {
        private static MarketDB _instance;

        public static MarketDB Instance => _instance ?? (_instance = new MarketDB());

        private SqlConnection _dbConnection;
        private MarketDB()
        {
            InitiateDb();
            CreateTables();
        }
        private void InitiateDb()
        {
            var dbPath = "Data Source=DESKTOP-NHU1RB6\\SQLEXPRESS;Initial Catalog=MarketData;Integrated Security=True;MultipleActiveResultSets=true";
            _dbConnection = new SqlConnection(dbPath);
             _dbConnection.Open();
        }

        private void CreateTables()
        {
            string[] createTableStrings =
            {
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
                CreateOrderItemTable(),
                CreateNotificationsTable(),
                CreateCategoryTable(),
                CreateCategoryProductConnectionTable(),
                CreateConditionTable(),
                CreateOperatorTable()
            };

            for (var i = 0; i < createTableStrings.Length; i++)
            {
                var createTableCommand = new SqlCommand(createTableStrings[i], _dbConnection);
                createTableCommand.ExecuteNonQuery();
            }
        }

        public void InsertByForce() { 

            string[] thingsToInsertByForce =
            {
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S1','X','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S2','Y','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S3','M','Here 4','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S4','Cluckin Bell','Los Santos','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S5','The Red Rock','Mivtza Yoav','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S6','24','Mezada','Active')",
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S7','T','wanderland','Active')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P1', 'BOX', 100, 'this is a plastic box')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P2', 'Golden BOX', 1000, 'this is a golden box')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P3', 'DeleteMy BOX', 10, 'this is a trush')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P4', 'Bamba', 6, 'munch')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P5', 'Goldstar', 11, 'beer')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P6', 'OCB', 10, 'accessories')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P7', 'Coated Peanuts', 10, 'munch')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P8', 'Alice', 10, 'popo')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P9', 'TheHatter', 10, 'popo')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P10', 'LittleCake', 100, 'eat my')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P11', 'LittleDrink', 200, 'drink my')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P12', 'CheshireCat', 200, 'smile')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P13', 'WhiteRabbit', 200, 'you are late')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P14', 'RedQueen', 200, 'Cutoff his head')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P15', 'Time', 200, 'Dont kill my')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P16', 'The March Hare', 200, 'Tea?')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P17', 'nonsense ', 200, 'no sense!')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P18', 'Pizza', 60, 'food')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P19', '#9', 5, 'its just a fucking burger, ok?')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P20', '#45 With Cheese', 18, 'its just a fucking cheesburger, ok?')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P21', 'Fraid Egg', 10, 'yami')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P22', 'OnePunchManPoster', 10, 'yami')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D1', 'HIDDEN', '2018-01-01', '2018-12-31', 50, 'True')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D2', 'HIDDEN', '2019-01-01', '2030-12-31', 50, 'True')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D3', 'HIDDEN', '2017-01-01', '2017-03-01', 50, 'True')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D4', 'HIDDEN', '2018-01-01', '2020-03-01', 50, 'True')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D5', 'HIDDEN', '2018-01-01', '2020-03-01', 50, 'False')",
                "INSERT INTO Discount (DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount, Percentages) VALUES ('D6', 'VISIBLE', '2018-01-01', '2020-03-01', 50, 'False')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S1', 'P1', 5, 'D1', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S1', 'P2', 5, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S4', 'P19', 10, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S4', 'P20', 1, 'null', 'Lottery')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S5', 'P4', 20, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S5', 'P5', 36, 'D1', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S6', 'P6', 100, 'D1', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S6', 'P7', 10, 'D1', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P3', 10, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P8', 10, 'D2', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P9', 10, 'D3', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P10', 10, 'D4', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P11', 10, 'D4', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P12', 10, 'D5', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P13', 10, 'D6', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S5', 'P18', 10, 'D1', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P14', 10, 'D6', 'Lottery')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P15', 10, 'null', 'Lottery')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P16', 10, 'null', 'Lottery')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P17', 10, 'null', 'Lottery')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P21', 10, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S7', 'P22', 10, 'null', 'Immediate')",
                "INSERT INTO Category (SystemID, name) VALUES ('C1', 'WanderlandItems')",
                "INSERT INTO CategoryProductConnection (CategoryID, ProductID) VALUES ('C1', 'P21')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L1', 'P1', 100, 0 , 'X' ,'2018-01-01', '2018-12-31', 'True')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L2', 'P15', 200, 0 , 'T' ,'2018-01-01', '2018-12-31', 'False')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L3', 'P16', 200, 0 , 'T' ,'2018-01-01', '2018-12-31', 'True')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L4', 'P17', 200, 0 , 'T' ,'2017-01-01', '2017-12-31', 'True')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L5', 'P17', 200, 0 , 'T' ,'2017-01-01', '2017-12-31', 'True')",
                "INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName, StartDate, EndDate, isActive) VALUES ('L6', 'P20', 18, 0 , 'T' ,'2018-01-01', '2018-12-31', 'True')",
                "INSERT INTO LotteryTicket (myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID) VALUES('T1', 'L1', 0, 0,0, 'WAITING', 0)",
                "INSERT INTO LotteryTicket (myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID) VALUES('T2', 'L2', 0, 0,2, 'WAITING', 8)",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (1,'Arik1','H3','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (2,'Arik2','H3','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (3,'Arik3','H3','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (4,'Big Smoke','Los Santos','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (5,'CJ','Los Santos','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (6,'Ryder','Los Santos','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (7,'Vadim Chernov','Mivtza Kilshon','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO Users (SystemID,Name,Address,Password,CreditCard) VALUES (8,'Vova','Menahem Donkelblum','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'SystemAdmin')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (2,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (3,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (4,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (5,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (6,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (7,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (8,'RegisteredUser')",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#9','Cluckin Bell',2,5.00,10.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#9 Large','Cluckin Bell',1,7.00,7.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#6 Extra Dip','Cluckin Bell',1,8.50,8.50)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#7','Cluckin Bell',1,8.00,8.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#45','Cluckin Bell',1,16.00,16.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'#45 With Cheese','Cluckin Bell',1,18.00,18.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'Large Soda','Cluckin Bell',1,5.00,5.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (4,'Gun','M',3,25.00,75.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (5,'Pizza','The Red Rock',2,60.00,120.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (6,'#9','Cluckin Bell',5,5.00,25.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (6,'BOX','X',2,100,200)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (7,'Bamba','The Red Rock',3,6.00,18.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (7,'Goldstar','The Red Rock',3,11.00,33.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (7,'OCB','24',2,10.00,20.00)",
                "INSERT INTO CartItem (SystemID,Name,Store,Quantity,UnitPrice,FinalPrice) VALUES (8,'Coated Peanuts','24',8,10.00,80.00)",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (5,'X','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'Y','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'M','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (5,'Cluckin Bell','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (8,'The Red Rock','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (7,'The Red Rock','ManageProducts')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (7,'The Red Rock','PromoteStoreAdmin')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (4,'The Red Rock','DeclareDiscountPolicy')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (5,'T','StoreOwner')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (1,'T','StoreOwner')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','Health Potion','X','Immediate',2,11.5,'2018-12-29')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','Mana Potion','Y','Lottery',3,12.0,'2018-12-29')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik1','INT Potion','Y','Lottery',2,8.0,'2018-12-29')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Arik3','STR Potion','Y','Immediate',1,4.0,'2018-12-29')",
                "INSERT INTO PurchaseHistory (UserName,Product,Store,SaleType,Quantity,Price,Date) VALUES ('Vadim Chernov','Goldstar','The Red Rock','Immediate',1,11.00,'2018-12-29')",
            };

            for (int i = 0; i < thingsToInsertByForce.Length; i++)
            {
                var insertCommand = new SqlCommand(thingsToInsertByForce[i], _dbConnection);
                try
                {
                    insertCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    //dont care
                }
            }

        }

        public void InsertByForceClient()
        {
            string[] thingsToInsertByForce =
            {
                "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S1','Avi`s Chocolate Kingdom','Here 4','Active')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P1', 'Dark Chocolate', 5, 'Join the darkside, we have chocolate')",
                "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P2', 'White Chocolate', 7, 'All your bases are belong to us')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S1', 'P1', 30, 'null', 'Immediate')",
                "INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S1', 'P2', 30, 'null', 'Immediate')",
                "INSERT INTO User (SystemID,Name,Address,Password,CreditCard) VALUES (1,'Avi','Ben-Gurion University','202cb962ac59075b964b07152d234b70','12345678')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'RegisteredUser')",
                "INSERT INTO StatePolicy (SystemID,State) VALUES (1,'SystemAdmin')",
                "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (1,'Avi`s Chocolate Kingdom','StoreOwner')",

	            "INSERT INTO Store (SystemID,Name,Address,Status) VALUES ('S2','Toy','StupidBoy','Active')",
	            "INSERT INTO User (SystemID,Name,Address,Password,CreditCard) VALUES (2,'Arik2','Mishol Susia','202cb962ac59075b964b07152d234b70','88888888')",
	            "INSERT INTO User (SystemID,Name,Address,Password,CreditCard) VALUES (3,'Arik3','Mishol','202cb962ac59075b964b07152d234b70','77777777')",
	            "INSERT INTO StatePolicy (SystemID,State) VALUES (3,'RegisteredUser')",
				"INSERT INTO StatePolicy (SystemID,State) VALUES (2,'RegisteredUser')",
	            "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (2,'Toy','ManageProducts')",
	            "INSERT INTO Products (SystemID, Name, BasePrice, Description) VALUES ('P3', 'euroticket', 5, 'Lets see the eurovision today')",
				"INSERT INTO Stock (StockID, ProductSystemID, Quantity, Discount, PurchaseWay) VALUES ('S2', 'P3', 30, 'null', 'Immediate')",
	            "INSERT INTO StoreManagerPolicy (SystemID,Store,Action) VALUES (3,'Toy','StoreOwner')",
			};
            for (int i = 0; i < thingsToInsertByForce.Length; i++)
            {
                var insertCommand = new SqlCommand(thingsToInsertByForce[i], _dbConnection);
                try
                {
                    insertCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    //dont care
                }
            }
        }

        public void CleanByForce()
        {
            var tableNames = new[]
            {
                "System_Log",
                "System_Errors",
                "Users",
                "Products",
                "Discount",
                "Stock",
                "LotteryTable",
                "LotteryTicket",
                "Store",
                "StatePolicy",
                "StoreManagerPolicy",
                "CartItem",
                "PurchaseHistory",
                "Orders",
                "OrderItem",
                "Notifications",
                "Category",
                "CategoryProductConnection",
                "conditions",
                "Operator"
            };

            for (int i = 0; i < tableNames.Length; i++)
            {
                var deleateTableCommand = new SqlCommand("Delete FROM " +tableNames[i], _dbConnection);
                deleateTableCommand.ExecuteNonQuery();
            }
        }


        private static string CreateSystemLogTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='System_Log' AND xtype='U')
                        CREATE TABLE [System_Log] (
                                    [LogID]         VARCHAR(256),
                                    [LogDate]       DATETIME,
                                    [ModuleName]    VARCHAR(256),
                                    [Description]   VARCHAR(256),
                                    PRIMARY KEY([LogID])
                                    )";
        }

        private static string CreateSystemErrorsTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='System_Errors' AND xtype='U') 
                        CREATE TABLE [System_Errors] (
                                    [ErrorID]       VARCHAR(256),
                                    [ModuleName]    VARCHAR(256),
                                    [Description]   VARCHAR(256),
                                    PRIMARY KEY([ErrorID])
                                    )";
        }

        private static string CreateUserTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U') 
                        CREATE TABLE [Users] (
                                    [SystemID]      INT,
                                    [Name]          VARCHAR(256),
                                    [Address]       VARCHAR(256),
                                    [Password]      VARCHAR(256),
                                    [CreditCard]    VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }

        private static string CreateStoreTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Store' AND xtype='U') 
                        CREATE TABLE [Store] (
                                    [SystemID]      VARCHAR(256),
                                    [Name]          VARCHAR(256), 
                                    [Address]       VARCHAR(256),
                                    [Status]        VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }

        private static string CreateUserStatePolicyTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StatePolicy' AND xtype='U') 
                        CREATE TABLE [StatePolicy] (
                                    [SystemID]      INT,
                                    [State]         VARCHAR(256),
                                    FOREIGN KEY([SystemID])     REFERENCES [Users]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[State])
                                    )";
        }

        private static string CreateUserStorePolicyTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StoreManagerPolicy' AND xtype='U') 
                        CREATE TABLE [StoreManagerPolicy] (
                                    [SystemID]      INT,
                                    [Store]         VARCHAR(256),
                                    [Action]        VARCHAR(256),
                                    FOREIGN KEY([SystemID])     REFERENCES [Users]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[Store],[Action])
                                    )";
        }

        private static string CreateCartItemTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CartItem' AND xtype='U') 
                        CREATE TABLE [CartItem] (
                                    [SystemID]      INT,
                                    [Name]          VARCHAR(256),
                                    [Store]         VARCHAR(256),
                                    [Quantity]      INTEGER,
                                    [UnitPrice]     FLOAT,
                                    [FinalPrice]    FLOAT,
                                    FOREIGN KEY([SystemID])     REFERENCES [Users]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([SystemID],[Name],[Store],[UnitPrice])
                                    )";

        }
        //                                    FOREIGN KEY([Store])        REFERENCES [Store]([Name])    ON DELETE CASCADE,
        private static string CreatePurchaseHistoryTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PurchaseHistory' AND xtype='U') 
                        CREATE TABLE [PurchaseHistory] (
                                    [UserName]      VARCHAR(256),
                                    [Product]       VARCHAR(256),
                                    [Store]         VARCHAR(256),
                                    [SaleType]      VARCHAR(256),
                                    [Quantity]      INT,
                                    [Price]         FLOAT,
                                    [Date]          DATETIME,
                                    PRIMARY KEY([UserName],[Product],[Store],[SaleType],[Date])
                                    )";
        }

        private static string CreateProductTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U') 
                        CREATE TABLE [Products] (
                                    [SystemID]     VARCHAR(256),
                                    [Name]         VARCHAR(256),
                                    [BasePrice]    FLOAT,
                                    [Description]  VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }
        private static string CreateDiscountTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Discount' AND xtype='U') 
                        CREATE TABLE [Discount] (
                                    [DiscountCode]          VARCHAR(256),
                                    [DiscountType]          VARCHAR(256),
                                    [StartDate]             DATETIME,
                                    [EndDate]               DATETIME,
                                    [DiscountAmount]        INT,
                                    [Percentages]           VARCHAR(256),
                                    PRIMARY KEY([DiscountCode])
                                    )";
        }
        private static string CreateStockTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Stock' AND xtype='U') 
                        CREATE TABLE [Stock] (
                                    [StockID]               VARCHAR(256),
                                    [ProductSystemID]       VARCHAR(256),
                                    [Quantity]              INT,
                                    [Discount]              VARCHAR(256),
                                    [PurchaseWay]           VARCHAR(256), CHECK (PurchaseWay IN ('Immediate', 'Lottery')),
                                    PRIMARY KEY([ProductSystemID]),
                                    FOREIGN KEY([ProductSystemID]) REFERENCES [Products]([SystemID]) ON DELETE CASCADE
                                    )";
        }
        
        private static string CreateLotteryTicketsTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LotteryTicket' AND xtype='U') 
                        CREATE TABLE [LotteryTicket] (
                                    [myID]              VARCHAR(256),
                                    [LotteryID]         VARCHAR(256),
                                    [IntervalStart]     FLOAT,
                                    [IntervalEnd]       FLOAT,
                                    [Cost]              FLOAT,
                                    [Status]            VARCHAR(256),
                                    [UserID]            INT,
                                    PRIMARY KEY([myID]),
                                    FOREIGN KEY([LotteryID]) REFERENCES [LotteryTable]([SystemID]) ON DELETE CASCADE
                                    )";
        }

        private static string CreateLotteryTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LotteryTable' AND xtype='U') 
                        CREATE TABLE [LotteryTable] (
                                    [SystemID]              VARCHAR(256),
                                    [ProductSystemID]       VARCHAR(256),
                                    [ProductNormalPrice]    FLOAT,
                                    [TotalMoneyPayed]       FLOAT,
                                    [storeName]             VARCHAR(256),
                                    [StartDate]             DATETIME,
                                    [EndDate]               DATETIME,
                                    [isActive]              VARCHAR(256),
                                    PRIMARY KEY([SystemID]),
                                    FOREIGN KEY([ProductSystemID]) REFERENCES [Products]([SystemID]) ON DELETE CASCADE
                                    )";
        }
        
        private static string CreateOrderTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U') 
                        CREATE TABLE [Orders] (
                                    [OrderID]           INT,
                                    [UserName]          VARCHAR(256),
                                    [ShippingAddress]   VARCHAR(256),
                                    [TotalPrice]        FLOAT,
                                    [Date]              VARCHAR(256),
                                    PRIMARY KEY([OrderID])
                                    )";
        }

        private static string CreateOrderItemTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderItem' AND xtype='U') 
                        CREATE TABLE [OrderItem] (
                                    [OrderID]       INT,
                                    [Store]         VARCHAR(256),
                                    [Name]          VARCHAR(256),
                                    [Price]         FLOAT,
                                    [Quantity]      INT,
                                    FOREIGN KEY([OrderID])      REFERENCES [Orders]([OrderID]) ON DELETE CASCADE,                                
                                    PRIMARY KEY([OrderID],[Store],[Name])
                                    )";
        }

        private static string CreateNotificationsTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U') 
                        CREATE TABLE [Notifications] (
                                    [NotificationID]    VARCHAR(256),
                                    [Receiver]          INT,
                                    [Message]           VARCHAR(256),
                                    [Status]            VARCHAR(256),
                                    FOREIGN KEY([Receiver])     REFERENCES [Users]([SystemID]) ON DELETE CASCADE,
                                    PRIMARY KEY([NotificationID])
                                    )";
        }
        private static string CreateCategoryTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Category' AND xtype='U') 
                        CREATE TABLE [Category] (
                                    [SystemID]          VARCHAR(256),
                                    [name]              VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }
        private string CreateCategoryProductConnectionTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CategoryProductConnection' AND xtype='U') 
                        CREATE TABLE [CategoryProductConnection] (
                                    [CategoryID]        VARCHAR(256),
                                    [ProductID]         VARCHAR(256),
                                    PRIMARY KEY([CategoryID], [ProductID])
                                    )";
        }

        private static string CreateConditionTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='conditions' AND xtype='U') 
                        CREATE TABLE [conditions] (
                                    [SystemID]              INT,
                                    [conditionsType]        VARCHAR(256),
                                    [PolicyType]            VARCHAR(256),
                                    [Subject]               VARCHAR(256),
                                    [value]                 VARCHAR(256),
                                    [isRoot]                VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }
        private static string CreateOperatorTable()
        {
            return @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Operator' AND xtype='U') 
                        CREATE TABLE [Operator] (
                                    [SystemID]               INT,
                                    [OperatorType]           VARCHAR(256),
                                    [PolicyType]             VARCHAR(256),
                                    [Subject]                VARCHAR(256),
                                    [COND1ID]                INT,
                                    [COND2ID]                INT,
                                    [isRoot]                 VARCHAR(256),
                                    PRIMARY KEY([SystemID])
                                    )";
        }
        public void InsertTable(string table,string tableColumns,string[] valuesNames,object[] values)
        {
            var insertRequest = "INSERT INTO "+table+" ("+ tableColumns + ") VALUES ("+ string.Join(",", valuesNames)
                                + ")";
            var commandDb = new SqlCommand(insertRequest, _dbConnection);
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

        public SqlDataReader SelectFromTable(string table, string toSelect)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table ;
            return new SqlCommand(selectRequest, _dbConnection).ExecuteReader();
        }

        public SqlDataReader SelectFromTableWithCondition(string table, string toSelect, string condition)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table + " WHERE "+condition;
            return new SqlCommand(selectRequest, _dbConnection).ExecuteReader();
        }

        public void UpdateTable(string table,string updateCondition,string[] columnNames, string[] valuesNames, object[] values)
        {
            string [] setString = new string[values.Length];
            for (int i = 0; i < setString.Length; i++)
            {
                setString[i] = columnNames[i] + " = " + valuesNames[i];
            }

            var updateCommand = "UPDATE " + table + " SET " + string.Join(", ", setString) + " WHERE " + updateCondition;
            var commandDb = new SqlCommand(updateCommand, _dbConnection);
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

        public void DeleteFromTable(string table,string deleteCondition)
        {
            var deleteCommand = "DELETE FROM " + table + " WHERE " + deleteCondition;
            var commandDb = new SqlCommand(deleteCommand, _dbConnection);
            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new MarketException(MarketError.DbError,"Problem occured in the attempt to delete system data in DB, returned error message :" + e.Message);
            }

        }

        public SqlDataReader freeStyleSelect(string cmd)
        {
            return new SqlCommand(cmd, _dbConnection).ExecuteReader();
        }
    } 
}
