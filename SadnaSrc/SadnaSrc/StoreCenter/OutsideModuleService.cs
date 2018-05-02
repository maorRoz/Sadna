using SadnaSrc.UserSpot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    interface OutsideModuleService
    {
        void UpdateQuantityAfterPurchase(string store, string product, int quantity);
        bool ProductExistsInQuantity(string storeName, string product, int quantity);
        double CalculateItemPriceWithDiscount(string storeName, string productName, string _DiscountCode, int _quantity);

        bool HasActiveLottery(string storeName, string productName, double priceWantToPay);
        void updateLottery(string storeName, string ProductName, double moenyPayed, string UserName, IOrderSyncher syncher,int cheatCode);

    }
}