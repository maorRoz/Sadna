using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.AdminView;

namespace SadnaSrc.StoreCenter
{
    public class StoreDL : SystemDL
    {

        private object[] GetStockListItemArray(StockListItem stockListItem)
        {
            return new object[]
            {
                stockListItem.SystemId,
                stockListItem.Product,
                stockListItem.Quantity,
                stockListItem.Discount,
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
                lottery.myStatus
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
            return new []
            {
                "'" + lottery.myID + "'",
                "'" + lottery.LotteryNumber + "'",
                "'" + lottery.IntervalStart + "'",
                "'" + lottery.IntervalEnd + "'",
                "'" + handler.PrintEnum(lottery.myStatus) + "'"
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
            return new[]
            {
                "'" + stockListItem.SystemId + "'",
                "'" + stockListItem.Product.SystemId + "'",
                "'" + stockListItem.Quantity + "'",
                "'" + stockListItem.Discount.discountCode + "'",
                "'" + handler.PrintEnum(stockListItem.PurchaseWay) + "'"
            };
        }

        internal Store GetStore(string storeID)
        {
            var dbReader = SelectFromTableWithCondition("Store", "*", "SystemID = " + storeID);
            return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetString(3));
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
        private string[] GetLotteryManagmentStringValues(LotterySaleManagmentTicket lotterySaleManagementTicket)
        {
            return new[]
            {
                 "'" + lotterySaleManagementTicket.SystemID + "'",
                "'" + lotterySaleManagementTicket.Original.SystemId + "'",
                "'" + lotterySaleManagementTicket.ProductNormalPrice + "'",
                "'" + lotterySaleManagementTicket.TotalMoneyPayed + "'",
                "'" + lotterySaleManagementTicket.StartDate + "'",
                "'" + lotterySaleManagementTicket.EndDate + "'",
                "'" + lotterySaleManagementTicket.IsActive + "'"
            };
        }


        public void AddProductToDatabase(Product product)
        {   
            InsertTable("Products", "SystemID, name, BasePrice, description",
                GetProductStringValues(product),GetProductValuesArray(product));
        }
        public void EditProductInDatabase(Product product)
        {
            string[] columnNames = {
                "SystemID",
                "name",
                "BasePrice",
                "description"
            };
            UpdateTable("Products", "SystemID = '"+product.SystemId+"'", columnNames,
                GetProductStringValues(product), GetProductValuesArray(product));
        }

        

        private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
        {
            List<PurchaseHistory> historyData = new List<PurchaseHistory>();
            while (dbReader.Read())
            {
                historyData.Add(new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                    dbReader.GetString(3), dbReader.GetInt32(4),dbReader.GetDouble(5),dbReader.GetString(6)));
            }

            return historyData.ToArray();
        }
        internal StockListItem GetStockListItembyProductID(string product)
        {
            Product _product = GetProductID(product);
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = new StockListItem(0, _product, null, 0, "");
            var dbReader = SelectFromTableWithCondition("Stock", "*", "ProductSystemID = "+_product);
            stockListItem.SystemId = dbReader.GetString(0);
            stockListItem.Quantity = dbReader.GetInt32(2);
            stockListItem.PurchaseWay = handler.GetPurchaseEnumString(dbReader.GetString(4));

            string DiscountCode = dbReader.GetString(3);
            var discountReader = SelectFromTableWithCondition("Discount", "*", "DiscountCode = " + DiscountCode);
            Discount discount = new Discount(DiscountCode, 0, DateTime.Now, DateTime.Now, 0, false);

            
            discount.discountType = handler.GetdiscountTypeEnumString(discountReader.GetString(1));
            discount.startDate = DateTime.Parse(discountReader.GetString(2));
            discount.EndDate = DateTime.Parse(discountReader.GetString(3));
            discount.DiscountAmount = discountReader.GetInt32(4);
            discount.Percentages = discountReader.GetBoolean(5);

            stockListItem.Discount = discount;

            
            return stockListItem;
        }

        internal void AddStore(Store temp)
        {
            InsertTable("Store", "SystemID, Name, Address, IsActive",
                GetStoreStringValues(temp), GetStoreArray(temp));
        }

        internal void AddLotteryTicket(LotteryTicket lottery)
        {
            InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd, Status",
                GetTicketStringValues(lottery), GetTicketValuesArray(lottery));
        }

        internal Product GetProductID(string iD)
        {
            var productReader = SelectFromTableWithCondition("Products", "*", "ProductSystemID = " + iD);
            return new Product(iD, productReader.GetString(1), productReader.GetInt32(2), productReader.GetString(3));
            
        }

