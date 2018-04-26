using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class PromoteToStoreManagerSlave
    {
        internal MarketAnswer answer;
        private IUserSeller _storeManager;
        public string _storeName;
        StoreDL global;
        public PromoteToStoreManagerSlave(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            _storeName = storeName;
            global = StoreDL.Instance;
        }
        internal void PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to grant " + someoneToPromoteName +
                                      " manager options in Store" + _storeName + ". Validating store activity and existence..");
            try
            {
                global.ValidateStoreExists(_storeName);
                ValidatePromotionEligible(actions);
                _storeManager.ValidateNotPromotingHimself(someoneToPromoteName);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has been authorized. granting " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "...");
                _storeManager.Promote(someoneToPromoteName, actions);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " granted " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "successfully");
                answer = new StoreAnswer(PromoteStoreStatus.Success, "promote with manager options has been successful!");

            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to promote others in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                answer = new StoreAnswer(PromoteStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to promote " + someoneToPromoteName +
                                  "with manager options in Store" + _storeName + " and therefore has been denied. Error message has been created!");
                answer = new StoreAnswer((PromoteStoreStatus)e.Status, e.GetErrorMessage());
            }

        }
        private void ValidatePromotionEligible(string actions)
        {
            if (actions.Contains("StoreOwner"))
            {
                _storeManager.CanPromoteStoreOwner();
            }
            else
            {
                _storeManager.CanPromoteStoreAdmin();
            }
        }
    }
}