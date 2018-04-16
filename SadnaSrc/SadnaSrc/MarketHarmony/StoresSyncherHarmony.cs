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
    public class StoresSyncherHarmony : IStoresSyncher
    {

        private OutsideModuleService _storeService;
        private IOrderSyncher orderSyncher;
        public StoresSyncherHarmony()
        {
            _storeService = ModuleGlobalHandler.GetInstance();
            orderSyncher = new OrderSyncherHarmony();
        }

        public void RemoveProducts(OrderItem[] purchased)
        {
            foreach (OrderItem item in purchased)
            {
                _storeService.UpdateQuantityAfterPurchase(item.Store, item.Name, item.Quantity);
            }
        }

        public void UpdateLottery(string itemName, string store,double moenyPayed, string username,int cheatCode)
        {
            ModuleGlobalHandler globalHandler = ModuleGlobalHandler.GetInstance();
            globalHandler.updateLottery(store, itemName, moenyPayed, username, orderSyncher,cheatCode);
        }


        public bool IsValid(OrderItem toBuy)
        {
            if(toBuy.Quantity > 0)
                return _storeService.ProductExistsInQuantity(toBuy.Store, toBuy.Name, toBuy.Quantity);
            return false;
        }

        public void ValidateTicket(string itemName, string store, double wantToPay)
        {
            if (!_storeService.HasActiveLottery(store, itemName, wantToPay))
            {
                throw new OrderException(GiveDetailsStatus.InvalidNameOrAddress, "no lottery is on going! cannot get ticket from expired or unavailable lottery"); 
            }
        }

        public double GetPriceFromCoupon(string itemName, string store, int quantity, string coupon)
        {
            return _storeService.CalculateItemPriceWithDiscount(store, itemName, coupon, quantity);
        }

        public void CleanSession()
        {
            orderSyncher.CleanSession();
        }
    }
}
