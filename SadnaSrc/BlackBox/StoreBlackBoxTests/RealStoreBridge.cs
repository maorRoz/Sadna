using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	class RealStoreBridge :IStoreBridge
	{
		private readonly MarketYard _market;
		private IStoreShoppingService _storeShoppingService;

		public RealStoreBridge()
		{
			_market = MarketYard.Instance;
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


	}
}
