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
    class StoresSyncherHarmony : IStoresSyncher
    {

        private OutsideModuleService _storeService;
        public StoresSyncherHarmony()
        {
            _storeService = ModuleGlobalHandler.GetInstance();
        }

        public void RemoveProducts(OrderItem[] purchased)
        {
            foreach (OrderItem item in purchased)
            {
                _storeService.UpdateQuantityAfterPurchase(item.Store, item.Name, item.Quantity);
            }
        }

        public void UpdateLottery(string itemName, string store, string username)
        {
            throw new NotImplementedException();
        }


        public bool IsValid(OrderItem toBuy)
        {
            return _storeService.ProductExistsInQuantity(toBuy.Store, toBuy.Name, toBuy.Quantity);
        }

        public bool IsTicketValid(string itemName, string store)
        {
            throw new NotImplementedException();
        }

        public OrderItem GetItemFromCoupon(string itemName, string store, int quantity, string coupon)
        {
            throw new NotImplementedException();
        }
    }
}
