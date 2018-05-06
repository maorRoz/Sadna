using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    class EditItemPolicySlave : AbstractStoreCenterSlave
    {
        public StoreAnswer answer;

        public EditItemPolicySlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public PurchasePolicy EditPolicy(string product, int newAmount, bool max)
        {
            MarketLog.Log("StoreCenter", "trying to edit policy of store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                StockListItem item = GetItem(product);
                CheckAmount(newAmount);
                PurchasePolicy policy = item.Policy;
                if (max)
                    policy._maxAmount = newAmount;
                else
                    policy._minAmount = newAmount;
                DataLayerInstance.EditPurchasePolicy(item, policy);
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

        public PurchasePolicy AddPolicyConstraints(string product, int type, string value)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                StockListItem item = GetItem(product);
                PurchasePolicy policy = item.Policy;
                switch (type)
                {
                    case 1:
                        policy.AddBlocked(value);
                        break;
                    case 2:
                        policy.Addbypass(value);
                        break;
                    case 3:
                        policy.AddRelevant(value);
                        break;
                }
                DataLayerInstance.EditPurchasePolicy(item, policy);
                MarketLog.Log("StoreCenter", "purchase policy updated successfully");
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

        public PurchasePolicy RemovePolicyConstraints(string product, int type, string value)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                StockListItem item = GetItem(product);
                PurchasePolicy policy = item.Policy;
                switch (type)
                {
                    case 1:
                        policy.RemoveBlocked(value);
                        break;
                    case 2:
                        policy.Removebypass(value);
                        break;
                    case 3:
                        policy.RemoveRelevant(value);
                        break;
                }
                DataLayerInstance.EditPurchasePolicy(item, policy);
                MarketLog.Log("StoreCenter", "purchase policy updated successfully");
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

        private StockListItem GetItem(string product)
        {
            checkIfStoreExistsAndActive();
            MarketLog.Log("StoreCenter", " store exists");
            MarketLog.Log("StoreCenter", " check if has premmision to edit store");
            _storeManager.CanDeclarePurchasePolicy();
            MarketLog.Log("StoreCenter", " has premmission");
            MarketLog.Log("StoreCenter", " get store " + _storeName);
            Store store = DataLayerInstance.GetStorebyName(_storeName);
            MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
            return DataLayerInstance.GetProductFromStore(_storeName, product);
        }
    }
}
