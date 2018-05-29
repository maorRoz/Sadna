using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ViewPromotionHistorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        public ViewPromotionHistorySlave(string _store, IUserSeller storeManager, IStoreDL storeDL) : base(_store, storeManager, storeDL)
        {
            
        }

        public void ViewPromotionHistory()
        {
            try
            {
                MarketLog.Log("StoreCenter", "Trying to view promotion history in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", "Validate that can watch promotion history of that store");
                _storeManager.CanPromoteStoreOwner();
                MarketLog.Log("StoreCenter", "Retreiving the store promotion history records");
                var historyRecords = DataLayerInstance.GetPromotionHistory(_storeName);
                MarketLog.Log("StoreCenter", "'View Promotion History' has been successfully done on store '"+_storeName+"'");
                Answer = new StoreAnswer(StoreEnum.Success,
                    "Successfully has been retrived the store promotion history records",historyRecords);

            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }

        }
    }
}
