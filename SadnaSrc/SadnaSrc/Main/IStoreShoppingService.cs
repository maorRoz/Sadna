using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreShoppingService
    {
        MarketAnswer OpenStore(string storeName, string storeAddress);

        MarketAnswer ViewStoreInfo(string store);

        MarketAnswer ViewStoreStock(string store);

        MarketAnswer AddProductToCart(string productName);
    }
}
