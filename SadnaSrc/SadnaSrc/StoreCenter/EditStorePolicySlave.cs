using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    class EditStorePolicySlave : AbstractStoreCenterSlave
    {
        public StoreAnswer answer;

        public EditStorePolicySlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public PurchasePolicy EditPolicy(int newAmount, bool max)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                MarketLog.Log("StoreCenter", "check that amounts are OK");
                CheckAmount(newAmount);
                PurchasePolicy policy = store.Policy;
                if (max)
                    policy._maxAmount = newAmount;
                else
                    policy._minAmount = newAmount;
                DataLayerInstance.EditPurchasePolicy(store, policy);
                MarketLog.Log("StoreCenter", "purchase policy added successfully");
                answer = new StoreAnswer(StoreEnum.Success, "purchase policy updated successfully");
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

        private static void CheckAmount(int newAmount)
        {
            if (newAmount < 0)
            {
                MarketLog.Log("StoreCenter", "min/max amount is negative");
                throw new StoreException(StoreEnum.QuantityIsNegative, "Can't declare purchase policy with negative amount");
            }
        }
    }
}
