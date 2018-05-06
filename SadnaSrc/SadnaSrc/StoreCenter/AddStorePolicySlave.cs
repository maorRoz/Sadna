using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    class AddStorePolicySlave : AbstractStoreCenterSlave
    {
        public StoreAnswer answer;

        public AddStorePolicySlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public PurchasePolicy AddPolicy(int minAmount, int maxAmount)
        {
            MarketLog.Log("StoreCenter", "trying to add policy to store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit store");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if store " + _storeName + "exists");
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                MarketLog.Log("StoreCenter", "check that amounts are OK");
                CheckAmounts(minAmount, maxAmount);
                PurchasePolicy policy = new PurchasePolicy(_storeName, minAmount, maxAmount);
                store.Policy = policy;
                DataLayerInstance.AddPurchasePolicy(store, policy);
                MarketLog.Log("StoreCenter", "purchase policy added successfully");
                answer = new StoreAnswer(StoreEnum.Success, "purchase policy added successfully");
                return policy;
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
                return null;
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
                return null;
            }
        }

        private static void CheckAmounts(int minAmount, int maxAmount)
        {
            if (minAmount < 0 || maxAmount < 0)
            {
                MarketLog.Log("StoreCenter", "min/max amount is negative");
                throw new StoreException(StoreEnum.QuantityIsNegative, "Can't declare purchase policy with negative amount");
            }
        }
    }
}
