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
        ModuleGlobalHandler storeLogic;
        public OpenStoreSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = ModuleGlobalHandler.GetInstance();
        }
        public Store OpenStore(string storeName, string address)
        {
            MarketLog.Log("StoreCenter", "trying to add new store");
                _shopper.ValidateRegistered();
                MarketLog.Log("StoreCenter", "premission gained");
                Store newStore = new Store(storeLogic.GetNextStoreId(), storeName, address);
                storeLogic.AddStore(newStore);
                MarketLog.Log("StoreCenter", "store was opened");
                _shopper.AddOwnership(storeName);                
                MarketLog.Log("StoreCenter", "add myself to the store list");
                answer = new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
                return newStore;
        }
    }
}
