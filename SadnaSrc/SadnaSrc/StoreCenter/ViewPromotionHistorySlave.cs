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
                checkIfStoreExistsAndActive();
               

            }
            catch (StoreException e)
            {
               
            }
            catch (MarketException e)
            {
               
            }
            catch (DataException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }

        }
    }
}
