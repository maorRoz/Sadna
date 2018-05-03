using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class PromoteToStoreManagerSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        public PromoteToStoreManagerSlave(IUserSeller storeManager, string storeName, I_StoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }
        public void PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to grant " + someoneToPromoteName +
                                      " manager options in Store" + _storeName + ". Validating store activity and existence..");
            try
            {
                checkIfStoreExistsAndActive();   
                ValidatePromotionEligible(actions);
                _storeManager.ValidateNotPromotingHimself(someoneToPromoteName);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has been authorized. granting " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "...");
                _storeManager.Promote(someoneToPromoteName, actions);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " granted " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "successfully");
                Answer = new StoreAnswer(PromoteStoreStatus.Success, "promote with manager options has been successful!");

            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to promote others in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                Answer = new StoreAnswer(PromoteStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to promote " + someoneToPromoteName +
                                  "with manager options in Store" + _storeName + " and therefore has been denied. Error message has been created!");
                Answer = new StoreAnswer((PromoteStoreStatus)e.Status, e.GetErrorMessage());
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