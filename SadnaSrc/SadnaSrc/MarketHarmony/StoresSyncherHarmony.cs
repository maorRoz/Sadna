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

        public void RemoveProducts(OrderItem[] purchased)
        {
            foreach (OrderItem item in purchased)
            {
                _storeService.UpdateQuantityAfterPurchase(item.Store, item.Name, item.Quantity);
            }
        }

        public void UpdateLottery(string itemName, string store,double moenyPayed, string username)
        {
            ModuleGlobalHandler globalHandler = ModuleGlobalHandler.GetInstance();
            globalHandler.updateLottery(itemName, store, moenyPayed, username);
        }


        public bool IsValid(OrderItem toBuy)
        {
            if(toBuy.Quantity > 0)
                return _storeService.ProductExistsInQuantity(toBuy.Store, toBuy.Name, toBuy.Quantity);
            return false;
        }

        public bool IsTicketValid(string itemName, string store, double wantToPay)
        {
            return _storeService.HasActiveLottery(store, itemName, wantToPay);
        }

        public double GetPriceFromCoupon(string itemName, string store, int quantity, string coupon)
        {
            return _storeService.CalculateItemPriceWithDiscount(store, itemName, coupon, quantity);
        }
    }
}
