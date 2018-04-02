using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    /**
     * this class handles the stores, Adding Stores, and so on. 
     * it will hold the connection to the DB
     * it will hold the logger
     * this is the gateway from the system to the StoreCenter Packege
     **/
    class StoreService : IStoreService
    {
        LinkedList<Store> allStores;
        int StoreIdCounter;
        int globalDiscountCode;

        public StoreService()
        {
            StoreIdCounter = 0;
            globalDiscountCode = 0;
            allStores = new LinkedList<Store>();
        }

        public MarketAnswer OpenStore(User owner)
        {
            Store temp = new Store(owner, getNextStoreId());
            allStores.AddLast(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }

        private int getNextStoreId()
        {
            int temp = StoreIdCounter;
            StoreIdCounter++;
            return temp;
        }

        private LinkedList<Store> getStoresOfUser(User user)
        {
            LinkedList<Store> result = new LinkedList<Store>();
            foreach (Store store in allStores)
            {
                if (store.IsOwner(user) && store.isStoreActive())
                {
                    result.AddLast(store);
                }
            }
            if (result.Count != 0)
                return result;
            return null;
        }
        public MarketAnswer PromoteToOwner(Store store, User CurrentUser, User someoneToPromote)
        {
            LinkedList<Store> temp = getStoresOfUser(CurrentUser);
            if (temp == null)
            {
                return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + CurrentUser.SystemID + " has no active Store");
            }
            else
            {
                if (temp.Contains(store))
                {
                    return store.PromoteToOwner(someoneToPromote);
                }
                else
                {
                    return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + CurrentUser.SystemID + " is not a Store Owner of the store" + store.SystemId);
                }
            }
        }
        public MarketAnswer PromoteToManager(Store store, User CurrentUser, User someoneToPromote)
        {
            LinkedList<Store> temp = getStoresOfUser(CurrentUser);
            if (temp == null)
            {
                return new StoreAnswer(StoreEnum.AddStoreManagerFail, "user " + CurrentUser.SystemID + " has no Store");
            }
            else
            {
                if (temp.Contains(store))
                {
                    return store.PromoteToManager(someoneToPromote);
                }
                else
                {
                    return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + CurrentUser.SystemID + " is not a Store Owner of the store" + store.SystemId);
                }
            }
        }
        public MarketAnswer CloseStore(Store store)
        {
            return store.CloseStore();
        }

        public MarketAnswer ChangeProductPurchesWayToImmidiate(Store store, Product product)
        {
            return store.ChangeProductPurchesWayToImmidiate(product);
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct(product, getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, true);
        }
        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct(product, getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, true);
        }
        public MarketAnswer addDiscountToProduct_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct(product, getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, false);
        }
        public MarketAnswer addDiscountToProduct_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct(product, getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, false);
        }
        public MarketAnswer removeDiscountToProduct(Store store, Product product)
        {
            return store.removeDiscountToProduct(product);
        }
        public MarketAnswer removeProduct(Store store, Product product) { return store.removeProduct(product); }
        private int getDiscountCode()
        {
            int temp = globalDiscountCode;
            globalDiscountCode++;
            return temp;
        }

        public MarketAnswer ChangeProductPurchesWayToLottery(Store store, Product product)
        {
            return store.ChangeProductPurchesWayToLottery(product);
        }

        /**  public MarketAnswer CloseStore(User CurrentUser)             maybe to remove this one? ask Zohar
          {
              Store temp = getStoreByUser(CurrentUser);
              if (temp != null)
              {
                  return temp.CloseStore();
              }
              return new StoreAnswer(StoreEnum.CloseStoreFail, "user " + CurrentUser.SystemID + " has no Store");
          }**/
    }
}
