using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class OpenStoreSlave
    {
        public MarketAnswer answer { get; set; }
        IUserShopper _shopper;
        StoreDL storeLogic;
        private int StoreIdCounter;
        public OpenStoreSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = StoreDL.Instance;
            StoreIdCounter = storeLogic.FindMaxStoreId();
        }
        public Store OpenStore(string storeName, string address)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add new store");
                _shopper.ValidateRegistered();
                MarketLog.Log("StoreCenter", "premission gained");
                Store newStore = new Store(GetNextStoreId(), storeName, address);
                storeLogic.AddStore(newStore);
                MarketLog.Log("StoreCenter", "store was opened");
                _shopper.AddOwnership(storeName);
                MarketLog.Log("StoreCenter", "add myself to the store list");
                answer = new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
                return newStore;
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "error in opening store");
                answer = new StoreAnswer((OpenStoreStatus)e.Status, "Store " + storeName + " creation has been denied. " +
                                                 "something is wrong with adding a new store of that type. Error message has been created!");
                return null;
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(OpenStoreStatus.InvalidUser,
                    "User validation as store owner has been failed. only registered users can open new stores. Error message has been created!");
                return null;
            }
        }
        public string GetNextStoreId()
        {
            int currentMaxStoreId = StoreIdCounter;
            StoreIdCounter++;
            return "S" + currentMaxStoreId;
        }
    }
}
