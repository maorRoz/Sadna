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
        private string[] getProductStringValues(Product product)
        {

            string[] Stringvalues = new string[4];
            Stringvalues[0] = "'" + product.SystemId + "'";
            Stringvalues[1] = "'" + product.name + "'";
            Stringvalues[2] = product.BasePrice + "";
            Stringvalues[3] = "'" + product.description + "'";
            return Stringvalues;
        }
        private object[] getProductValuesArray(Product product)
        {
            object[] values = new string[4];
            values[0] = product.SystemId;
            values[1] = product.name;
            values[2] = product.BasePrice;
            values[3] = product.description;
            return values;
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
        public String[] getHistory(Store store)
        {
            String[] result;
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

        internal void EditDiscountInDatabase(Discount discount)
        {
            throw new NotImplementedException();
        }

        internal void EditStockInDatabase(Stock.StockListItem stockListItem)
        {
            throw new NotImplementedException();
        }
    }
}
