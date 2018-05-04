using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class OpenStoreSlave
    {
        public MarketAnswer Answer { get; private set; }
        IUserShopper _shopper;
        IStoreDL storeLogic;
        public OpenStoreSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
        }
        public Store OpenStore(string storeName, string address)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add new store");
                _shopper.ValidateRegistered();
                MarketLog.Log("StoreCenter", "premission gained");
                CheckIfNameAvailable(storeName);
                Store newStore = new Store(storeName, address);
                storeLogic.AddStore(newStore);
                MarketLog.Log("StoreCenter", "store was opened");
                _shopper.AddOwnership(storeName);
                MarketLog.Log("StoreCenter", "add myself to the store list");
                Answer = new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
                return newStore;
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "error in opening store");
                Answer = new StoreAnswer((OpenStoreStatus)e.Status, "Store " + storeName + " creation has been denied. " +
                                                 "something is wrong with adding a new store of that type. Error message has been created!");
                return null;
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                Answer = new StoreAnswer(OpenStoreStatus.InvalidUser,
                    "User validation as store owner has been failed. only registered users can open new stores. Error message has been created!");
                return null;
            }
        }

        private void CheckIfNameAvailable(string name)
        {
            var store = storeLogic.GetStorebyName(name);
            if (store != null)
                throw new StoreException(OpenStoreStatus.AlreadyExist, "store name must be uniqe");
        }
    }
}
