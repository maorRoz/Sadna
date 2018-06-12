using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace SadnaSrc.StoreCenter
{
    public class OpenStoreSlave
    {
        public MarketAnswer Answer { get; private set; }
        IUserShopper _shopper;
        IStoreDL storeDB;
        public OpenStoreSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeDB = storeDL;
        }
        public Store OpenStore(string storeName, string address)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add new store");
                _shopper.ValidateRegistered();
                MarketLog.Log("StoreCenter", "premission gained");
                CheckIfNameAvailable(storeName);
				CheckIfDataValid(address);
				MarketLog.Log("StoreCenter", "data is valid");
				Store newStore = new Store(storeName, address);
                storeDB.AddStore(newStore);
                MarketLog.Log("StoreCenter", "store was opened");
                _shopper.AddOwnership(storeName);
                storeDB.AddPromotionHistory(storeName,_shopper.GetShopperName(),_shopper.GetShopperName(),new[]{"StoreOwner"},storeName +" has been opened");
                Answer = new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
                return newStore;
            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer((OpenStoreStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(OpenStoreStatus.InvalidUser,
                    "User validation as store owner has been failed. only registered users can open new stores.");
            }
            catch (DataException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            return null;
        }

        private void CheckIfNameAvailable(string name)
        {
            var store = storeDB.GetStorebyName(name);
            if (store != null)
                throw new StoreException(OpenStoreStatus.AlreadyExist, "store name must be uniqe");
        }

		private void CheckIfDataValid(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new StoreException(OpenStoreStatus.InvalidData, "store address is invalid");
			}
		}
	}
}
