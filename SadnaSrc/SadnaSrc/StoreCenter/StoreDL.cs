using SadnaSrc.Main;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreDL : IStoreDL
    {

        private static StoreDL _instance;

        public static StoreDL Instance => _instance ?? (_instance = new StoreDL());

        private MarketDB dbConnection;

        private StoreDL()
        {
            dbConnection = MarketDB.Instance;
        }

        public string[] GetAllStoresIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Store", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllProductIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Products", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllDiscountIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Discount", "DiscountCode"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public void AddProductToCategory(string categoryid, string productid)
        {
            string[] a = { "'" + categoryid + "'", "'" + productid + "'" };
            object[] b = { categoryid, productid };
            dbConnection.InsertTable("CategoryProductConnection", "CategoryID, ProductID",
               a,b);
        }
        public void RemoveProductFromCategory(string categoryid, string productid)
        {
            dbConnection.DeleteFromTable("CategoryProductConnection", "ProductID = '" + productid + "' AND CategoryID = '"+ categoryid + "'");
        }
        public string[] GetAllLotteryTicketIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTicket", "myID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllLotteryManagmentIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTable", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
      
        public Store GetStorebyName(string storeName)
        {

            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "*", "Name = '" + storeName + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                        dbReader.GetString(3));
                }

                return null;
            }
        }

      

        public void EditLotteryTicketInDatabase(LotteryTicket ticket)
        {

            string[] columnNames =
            {
                "myID",
                "LotteryID",
                "IntervalStart",
                "IntervalEnd",
                "Cost",
                "Status",
                "UserID"
            };
            dbConnection.UpdateTable("LotteryTicket", "myID = '" + ticket.myID + "'", columnNames,
                new []{"@idParam","@lotteryParam","@interStart","@interEnd","@cost","@stat","@user"}, ticket.GetTicketValuesArray());
        }


        public Store GetStorebyID(string storeid)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "*", "SystemID = '" + storeid + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                        dbReader.GetString(3));
                }

                return null;
            }
        }



        


        public void AddProductToDatabase(Product product)
        {
            dbConnection.InsertTable("Products", "SystemID, name, BasePrice, description",
                new []{"@idParam","@name","@price","@desc"}, product.GetProductValuesArray());
        }

        public void EditProductInDatabase(Product product)
        {
            string[] columnNames =
            {
                "SystemID",
                "name",
                "BasePrice",
                "description"
            };
            dbConnection.UpdateTable("Products", "SystemID = '" + product.SystemId + "'", columnNames,
                new[] { "@idParam", "@name", "@price", "@desc" }, product.GetProductValuesArray());
        }

        public LinkedList<LotteryTicket> GetAllTickets(string systemid)
        {
            LinkedList<LotteryTicket> result = new LinkedList<LotteryTicket>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "LotteryID = '" + systemid + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lottery = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1),
                        dbReader.GetDouble(2), dbReader.GetDouble(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lottery.myStatus = EnumStringConverter.GetLotteryStatusString(dbReader.GetString(5));
                    result.AddLast(lottery);
                }
            }
            return result;
        }

        private PurchaseHistory[] GetPurchaseHistory(SqlDataReader dbReader)
        {
            List<PurchaseHistory> historyData = new List<PurchaseHistory>();
            while (dbReader.Read())
            {
                historyData.Add(new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                    dbReader.GetString(3), dbReader.GetInt32(4), dbReader.GetDouble(5), dbReader.GetString(6)));
            }

            return historyData.ToArray();
        }

        public StockListItem GetStockListItembyProductID(string productid)
        {
            Product product = GetProductID(productid);
            StockListItem stockListItem = null;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "*", "ProductSystemID = '" + productid + "'"))
            {
                while (dbReader.Read())
                {
                    stockListItem = new StockListItem(dbReader.GetInt32(2), product,
                        GetDiscount(dbReader.GetString(3)), EnumStringConverter.GetPurchaseEnumString(dbReader.GetString(4)),
                        dbReader.GetString(0));
                    return stockListItem;
                }
            }

            return stockListItem;
        }

        public Discount GetDiscount(string discountCode)
        {
            Discount discount = null;
            using (var discountReader =
                dbConnection.SelectFromTableWithCondition("Discount", "*", "DiscountCode = '" + discountCode + "'"))
            {
                while (discountReader.Read())
                {
                    discount = new Discount(discountCode,
                        EnumStringConverter.GetdiscountTypeEnumString(discountReader.GetString(1)),
                        DateTime.Parse(discountReader.GetString(2))
                        , DateTime.Parse(discountReader.GetString(3))
                        , discountReader.GetInt32(4),
                        Boolean.Parse(discountReader.GetString(5)));
                }
            }

            return discount;
        }
        
        public bool IsStoreExistAndActive(string store)
        {
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Store", "*", " Name = '" + store + "' AND Status = 'Active'"))
            {
                return dbReader.Read();
            }
        }
       
        public void AddStore(Store store)
        {
            dbConnection.InsertTable("Store", "SystemID, Name, Address, Status",
                new []{"@idParam","@nameParam","@addressParam","@statParam"}, store.GetStoreArray());
        }

        public LotteryTicket GetLotteryTicket(string ticketid)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "myID = '" + ticketid + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lotty = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1),
                        dbReader.GetDouble(2), dbReader.GetDouble(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lotty.myStatus = EnumStringConverter.GetLotteryStatusString(dbReader.GetString(5));
                    return lotty;
                }
            }

            return null;
        }

        public void RemoveLotteryTicket(LotteryTicket lottery)
        {
            dbConnection.DeleteFromTable("LotteryTicket", "myID = '" + lottery.myID + "'");
        }

        public void AddLotteryTicket(LotteryTicket ticket)
        {
            dbConnection.InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID",
                new[] { "@idParam", "@lotteryParam", "@interStart", "@interEnd", "@cost", "@stat", "@user" },
                ticket.GetTicketValuesArray());
        }

        public Product GetProductID(string iD)
        {
            using (var productReader = dbConnection.SelectFromTableWithCondition("Products", "*", "SystemID = '" + iD + "'"))
            {
                while (productReader.Read())
                {
                    return new Product(iD, productReader.GetString(1), productReader.GetDouble(2),
                        productReader.GetString(3));
                }
            }

            return null;

        }

        public string[] GetHistory(Store store)
        {
            string[] result;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + store.Name + "'"))
            {
                PurchaseHistory[] resultPurchase = GetPurchaseHistory(dbReader);
                result = new string[resultPurchase.Length];
                int i = 0;
                foreach (PurchaseHistory purchaseHistory in resultPurchase)
                {
                    result[i] = purchaseHistory.ToString();
                    i++;
                }
            }
            return result;
        }

        public void AddDiscount(Discount discount)
        {
            dbConnection.InsertTable("Discount", "DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount,Percentages ",
                discount.GetDiscountStringValues(), discount.GetDiscountValuesArray());
        }


        public void AddStockListItemToDataBase(StockListItem stockListItem)
        {
            if (stockListItem.Discount != null)
            {
                AddDiscount(stockListItem.Discount);
            }

            AddProductToDatabase(stockListItem.Product);
            dbConnection.InsertTable("Stock", "StockID, ProductSystemID, quantity, discount, PurchaseWay",
                new []{"@idParam","@productIdParam","@quantParam","@discountParam","@purchParam"}, stockListItem.GetStockListItemArray());
        }



        public void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            dbConnection.DeleteFromTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'");
        }

        public void RemoveStockListItem(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                var lsmt = GetLotteryByProductID(stockListItem.Product.SystemId);
                if (lsmt != null)
                    RemoveLottery(lsmt);
            }
            if (stockListItem.Discount != null)
            {
                RemoveDiscount(stockListItem.Discount);
            }

            RemoveProduct(stockListItem
                .Product); // the DB will delete the StockListItem due to the conection between of the 2 tables
        }

        public void EditDiscountInDatabase(Discount discount)
        {
            string[] columnNames =
            {
                "DiscountCode",
                "DiscountType",
                "StartDate",
                "EndDate",
                "DiscountAmount",
                "Percentages"
            };
            dbConnection.UpdateTable("Discount", "DiscountCode = '" + discount.discountCode + "'", columnNames,
                discount.GetDiscountStringValues(), discount.GetDiscountValuesArray());
        }

        public void EditStore(Store store)
        {

            string[] columnNames =
            {
                "SystemID",
                "Name",
                "Address",
                "Status",
            };
            dbConnection.UpdateTable("Store", "SystemID = '" + store.SystemId + "'", columnNames,
                new[] { "@idParam", "@nameParam", "@addressParam", "@statParam" }, store.GetStoreArray());
        }




        public void RemoveProduct(Product product)
        {
            dbConnection.DeleteFromTable("Products", "SystemID = '" + product.SystemId + "'");
        }

        public void EditStockInDatabase(StockListItem stockListItem)
        {
            string[] columnNames =
            {
                "StockID",
                "ProductSystemID",
                "quantity",
                "discount",
                "PurchaseWay"
            };
            dbConnection.UpdateTable("Stock", "ProductSystemID = '" + stockListItem.Product.SystemId + "'",
                columnNames, new[] { "@idParam", "@productIdParam", "@quantParam", "@discountParam", "@purchParam" },
                stockListItem.GetStockListItemArray());
        }

        public void RemoveStore(Store store)
        {
            dbConnection.DeleteFromTable("Store", "SystemID = '" + store.SystemId + "'");
        }

        public string[] GetStoreInfo(string store)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "Name,Address",
                " Name = '" + store + "' AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(0), dbReader.GetString(1) };

                }
            }
            return null; 
        }
        public Product GetProductByNameFromStore(string storeName, string productName)
        {
            Store store = GetStorebyName(storeName);
            var productsid = GetAllStoreProductsID(store.SystemId);
            foreach (string id in productsid)
            {
                Product product = GetProductID(id);
                if (product.Name == productName)
                {
                    return product;
                }
            }

            return null;
        }
        public string[] GetStoreStockInfo(string store)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "Name,Address",
                " Store = '" + store + " AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(1), dbReader.GetString(2) };

                }
            }
            return null;
        }
        public StockListItem GetProductFromStore(string store, string productName)
        {
            Product product = GetProductByNameFromStore(store, productName);
            if (product == null) return null;
            return GetStockListItembyProductID(product.SystemId);

        }

        public void RemoveDiscount(Discount discount)
        {
            dbConnection.DeleteFromTable("Discount", "DiscountCode = '" + discount.discountCode + "'");
        }



        public void AddLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            dbConnection.InsertTable("LotteryTable",
                "SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName ,StartDate,EndDate,IsActive ",
                lotteryManagment.GetLotteryManagmentStringValues(), lotteryManagment.GetLotteryManagmentValuesArray());

        }


        public string[] GetAllStoreProductsID(string systemid)
        {
            List<string> result = new List<string>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = '" + systemid + "'"))
            {
                while (dbReader.Read())
                {
                    result.Add(dbReader.GetString(0));
                }
            }

            return result.ToArray();
        }

        public LotterySaleManagmentTicket GetLotteryByProductNameAndStore(string storeName, string productName)
        {
            Product product = GetProductByNameFromStore(storeName, productName);
            return GetLotteryByProductID(product.SystemId);
        }

        public int GetUserIDFromUserName(string userName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "SystemID", "Name = '" + userName + "'"))
            {
                while (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }

                return -1;
            }

        }

        public LotterySaleManagmentTicket GetLotteryByProductID(string productid)
        {
            Product product = GetProductID(productid);
            LotterySaleManagmentTicket lotteryManagement = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = '" + productid + "'"))
            {
                while (dbReader.Read())
                {
                    lotteryManagement = new LotterySaleManagmentTicket(dbReader.GetString(0), dbReader.GetString(4), product,
                        DateTime.Parse(dbReader.GetString(5)), DateTime.Parse(dbReader.GetString(6)));
                    lotteryManagement.TotalMoneyPayed = dbReader.GetDouble(3);

                    lotteryManagement.IsActive = (Boolean.Parse(dbReader.GetString(7)));
                }
            }

            return lotteryManagement;
        }
        public void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment)
        {
            string[] columnNames =
            {
                "SystemID",
                "ProductSystemID",
                "ProductNormalPrice",
                "TotalMoneyPayed",
                "storeName",
                "StartDate",
                "EndDate",
                "IsActive"
            };
            dbConnection.UpdateTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'", columnNames,
                lotteryManagment.GetLotteryManagmentStringValues(), lotteryManagment.GetLotteryManagmentValuesArray());
        }
        public Category GetCategoryByName(string categoryname)
        {
            Category category = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Category", "*", "name = '" + categoryname + "'"))
            {
                while (dbReader.Read())
                {
                    category = new Category(dbReader.GetString(0), dbReader.GetString(1));
                }
            }
            return category;
        }
        public LinkedList<Product> GetAllCategoryProducts(string categoryid)
        {
            LinkedList<Product> products = new LinkedList<Product>();
            LinkedList<string> productids = new LinkedList<string>();
            Product product;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("CategoryProductConnection", "ProductID", "CategoryID = '" + categoryid + "'"))
            {
                while (dbReader.Read())
                {
                    productids.AddLast(dbReader.GetString(0));
                }
            }
            foreach (string id in productids)
            {
                product = GetProductID(id);
                products.AddLast(product);
            }
            return products;
        }

        public string[] GetAllCategorysIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Category", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
    }
}