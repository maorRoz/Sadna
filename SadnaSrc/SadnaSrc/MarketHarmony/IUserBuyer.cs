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
        OrderItem[] CheckoutAll();
            
        OrderItem[] CheckoutFromStore(string store);

        OrderItem CheckoutItem(string itemName, string store, int quantity, double unitPrice);

        string GetAddress();

        string GetName();

        void CleanSession(); //only for testing
    }
}
