
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

        public StockListItem AddNewProduct(string _name, double _price, string _description, int quantity)
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
                CheckIfProductNameAvailable(_name); 
                MarketLog.Log("StoreCenter", " name is avlaiable");
                MarketLog.Log("StoreCenter", " checking that quanitity is positive");
                CheckQuantityIsOK(quantity);
                MarketLog.Log("StoreCenter", " quanitity is positive");
                Product product = new Product(_name, _price, _description);
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, store.SystemId);
                DataLayerInstance.AddStockListItemToDataBase(stockListItem);
                MarketLog.Log("StoreCenter", "product added");
                answer = new StoreAnswer(StoreEnum.Success, "product added");
                return stockListItem;
            }
            catch (StoreException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status,"Product was not added!");
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            return null;
        }

        private void CheckIfProductNameAvailable(string name)
        {
            Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, name);
            if (product != null)
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "product name must be uniqe per shop");
        }

        private void CheckQuantityIsOK(int quantity)
        {
            if (quantity <= 0)
            { throw new StoreException(StoreEnum.QuantityIsNegative, "negative quantity"); }
        }

    }
}