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

        private IStockSyncher _storeService;
        private IOrderSyncher orderSyncher;
        public StoresSyncherHarmony()
        {
            _storeService = StockSyncher.Instance;
            orderSyncher = new OrderSyncherHarmony();
        }

        public void RemoveProducts(OrderItem[] purchased)
        {
            foreach (OrderItem item in purchased)
            {
                try
                {
                    _storeService.UpdateQuantityAfterPurchase(item.Store, item.Name, item.Quantity);
                }
                catch (StoreException e)
                {
                    throw new OrderException(OrderItemStatus.InvalidDetails, e.GetErrorMessage());
                }
            }
        }

        public void UpdateLottery(string itemName, string store,double moenyPayed, string username,int cheatCode)
        {
            try
            {
                StockSyncher syncher = StockSyncher.Instance;
                syncher.UpdateLottery(store, itemName, moenyPayed, username, orderSyncher, cheatCode);
            }
            catch (StoreException e)
            {
                throw new OrderException(LotteryOrderStatus.InvalidLotteryTicket, e.GetErrorMessage());
            }
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
                throw new OrderException(LotteryOrderStatus.InvalidLotteryTicket, "no lottery is on going! cannot get ticket from expired or unavailable lottery"); 
            }
        }

        public double GetPriceFromCoupon(string itemName, string store, int quantity, string coupon)
        {
            try
            {
                double result = _storeService.CalculateItemPriceWithDiscount(store, itemName, coupon, quantity);
                return result;
            }
            catch (StoreException e)
            {
                throw new OrderException(OrderItemStatus.InvalidDetails, e.GetErrorMessage());
            }
            
        }

    }
}