        public string[] GetHistory(Store store)
        {
            string[] result;
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + store.SystemId + "'"))
            {
                if (!dbReader.Read())
                {
                    throw new StoreException(ViewPurchaseHistoryStatus.NoStoreFound, "Couldn't find any store with that ID in history records");
                }
                PurchaseHistory[] resultPurchase = GetPurchaseHistory(dbReader);
                result = new String[resultPurchase.Length];
                int i = 0;
                foreach (PurchaseHistory purchaseHistory in resultPurchase)
                {
                    result[i] = purchaseHistory.ToString();
                    i++;
                }
                return result;
            }
        }

        internal void AddDiscount(Discount discount)
        {
            InsertTable("Discount", "DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount,Percentages ",
                GetDiscountStringValues(discount), GetDiscountValuesArray(discount));
        }

        
        internal void AddStockListItemToDataBase(StockListItem stockListItem)
        {
            AddDiscount(stockListItem.Discount);
            AddProductToDatabase(stockListItem.Product);
            InsertTable("Stock", "StockID, ProductSystemID, quantity, discount, PurchaseWay",
                   GetStockListItemStringValues(stockListItem), GetStockListItemArray(stockListItem));
        }

       

        internal void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            DeleteFromTable("LotteryTable", "SystemID = " + lotteryManagment.SystemID);
        }

        internal void RemoveStockListItem(StockListItem stockListItem)
        {
            DeleteFromTable("Stock", "StockID = " + stockListItem.SystemId);
        }

        internal void EditDiscountInDatabase(Discount discount)
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
            UpdateTable("Stock", "DiscountCode = '" + discount.discountCode + "'", columnNames,
                GetDiscountStringValues(discount), GetDiscountValuesArray(discount));
        }

        internal void EditStore(Store store)
        {

            string[] columnNames =
            {
                "SystemID",
                "Name",
                "Address",
                "IsActive",
            };
            UpdateTable("Store", "SystemID = '" + store.SystemId + "'", columnNames,
                GetStoreStringValues(store), GetStoreArray(store));
        }


        

        internal void RemoveProduct(Product product)
        {
            DeleteFromTable("Products", "SystemID" + product.SystemId);
        }

        internal void EditStockInDatabase(StockListItem stockListItem)
        {
            string[] columnNames =
            {
                "StockID",
                "ProductSystemID",
                "quantity",
                "discount",
                "PurchaseWay"
            };
            UpdateTable("Stock", "StockID = '" + stockListItem.SystemId + "'", columnNames,
                GetStockListItemStringValues(stockListItem), GetStockListItemArray(stockListItem));
        }
        

        internal LinkedList<Store> GetAllActiveStores() // all active stores
        {
            LinkedList<Store> result = new LinkedList<Store>();
            var dbReader = SelectFromTableWithCondition("Store", "*", "IsActive = Active");
            while (dbReader.Read())
            {
                Store store = new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetString(3));
                result.AddLast(store);
            }
            return result;
        }

        internal void RemoveDiscount(Discount discount)
        {
            DeleteFromTable("Discount", "DiscountCode = " + discount.discountCode);
        }



        internal void AddLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            InsertTable("LotteryTable", "SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, StartDate,EndDate,isActive ",
                GetLotteryManagmentStringValues(lotteryManagment), GetLotteryManagmentValuesArray(lotteryManagment));

        }

        

        

        internal LinkedList<string> GetAllStoreProductsID(object systemID)
        {
            LinkedList<string> result = new LinkedList<string>();
            var dbReader = SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = " +systemID);
            while (dbReader.Read())
            {
                result.AddLast(dbReader.GetString(0));
            }
            return result;
        }

        internal LotterySaleManagmentTicket GetLotteryByProductID(string productID)
        {
            Product P = GetProductID(productID);
            var dbReader = SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = " + productID);
            LotterySaleManagmentTicket lotteryManagement = new LotterySaleManagmentTicket(dbReader.GetString(0), P, DateTime.Parse(dbReader.GetString(4)), DateTime.Parse(dbReader.GetString(5)));
            lotteryManagement.TotalMoneyPayed = dbReader.GetInt32(3);
            lotteryManagement.IsActive = dbReader.GetBoolean(6);
            return lotteryManagement;
        }

        internal void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment)
        {
            string[] columnNames =
            {
                "SystemID",
                "ProductSystemID",
                "ProductNormalPrice",
                "TotalMoneyPayed",
                "StartDate",
                "EndDate",
                "isActive"
            };
            UpdateTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'", columnNames,
                GetLotteryManagmentStringValues(lotteryManagment), GetLotteryManagmentValuesArray(lotteryManagment));
        }

    }
}