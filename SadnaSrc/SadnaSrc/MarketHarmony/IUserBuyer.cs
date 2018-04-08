using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    // integration between UserSpot to OrderPool only
    public interface IUserBuyer
    {
        /// <summary>
        ///Fetch everything from the user's cart into order items array.
        /// </summary>
        /// 
        OrderItem[] CheckoutAll();
        /// <summary>
        /// Fetch everything from the user cart which belong to the <paramref name="store"/>.
        /// </summary>
        /// <param name="store"> The name of the store </param>
        OrderItem[] CheckoutFromStore(string store);
        /// <summary>
        /// Fetch product with name of <paramref name="itemName"/> from the <paramref name="store"/> in quantity of <paramref name="quantity"/>  and unit price of <paramref name="unitPrice"/>.
        /// </summary>
        /// <param name="itemName"> The name of the store </param>
        /// <param name="store"> The name of the store </param>
        /// <param name="quantity"> The name of the store </param>
        /// <param name="unitPrice"> The name of the store </param>
        OrderItem CheckoutItem(string itemName, string store, int quantity, double unitPrice);
        /// <summary>
        /// Return registered user address or null if not registered
        /// </summary>

        string GetAddress();
        /// <summary>
        /// Return registered user name or null if not registered
        /// </summary>
        string GetName();
        /// <summary>
        /// Return registered user credit card or null if not registered
        /// </summary>
        string GetCreditCard();
        /// <summary>
        /// Clean UserService data from DB . use during clean up method of testing.
        /// </summary>
        void CleanSession(); //only for testing
    }
}
