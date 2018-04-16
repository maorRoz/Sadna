using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    public interface IStoresSyncher
    {
        
        /// <summary>
        /// Rmove <paramref name="purchased"/> from store stock
        /// </summary>
        void RemoveProducts(OrderItem[] purchased);

        /// <summary>
        /// update the lottery details after the purchase of a ticket
        /// </summary>
        void UpdateLottery(string itemName, string store,double moenyPayed, string username);


        /// <summary>
        /// Validate that <paramref name="toBuy"/> is valid product to buy by the user
        /// </summary>
        bool IsValid(OrderItem toBuy);

        /// <summary>
        /// Validate that <paramref name="toBuy"/> is valid product to buy by the user
        /// </summary>
        void ValidateTicket(string itemName, string store, double wantToPay);


        /// <summary>
        /// returns an order item with the updated price after discount, if coupon is invalid return null
        /// </summary>
        double GetPriceFromCoupon(string itemName, string store, int quantity, string coupon);
    }
}
