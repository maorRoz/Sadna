using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
    class RealStoreShoppingBridge : IStoreShoppingBridge
    {
        private readonly MarketYard _market;
        private IStoreShoppingService _storeShoppingService;

        public RealStoreShoppingBridge()
        {
            _market = MarketYard.Instance;
            _storeShoppingService = null;
        }

        public void GetStoreShoppingService(IUserService userService)
        {
            _storeShoppingService = _market.GetStoreShoppingService(ref userService);
        }

        public MarketAnswer OpenStore(string name, string address)
        {
            return _storeShoppingService.OpenStore(name, address);
        }

        public MarketAnswer ViewStoreInfo(string store)
        {
            return _storeShoppingService.ViewStoreInfo(store);
        }


        public MarketAnswer AddProductToCart(string store, string productName, int quantity)
        {
            return _storeShoppingService.AddProductToCart(store, productName, quantity);
        }

        public MarketAnswer ViewStoreStock(string store)
        {
            return _storeShoppingService.ViewStoreStock(store);
        }

        public void CleanSession()
        {
            _storeShoppingService?.CleanSeesion();
        }


    }
}