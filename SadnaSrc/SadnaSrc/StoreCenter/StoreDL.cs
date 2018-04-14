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
    public class StoreDL : SystemDL
    {
        public int FindMaxStoreId()
        {
            using (var dbReader = SelectFromTable("Store", "*"))
            {
                return FindMaxId(dbReader);

            }
        }

        public int FindMaxProductId()
        {
            using (var dbReader = SelectFromTable("Products", "*"))
            {
                return FindMaxId(dbReader);

            }
        }

        public int FindMaxLotteryId()
        {
            using (var dbReader = SelectFromTable("LotteryTable", "*"))
            {
                return FindMaxId(dbReader);

            }
        }

        public int FindMaxLotteryTicketId()
        {
            using (var dbReader = SelectFromTable("LotteryTicket", "*"))
            {
                return FindMaxId(dbReader);

            }
        }

        public Store getStorebyName(string storeName)
        {

            using (var dbReader = SelectFromTableWithCondition("Store", "*", "Name = '" + storeName + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetString(3));
                }
                return null;
            }
        }

        public int FindMaxDiscountId()
        {
            using (var dbReader = SelectFromTable("Discount", "*"))
            {
                return FindMaxId(dbReader);

            }
        }
        private int FindMaxId(SQLiteDataReader dbReader)
        {
            int count = 1;
            while (dbReader.Read())
            {
                count++;
            }
            return count;
        }

        private object[] GetStockListItemArray(StockListItem stockListItem)
        {
            object discountObject = "";
            if (stockListItem.Discount != null)
            { discountObject = stockListItem.Discount; }
            return new object[]
            {
                stockListItem.SystemId,
                stockListItem.Product,
                stockListItem.Quantity,
                discountObject,
                stockListItem.PurchaseWay
            };
        }
        private object[] GetProductValuesArray(Product product)
        {
            return new object[]
            {
                product.SystemId,
                product.Name,
                product.BasePrice,
                product.Description
            };
        }



        private object[] GetTicketValuesArray(LotteryTicket lottery)
        {

            return new object[]
            {
                lottery.myID,
                lottery.LotteryNumber,
                lottery.IntervalStart,
                lottery.IntervalEnd,
                lottery.myStatus,
                lottery.UserID
            };
        }

        private object[] GetDiscountValuesArray(Discount discount)
        {
            return new object[]
            {
                discount.discountCode,
                discount.discountType,
                discount.startDate,
                discount.EndDate,
                discount.DiscountAmount,
                discount.Percentages
            };
        }
        private object[] GetLotteryManagmentValuesArray(LotterySaleManagmentTicket lotterySaleManagementTicket)
        {
            return new object[]
            {
                lotterySaleManagementTicket.SystemID,
                lotterySaleManagementTicket.Original.SystemId,
                lotterySaleManagementTicket.ProductNormalPrice,
                lotterySaleManagementTicket.TotalMoneyPayed,
                lotterySaleManagementTicket.StartDate,
                lotterySaleManagementTicket.EndDate,
                lotterySaleManagementTicket.IsActive
            };
        }
        private object[] GetStoreArray(Store store)
        {
            return new object[]
            {
                store.SystemId,
                store.Name,
                store.Address,
                store.GetStringFromActive()
            };
        }

        private string[] GetTicketStringValues(LotteryTicket lottery)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return new[]
            {
                "'" + lottery.myID + "'",
                "'" + lottery.LotteryNumber + "'",
                "'" + lottery.IntervalStart + "'",
                "'" + lottery.IntervalEnd + "'",
                "'" + lottery.Cost + "'",
                "'" + handler.PrintEnum(lottery.myStatus) + "'",
                "'" + lottery.UserID + "'"
            };
        }
        private string[] GetDiscountStringValues(Discount discount)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return new[]
            {
                "'" + discount.discountCode + "'",
                 "'" + handler.PrintEnum(discount.discountType) + "'",
                "'" + discount.startDate + "'",
                "'" + discount.EndDate + "'",
                "'" + discount.DiscountAmount + "'",
                "'" + discount.Percentages + "'"
            };
        }
        private string[] GetProductStringValues(Product product)
        {
            return new[]
            {
                "'" + product.SystemId + "'",
                "'" + product.Name + "'",
                product.BasePrice + "",
                "'" + product.Description + "'"
            };
        }
        private string[] GetStockListItemStringValues(StockListItem stockListItem)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            string IfDiscountNotExists = "null";
            if (stockListItem.Discount != null) { IfDiscountNotExists = stockListItem.Discount.discountCode; }
            return new[]
            {
                "'" + stockListItem.SystemId + "'",
                "'" + stockListItem.Product.SystemId + "'",
                "'" + stockListItem.Quantity + "'",
                "'" + IfDiscountNotExists + "'",
                "'" + handler.PrintEnum(stockListItem.PurchaseWay) + "'"
            };
        }

        public Store GetStorebyID(string storeID)
        {
            using (var dbReader = SelectFromTableWithCondition("Store", "*", "SystemID = '" + storeID + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetString(3));
                }
                return null;
            }
        }

        private string[] GetStoreStringValues(Store store)
        {
            return new[]
            {
                "'" + store.SystemId + "'",
                "'" + store.Name + "'",
                "'" + store.Address + "'",
                "'" + store.GetStringFromActive() + "'"
            };
        }

        public string[] GetLotteryManagmentStringValues(LotterySaleManagmentTicket lotterySaleManagementTicket)
        {
            string isActive = "";
            if (lotterySaleManagementTicket.IsActive) { isActive = "true"; } else { isActive = "false"; }

            return new[]
            {
                 "'" + lotterySaleManagementTicket.SystemID + "'",
                "'" + lotterySaleManagementTicket.Original.SystemId + "'",
                "'" + lotterySaleManagementTicket.ProductNormalPrice + "'",
                "'" + lotterySaleManagementTicket.TotalMoneyPayed + "'",
                "'" + lotterySaleManagementTicket.StartDate + "'",
                "'" + lotterySaleManagementTicket.EndDate + "'",
                "'" + isActive + "'"
            };
        }


        public void AddProductToDatabase(Product product)
        {
            InsertTable("Products", "SystemID, name, BasePrice, description",
                GetProductStringValues(product), GetProductValuesArray(product));
        }
        public void EditProductInDatabase(Product product)
        {
            string[] columnNames = {
                "SystemID",
                "name",
                "BasePrice",
                "description"
            };
            UpdateTable("Products", "SystemID = '" + product.SystemId + "'", columnNames,
                GetProductStringValues(product), GetProductValuesArray(product));
        }

        public LinkedList<LotteryTicket> getAllTickets(string systemID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            LinkedList<LotteryTicket> result = new LinkedList<LotteryTicket>();
            using (var dbReader = SelectFromTableWithCondition("LotteryTicket", "*", "LotteryID = '" + systemID + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lottery = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetInt32(2), dbReader.GetInt32(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lottery.myStatus = handler.GetLotteryStatusString(dbReader.GetString(5));
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
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = null;
            using (var dbReader = SelectFromTableWithCondition("Stock", "*", "ProductSystemID = '" + product + "'"))
            {
                while (dbReader.Read())
                {
                    stockListItem = new StockListItem(dbReader.GetInt32(2), _product, GetDiscount(dbReader.GetString(3)), handler.GetPurchaseEnumString(dbReader.GetString(4)), dbReader.GetString(0));
                    return stockListItem;
                }
            }

            return stockListItem;
        }
        public Discount GetDiscount(string DiscountCode)
        {
            Discount discount = null;

            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            using (var discountReader = SelectFromTableWithCondition("Discount", "*", "DiscountCode = '" + DiscountCode + "'"))
            {
                while (discountReader.Read())
                {
                        discount = new Discount(DiscountCode, handler.GetdiscountTypeEnumString(discountReader.GetString(1)),
                        DateTime.Parse(discountReader.GetString(2))
                        , DateTime.Parse(discountReader.GetString(3))
                        , discountReader.GetInt32(4),
                        Boolean.Parse(discountReader.GetString(5)));
                }
            }
            return discount;
        }

	    public void ValidateStoreExists(string store)
	    {
		    if (!IsStoreExist(store))
		    {
			    throw new StoreException(MarketError.LogicError, "store not found");
		    }   
	    }

        public bool IsStoreExist(string store)
        {
            using (var dbReader = SelectFromTableWithCondition("Store", "*", " Name = '" + store + "'"))
            {
                return dbReader.Read();
            }
        }

        public bool IsStoreExistAndActive(string store)
        {
            using (var dbReader = SelectFromTableWithCondition("Store", "*", " Name = '" + store + "' AND Status = 'Active'"))
            {
                return dbReader.Read();
            }
        }
        public void AddStore(Store toAdd)
        {
            if (IsStoreExist(toAdd.Name))
            {
                throw new StoreException(OpenStoreStatus.AlreadyExist, "Cannot open another instance of store " + toAdd.Name
                                                             + "Store already exist in the system!");
            }
            InsertTable("Store", "SystemID, Name, Address, Status",
                GetStoreStringValues(toAdd), GetStoreArray(toAdd));
        }

        public LotteryTicket GetLotteryTicket(string ticketID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            using (var dbReader = SelectFromTableWithCondition("LotteryTicket", "*", "myID = '" + ticketID + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lotty = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetInt32(2), dbReader.GetInt32(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lotty.myStatus = handler.GetLotteryStatusString(dbReader.GetString(5));
                    return lotty;
                }
            }
            return null;
        }
        public void RemoveLotteryTicket(LotteryTicket lottery)
        {
            DeleteFromTable("LotteryTicket", "myID = '" + lottery.myID + "'");
        }
        public void AddLotteryTicket(LotteryTicket lottery)
        {
            InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID",
                GetTicketStringValues(lottery), GetTicketValuesArray(lottery));
        }

        public Product GetProductID(string iD)
        {
            using (var productReader = SelectFromTableWithCondition("Products", "*", "SystemID = '" + iD + "'"))
            {
                while (productReader.Read())
                {
                    return new Product(iD, productReader.GetString(1), productReader.GetInt32(2), productReader.GetString(3));
                }
            }
            return null;

        }

        public string[] GetHistory(Store store)
        {
            string[] result;
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + store.SystemId + "'"))
            {
                if (!dbReader.Read())
                {
                    throw new StoreException(ViewStorePurchaseHistoryStatus.InvalidStore, "Couldn't find any store with that ID in history records");
                }
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
            InsertTable("Discount", "DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount,Percentages ",
                GetDiscountStringValues(discount), GetDiscountValuesArray(discount));
        }


        public void AddStockListItemToDataBase(StockListItem stockListItem)
        {
            if (stockListItem.Discount != null)
            {
                AddDiscount(stockListItem.Discount);
            }
            AddProductToDatabase(stockListItem.Product);
            InsertTable("Stock", "StockID, ProductSystemID, quantity, discount, PurchaseWay",
                   GetStockListItemStringValues(stockListItem), GetStockListItemArray(stockListItem));
        }



        public void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            DeleteFromTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'");
        }

        public void RemoveStockListItem(StockListItem stockListItem)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            if (stockListItem.Discount != null)
            {
                RemoveDiscount(stockListItem.Discount);
            }
            RemoveProduct(stockListItem.Product); // the DB will delete the StockListItem due to the conection between of the 2 tables
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
            UpdateTable("Discount", "DiscountCode = '" + discount.discountCode + "'", columnNames,
                GetDiscountStringValues(discount), GetDiscountValuesArray(discount));
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
            UpdateTable("Store", "SystemID = '" + store.SystemId + "'", columnNames,
                GetStoreStringValues(store), GetStoreArray(store));
        }




        public void RemoveProduct(Product product)
        {
            DeleteFromTable("Products", "SystemID = '" + product.SystemId + "'");
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
            UpdateTable("Stock", "ProductSystemID = '" + stockListItem.Product.SystemId + "'", columnNames,
                GetStockListItemStringValues(stockListItem), GetStockListItemArray(stockListItem));
        }

        public void RemoveStore(Store store)
        {
            DeleteFromTable("Store", "SystemID = '" + store.SystemId + "'");
        }

        public LinkedList<Store> GetAllActiveStores() // all active stores
        {
            LinkedList<Store> result = new LinkedList<Store>();
            using (var dbReader = SelectFromTableWithCondition("Store", "*", "Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    Store store = new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetString(3));
                    result.AddLast(store);
                }
            }
            return result;
        }

        public string[] GetStoreInfo(string store)
        {
            using (var dbReader = SelectFromTableWithCondition("Store", "Name,Address", " Name = '" + store + "' AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(0), dbReader.GetString(1) };

                }
            }
            throw new StoreException(ViewStoreStatus.NoStore, "There is no active store by the name of " + store);
        }
        public Product getProductByNameFromStore(string storeName, string ProductName)
        {
            Store store = getStorebyName(storeName);
            if (store == null) { throw new StoreException(StoreEnum.StoreNotExists, "not exists"); }
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
            using (var dbReader = SelectFromTableWithCondition("Stock", "Name,Address", " Store = '" + store + " AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(1), dbReader.GetString(2) };

                }
            }
            throw new StoreException(ViewStoreStatus.NoStore, "There is no active store by the name of " + store);
        }

        public StockListItem GetProductFromStore(string store, string productName)
        {
            Product product = getProductByNameFromStore(store, productName);
            if (product == null)
                throw new StoreException(AddProductStatus.NoProduct, "There is no product " + productName + " from " + store + "");
            return GetStockListItembyProductID(product.SystemId);
            

        }

        public void RemoveDiscount(Discount discount)
        {
            DeleteFromTable("Discount", "DiscountCode = '" + discount.discountCode + "'");
        }



        public void AddLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            InsertTable("LotteryTable", "SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, StartDate,EndDate,IsActive ",
                GetLotteryManagmentStringValues(lotteryManagment), GetLotteryManagmentValuesArray(lotteryManagment));

        }


        public LinkedList<string> GetAllStoreProductsID(string systemID)
        {
            LinkedList<string> result = new LinkedList<string>();
            using (var dbReader = SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = '" + systemID + "'"))
            {
                while (dbReader.Read())
                {
                    result.AddLast(dbReader.GetString(0));
                }
            }
            return result;
        }

        public LotterySaleManagmentTicket GetLotteryByProductID(string productID)
        {
            Product P = GetProductID(productID);
            LotterySaleManagmentTicket lotteryManagement = null;
            using (var dbReader = SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = '" + productID + "'"))
            {
                while (dbReader.Read())
                {
                    lotteryManagement = new LotterySaleManagmentTicket(dbReader.GetString(0), P, DateTime.Parse(dbReader.GetString(4)), DateTime.Parse(dbReader.GetString(5)));
                    lotteryManagement.TotalMoneyPayed = dbReader.GetInt32(3);
                    lotteryManagement.IsActive = (Boolean.Parse(dbReader.GetString(6)));
                }
            }
            return lotteryManagement;
        }
        //"INSERT INTO LotteryTable (SystemID, ProductSystemID, ProductNormalPrice, 
        //TotalMoneyPayed, StartDate, EndDate, isActive) VALUES 
        //('L100', 'P101', 100, 0 ,'01/01/2018', '31/12/2018', 'true')"
        public void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment)
        {
            string[] columnNames =
            {
                "SystemID",
                "ProductSystemID",
                "ProductNormalPrice",
                "TotalMoneyPayed",
                "StartDate",
                "EndDate",
                "IsActive"
            };
            UpdateTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'", columnNames,
                GetLotteryManagmentStringValues(lotteryManagment), GetLotteryManagmentValuesArray(lotteryManagment));
        }

    }
}
