using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class StoreDL : SystemDL
    {

        private object[] getStockListItemArray(Stock.StockListItem stockListItem)
        {
            object[] values = new object[5];
            values[0] = stockListItem.SystemId;
            values[1] = stockListItem.product;
            values[2] = stockListItem.quantity;
            values[3] = stockListItem.discount;
            values[4] = stockListItem.PurchaseWay;
            return values;
        }
        private object[] getProductValuesArray(Product product)
        {
            object[] values = new object[4];
            values[0] = product.SystemId;
            values[1] = product.name;
            values[2] = product.BasePrice;
            values[3] = product.description;
            return values;
        }
        private object[] getTicketValuesArray(LotteryTicket lottery)
        {

            object[] values = new object[5];
            values[0] = lottery.myID;
            values[1] = lottery.LotteryNumber;
            values[2] = lottery.IntervalStart;
            values[3] = lottery.IntervalEnd;
            values[4] = lottery.myStatus;
            return values;
        }
        private object[] getDiscountValuesArray(Discount discount)
        {
            object[] values = new object[6];
            values[0] = discount.discountCode;
            values[1] = discount.discountType;
            values[2] = discount.startDate;
            values[3] = discount.EndDate;
            values[4] = discount.DiscountAmount;
            values[5] = discount.Percentages;
            return values;
        }
        private object[] getLotteryManagmentValuesArray(LotterySaleManagmentTicket lSMT)
        {
            object[] values = new object[7];
            values[0] = lSMT.SystemID;
            values[1] = lSMT.original.SystemId;
            values[2] = lSMT.ProductNormalPrice;
            values[3] = lSMT.TotalMoneyPayed;
            values[4] = lSMT.StartDate;
            values[5] = lSMT.EndDate;
            values[6] = lSMT.isActive;
            return values;
        }

        private string[] getTicketStringValues(LotteryTicket lottery)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            string[] Stringvalues = new string[5];
            Stringvalues[0] = "'" + lottery.myID + "'";
            Stringvalues[1] = "'" + lottery.LotteryNumber + "'";
            Stringvalues[2] = "'" + lottery.IntervalStart + "'";
            Stringvalues[3] = "'" + lottery.IntervalEnd + "'";
            Stringvalues[4] = "'" + handler.PrintEnum(lottery.myStatus) + "'";
            return Stringvalues;
        }
        private string[] getDiscountStringValues(Discount discount)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            string[] Stringvalues = new string[6];
            Stringvalues[0] = "'" + discount.discountCode + "'";
            Stringvalues[1] = "'" + handler.PrintEnum(discount.discountType) + "'";
            Stringvalues[2] = "'" + discount.startDate + "'";
            Stringvalues[3] = "'" + discount.EndDate + "'";
            Stringvalues[4] = "'" + discount.DiscountAmount + "'";
            Stringvalues[5] = "'" + discount.Percentages + "'";
            return Stringvalues;
        }
        private string[] getProductStringValues(Product product)
        {

            string[] Stringvalues = new string[4];
            Stringvalues[0] = "'" + product.SystemId + "'";
            Stringvalues[1] = "'" + product.name + "'";
            Stringvalues[2] = product.BasePrice + "";
            Stringvalues[3] = "'" + product.description + "'";
            return Stringvalues;
        }
        private string[] getStockListItemStringValues(Stock.StockListItem stockListItem)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            string[] Stringvalues = new string[5];
            Stringvalues[0] = "'" + stockListItem.SystemId + "'";
            Stringvalues[1] = "'" + stockListItem.product.SystemId + "'";
            Stringvalues[2] = "'" + stockListItem.quantity + "'";
            Stringvalues[3] = "'" + stockListItem.discount.discountCode + "'";
            Stringvalues[4] = "'" + handler.PrintEnum(stockListItem.PurchaseWay) + "'";
            return Stringvalues;
        }

        private string[] getLotteryManagmentStringValues(LotterySaleManagmentTicket lSMT)
        {
            string[] Stringvalues = new string[7];
            Stringvalues[0] = "'" + lSMT.SystemID + "'";
            Stringvalues[1] = "'" + lSMT.original.SystemId + "'";
            Stringvalues[2] = "'" + lSMT.ProductNormalPrice + "'";
            Stringvalues[3] = "'" + lSMT.TotalMoneyPayed + "'";
            Stringvalues[4] = "'" + lSMT.StartDate + "'";
            Stringvalues[5] = "'" + lSMT.EndDate + "'";
            Stringvalues[6] = "'" + lSMT.isActive + "'";
            return Stringvalues;
        }


        public void AddProductToDatabase(Product product)
        {   
            InsertTable("Products", "SystemID, name, BasePrice, description",
                getProductStringValues(product),getProductValuesArray(product));
        }
        public void EditProductInDatabase(Product product)
        {
            string[] columnNames = new string[4];
            columnNames[0] = "SystemID";
            columnNames[1] = "name";
            columnNames[2] = "BasePrice";
            columnNames[3] = "description";
            UpdateTable("Products", "SystemID = '"+product.SystemId+"'", columnNames,
                getProductStringValues(product), getProductValuesArray(product));
        }

        

        private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
        {
            List<PurchaseHistory> historyData = new List<PurchaseHistory>();
            while (dbReader.Read())
            {
                historyData.Add(new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                    dbReader.GetString(3), dbReader.GetString(4)));
            }

            return historyData.ToArray();
        }
        internal Stock.StockListItem getStockListItembyProductID(string product)
        {
            Product P = getProductID(product);
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Stock.StockListItem SLI = new Stock.StockListItem(0, P, null, 0, "");
            var dbReader = SelectFromTableWithCondition("Stock", "*", "ProductSystemID = "+product);
            SLI.SystemId = dbReader.GetString(0);
            SLI.quantity = dbReader.GetInt32(2);
            SLI.PurchaseWay = handler.GetPurchaseEnumString(dbReader.GetString(4));

            string DiscountCode = dbReader.GetString(3);
            var discountReader = SelectFromTableWithCondition("Discount", "*", "DiscountCode = " + DiscountCode);
            Discount discount = new Discount(DiscountCode, 0, DateTime.Now, DateTime.Now, 0, false);

            
            discount.discountType = handler.GetdiscountTypeEnumString(discountReader.GetString(1));
            discount.startDate = DateTime.Parse(discountReader.GetString(2));
            discount.EndDate = DateTime.Parse(discountReader.GetString(3));
            discount.DiscountAmount = discountReader.GetInt32(4);
            discount.Percentages = discountReader.GetBoolean(5);

            SLI.discount = discount;

            
            return SLI;
        }
        

        internal void addLotteryTicket(LotteryTicket lottery)
        {
            InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd, Status",
                getTicketStringValues(lottery), getTicketValuesArray(lottery));
        }

        internal Product getProductID(string iD)
        {
            var ProductReader = SelectFromTableWithCondition("Products", "*", "ProductSystemID = " + iD);
            Product P = new Product(iD, ProductReader.GetString(1), ProductReader.GetInt32(2), ProductReader.GetString(3));
            return P;
            
        }

        public string[] getHistory(Store store)
        {
            string[] result;
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + store.SystemId + "'"))
            {
                if (!dbReader.Read())
                {
                    throw new StoreException(ViewPurchaseHistoryStatus.NoStoreFound, "Couldn't find any store with that ID in history records");
                }
                PurchaseHistory[] resultP = GetPurchaseHistory(dbReader);
                result = new String[resultP.Length];
                int i = 0;
                foreach (PurchaseHistory ph in resultP)
                {
                    result[i] = ph.ToString();
                    i++;
                }
                return result;
            }
        }

        internal void addDiscount(Discount discount)
        {
            InsertTable("Discount", "DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount,Percentages ",
                getDiscountStringValues(discount), getDiscountValuesArray(discount));
        }

        
        internal void AddStockListItemToDataBase(Stock.StockListItem stockListItem)
        {
            addDiscount(stockListItem.discount);
            AddProductToDatabase(stockListItem.product);
            InsertTable("Stock", "StockID, ProductSystemID, quantity, discount, PurchaseWay",
                   getStockListItemStringValues(stockListItem), getStockListItemArray(stockListItem));
        }

       

        internal void removeLottery(LotterySaleManagmentTicket LSMT)
        {
            DeleteFromTable("LotteryTable", "SystemID = " + LSMT.SystemID);
        }

        internal void removeStockListItem(Stock.StockListItem SLI)
        {
            DeleteFromTable("Stock", "StockID = " + SLI.SystemId);
        }

        internal void EditDiscountInDatabase(Discount discount)
        {
            string[] columnNames = new string[6];
            columnNames[0] = "DiscountCode";
            columnNames[1] = "DiscountType";
            columnNames[2] = "StartDate";
            columnNames[3] = "EndDate";
            columnNames[4] = "DiscountAmount";
            columnNames[5] = "Percentages";
            UpdateTable("Stock", "DiscountCode = '" + discount.discountCode + "'", columnNames,
                getDiscountStringValues(discount), getDiscountValuesArray(discount));
        }
        

        internal void removeProduct(Product product)
        {
            DeleteFromTable("Products", "SystemID" + product.SystemId);
        }

        internal void EditStockInDatabase(Stock.StockListItem stockListItem)
        {
            string[] columnNames = new string[5];
            columnNames[0] = "StockID";
            columnNames[1] = "ProductSystemID";
            columnNames[2] = "quantity";
            columnNames[3] = "discount";
            columnNames[4] = "PurchaseWay";
            UpdateTable("Stock", "StockID = '" + stockListItem.SystemId + "'", columnNames,
                getStockListItemStringValues(stockListItem), getStockListItemArray(stockListItem));
        }
        

        internal static LinkedList<Store> getAllActiveStores() // all active stores
        {
            throw new NotImplementedException();
        }

        internal void removeDiscount(Discount discount)
        {
            DeleteFromTable("Discount", "DiscountCode = " + discount.discountCode);
        }



        internal void AddLottery(LotterySaleManagmentTicket LSMT)
        {
            InsertTable("LotteryTable", "SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, StartDate,EndDate,isActive ",
                getLotteryManagmentStringValues(LSMT), getLotteryManagmentValuesArray(LSMT));

        }

        

        

        internal LinkedList<string> getAllStoreProductsID(object systemID)
        {
            LinkedList<string> result = new LinkedList<string>();
            var dbReader = SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = " +systemID);
            //foreach ()
            //result.AddLast(dbReader.ToString(0));
            throw new NotImplementedException();
        }

        internal LotterySaleManagmentTicket getLotteryByProductID(string productID)
        {
            Product P = getProductID(productID);
            var dbReader = SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = " + productID);
            LotterySaleManagmentTicket LSMT = new LotterySaleManagmentTicket(dbReader.GetString(0), P, DateTime.Parse(dbReader.GetString(4)), DateTime.Parse(dbReader.GetString(5)));
            LSMT.TotalMoneyPayed = dbReader.GetInt32(3);
            LSMT.isActive = dbReader.GetBoolean(6);
            return LSMT;
        }

        internal void editLotteryInDatabase(LotterySaleManagmentTicket LSMT)
        {
            string[] columnNames = new string[7];
            columnNames[0] = "SystemID";
            columnNames[1] = "ProductSystemID";
            columnNames[2] = "ProductNormalPrice";
            columnNames[3] = "TotalMoneyPayed";
            columnNames[4] = "StartDate";
            columnNames[5] = "EndDate";
            columnNames[6] = "isActive";
            UpdateTable("LotteryTable", "SystemID = '" + LSMT.SystemID + "'", columnNames,
                getLotteryManagmentStringValues(LSMT), getLotteryManagmentValuesArray(LSMT));
        }

    }
}