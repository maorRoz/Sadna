using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.MarketHarmony
{
    public interface IUserShopper
    {
        void AddToCart(Product product, string store, int quantity, string sale);

        void AddOwnership(string store);

        void ValidateCanOpenStore();

        void ValidateCanBrowseMarket();

    }
}
