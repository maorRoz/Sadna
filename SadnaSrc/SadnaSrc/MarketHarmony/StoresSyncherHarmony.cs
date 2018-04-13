using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.MarketHarmony
{
    //TODO: improve this class igor/lior/zohar!!!
    public class StoresSyncherHarmony : IStoresSyncher
    {

        private OutsideModuleService _storeService;
        public StoresSyncherHarmony()
        {
            _storeService = ModuleGlobalHandler.GetInstance();
        }

        public void CloseLottery(string lottery)
        {
            //TODO: implement once the implementation in the StoreCenter module is complete
            throw new NotImplementedException();
        }

        public void RemoveProducts(OrderItem[] purchased)
        {
            foreach (OrderItem item in purchased)
            {
                _storeService.UpdateQuantityAfterPurchase(item.Store, item.Name, item.Quantity);
            }
        }

        public bool IsValid(OrderItem toBuy)
        {
            return _storeService.ProductExistsInQuantity(toBuy.Store, toBuy.Name, toBuy.Quantity);
        }
    }
}
