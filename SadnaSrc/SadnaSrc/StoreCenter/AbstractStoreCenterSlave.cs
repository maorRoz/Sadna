using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal abstract class AbstractStoreCenterSlave
    {
        /**
         * many of the slaves in the Store Managment use same methods and hold smae attrebutes. 
         * in oreder to avoid double-code I added them here
         **/
        protected string _storeName;
        protected I_StoreDL DataLayerInstance;
        protected IUserSeller _storeManager;
        protected AbstractStoreCenterSlave(string storeName, IUserSeller storeManager,I_StoreDL storeDL )
        {
            _storeName = storeName;
            DataLayerInstance = storeDL;
            _storeManager = storeManager;
        }
        protected void checkIfStoreExistsAndActive()
        {
            if (!DataLayerInstance.IsStoreExistAndActive(_storeName))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
        }

        protected void IsProductNameAvailableInStore(string name)
        {

            Product P = DataLayerInstance.GetProductByNameFromStore(_storeName, name);
            if (P == null)
            { throw new StoreException(StoreEnum.ProductNotFound, "Product Name is already Exists In Shop"); }
        }
        protected void checkifProductExists(Product product)
        {
            if (product == null)
            {
                MarketLog.Log("StoreCenter", "product not exists");
                throw new StoreException(StoreEnum.ProductNotFound, "no Such Product");
            }
            MarketLog.Log("StoreCenter", "product exists");
        }
    }
}