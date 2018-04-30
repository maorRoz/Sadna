using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal abstract class AbstractSlave
    {
        /**
         * many of the slaves in the Store Managment use same methods and hold smae attrebutes. 
         * in oreder to avoid double-code I added them here
         **/
        protected string _storeName;
        protected I_StoreDL global;
        protected IUserSeller _storeManager;
        public AbstractSlave(string storeName, IUserSeller storeManager)
        {
            _storeName = storeName;
            global = StoreDL.GetInstance();
            _storeManager = storeManager;
        }
        protected void checkIfStoreExistsAndActive()
        {
            if (!global.IsStoreExistAndActive(_storeName))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
        }

        protected void IsProductNameAvailableInStore(string name)
        {

            Product P = global.getProductByNameFromStore(_storeName, name);
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