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
        public void makeAProductToData(Product product)
        {
            
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
    }
}
