using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
    class ProxyStoreShoppingBridge : IStoreShoppingBridge
    {
        public IStoreShoppingBridge real;

        public void GetStoreShoppingService(IUserService userService)
        {
            if (real != null)
            {
                real.GetStoreShoppingService(userService);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public MarketAnswer OpenStore(string name, string address)
        {
            if (real != null)
            {
                return real.OpenStore(name, address);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer ViewStoreInfo(string store)
        {
            if (real != null)
            {
                return real.ViewStoreInfo(store);
            }

            throw new NotImplementedException();
        }


        public MarketAnswer AddProductToCart(string store, string productName, int quantity)
        {
            if (real != null)
            {
                return real.AddProductToCart(store, productName, quantity);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer ViewStoreStock(string store)
        {
            if (real != null)
            {
                return real.ViewStoreStock(store);
            }
            throw new NotImplementedException();
        }

        public void CleanSession()
        {
            if (real != null)
            {
                real.CleanSession();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}