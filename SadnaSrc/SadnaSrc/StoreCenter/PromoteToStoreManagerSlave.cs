using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class PromoteToStoreManagerSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        public PromoteToStoreManagerSlave(IUserSeller storeManager, string storeName, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }
        public void PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            try
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to grant " + someoneToPromoteName +
                                             " manager options in Store" + _storeName + ". Validating store activity and existence..");
                checkIfStoreExistsAndActive();   
                ValidatePromotionEligible(actions);
                _storeManager.ValidateNotPromotingHimself(someoneToPromoteName);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has been authorized. granting " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "...");
                var appliedPermissions = _storeManager.Promote(someoneToPromoteName, actions);
                DataLayerInstance.AddPromotionHistory(_storeName,_storeManager.GetName(), someoneToPromoteName,appliedPermissions,"Regular promotion");
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " granted " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "successfully");
                Answer = new StoreAnswer(PromoteStoreStatus.Success, "promote with manager options has been successful!");

            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer(PromoteStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new StoreAnswer((PromoteStoreStatus)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
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