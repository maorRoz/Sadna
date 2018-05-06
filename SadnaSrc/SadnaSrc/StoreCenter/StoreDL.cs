using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public LinkedList<string> GetAllStoresIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Store", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids;
        }
        public LinkedList<string> GetAllProductIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Products", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids;
        }
        public LinkedList<string> GetAllDiscountIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Discount", "DiscountCode"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids;
        }
        public LinkedList<string> GetAllLotteryTicketIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTicket", "myID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids;
        }
        public LinkedList<string> GetAllLotteryManagmentIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTable", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids;
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

      

        public void EditLotteryTicketInDatabase(LotteryTicket lotter)
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
            dbConnection.UpdateTable("LotteryTicket", "myID = '" + lotter.myID + "'", columnNames,
                lotter.GetTicketStringValues(), lotter.GetTicketValuesArray());
        }


        public Store GetStorebyID(string storeID)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "*", "SystemID = '" + storeID + "'"))
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
                product.GetProductStringValues(), product.GetProductValuesArray());
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
                product.GetProductStringValues(), product.GetProductValuesArray());
        }

        public LinkedList<LotteryTicket> GetAllTickets(string systemID)
        {
            StockSyncher handler = StockSyncher.Instance;
            LinkedList<LotteryTicket> result = new LinkedList<LotteryTicket>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "LotteryID = '" + systemID + "'"))
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

        private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
        {
            List<PurchaseHistory> historyData = new List<PurchaseHistory>();
            while (dbReader.Read())
            {
                historyData.Add(new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                    dbReader.GetString(3), dbReader.GetInt32(4), dbReader.GetDouble(5), dbReader.GetString(6)));
            }

            return historyData.ToArray();
        }

        public StockListItem GetStockListItembyProductID(string product)
        {
            Product _product = GetProductID(product);
            StockSyncher handler = StockSyncher.Instance;
            StockListItem stockListItem = null;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "*", "ProductSystemID = '" + product + "'"))
            {
                while (dbReader.Read())
                {
                    stockListItem = new StockListItem(dbReader.GetInt32(2), _product,
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

            StockSyncher handler = StockSyncher.Instance;
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
       
        public void AddStore(Store toAdd)
        {
            dbConnection.InsertTable("Store", "SystemID, Name, Address, Status",
                toAdd.GetStoreStringValues(), toAdd.GetStoreArray());
        }

        public LotteryTicket GetLotteryTicket(string ticketID)
        {
            StockSyncher handler = StockSyncher.Instance;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "myID = '" + ticketID + "'"))
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

        public void AddLotteryTicket(LotteryTicket lottery)
        {
            dbConnection.InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID",
                lottery.GetTicketStringValues(), lottery.GetTicketValuesArray());
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

                return result;
            }
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
                stockListItem.GetStockListItemStringValues(), stockListItem.GetStockListItemArray());
        }



        public void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            dbConnection.DeleteFromTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'");
        }

        public void RemoveStockListItem(StockListItem stockListItem)
        {
            StockSyncher handler = StockSyncher.Instance;
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                LotterySaleManagmentTicket LSMT = GetLotteryByProductID(stockListItem.Product.SystemId);
                if (LSMT != null)
                    RemoveLottery(LSMT);
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
                store.GetStoreStringValues(), store.GetStoreArray());
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
            dbConnection.UpdateTable("Stock", "ProductSystemID = '" + stockListItem.Product.SystemId + "'", columnNames,
                stockListItem.GetStockListItemStringValues(), stockListItem.GetStockListItemArray());
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
        public Product GetProductByNameFromStore(string storeName, string ProductName)
        {
            Store store = GetStorebyName(storeName);
            LinkedList<string> productsID = GetAllStoreProductsID(store.SystemId);
            foreach (string ID in productsID)
            {
                Product product = GetProductID(ID);
                if (product.Name == ProductName)
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


        public LinkedList<string> GetAllStoreProductsID(string systemID)
        {
            LinkedList<string> result = new LinkedList<string>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = '" + systemID + "'"))
            {
                while (dbReader.Read())
                {
                    result.AddLast(dbReader.GetString(0));
                }
            }

            return result;
        }

        public LotterySaleManagmentTicket GetLotteryByProductNameAndStore(string storeName, string productName)
        {
            Product P = GetProductByNameFromStore(storeName, productName);
            return GetLotteryByProductID(P.SystemId);
        }

        public int GetUserIDFromUserName(string userName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("User", "SystemID", "Name = '" + userName + "'"))
            {
                while (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }

                return -1;
            }

        }

        public LotterySaleManagmentTicket GetLotteryByProductID(string productID)
        {
            Product P = GetProductID(productID);
            LotterySaleManagmentTicket lotteryManagement = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = '" + productID + "'"))
            {
                while (dbReader.Read())
                {
                    lotteryManagement = new LotterySaleManagmentTicket(dbReader.GetString(0), dbReader.GetString(4), P,
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

    }
}