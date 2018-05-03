using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddQuanitityToProductSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;

        public AddQuanitityToProductSlave(string storeName, IUserSeller storeManager, I_StoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public void AddQuanitityToProduct(string productName, int quantity)
        {
            try
            {
                MarketLog.Log("StoreCenter", "checking that store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", "checking that has premmisions");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", "checking that Product Exists");
                checkifProductExists(DataLayerInstance.GetProductByNameFromStore(_storeName, productName));
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
                MarketLog.Log("StoreCenter", "checking that quantity is positive");
                CheckIfQuanityIsOK(quantity);
                stockListItem.Quantity += quantity;
                DataLayerInstance.EditStockInDatabase(stockListItem);
                answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " added by amound of " + quantity);
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                answer = new StoreAnswer(StoreEnum.NoPremmision, e.GetErrorMessage());
            }
        }

        private void CheckIfQuanityIsOK(int quantity)
        {
            if (quantity <= 0)
            {
                MarketLog.Log("StoreCenter", "quantity is not positive");
                throw new StoreException(StoreEnum.quantityIsNegatie, "quantity " + quantity + " is less then or equal to 0");
            }
        }
    }
}