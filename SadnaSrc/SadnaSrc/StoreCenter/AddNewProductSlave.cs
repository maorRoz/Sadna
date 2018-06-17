
using Castle.Core.Internal;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddNewProductSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        public AddNewProductSlave(IUserSeller storeManager, string storeName, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public StockListItem AddNewProduct(string name, double price, string description, int quantity)
        {

            try
            {
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                MarketLog.Log("StoreCenter", "trying to add product to store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to add products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name avlaiable in the store" + _storeName);
                CheckInput(name, price, description, quantity);
                MarketLog.Log("StoreCenter", " Input is valid.");
                Product product = new Product(name, price, description);
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, store.SystemId);
                DataLayerInstance.AddStockListItemToDataBase(stockListItem);
                MarketLog.Log("StoreCenter", "product added");
                answer = new StoreAnswer(StoreEnum.Success, "product added");
                return stockListItem;
            }
            catch (StoreException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            return null;
        }

        private void CheckInput(string name, double price, string description, int quantity)
        {
            if(name.IsNullOrEmpty() | description.IsNullOrEmpty())
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Invalid Name or Description");
            Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, name);
            if (product != null)
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Product name must be uniqe per shop");
            if (quantity <= 0)
                throw new StoreException(StoreEnum.QuantityIsNegative, "Invalid quantity"); 
            if (price <= 0)
                throw new StoreException(StoreEnum.QuantityIsNegative, "Invalid price"); 

        }

    }
}