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
		private IStoreManagementService _storeManagementService;

		public RealStoreBridge()
		{
			_market = MarketYard.Instance;
			_storeShoppingService = null;
			_storeManagementService = null;
		}

		public void GetStoreShoppingService(IUserService userService)
		{
			_storeShoppingService = _market.GetStoreShoppingService(ref userService);
		}

		public void GetStoreManagementService(IUserService userService, string store)
		{
			_storeManagementService = _market.GetStoreManagementService(userService, store);
		}

		public MarketAnswer OpenStore(string name, string address)
		{
			return _storeShoppingService.OpenStore(name, address);
		}
	
		public MarketAnswer ViewStoreInfo(string store)
		{
			return _storeShoppingService.ViewStoreInfo(store);
		}

		public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
		{
			return _storeManagementService.PromoteToStoreManager(someoneToPromoteName, actions);
		}


		public void CleanSession()
	    {
		    _storeShoppingService?.CleanSeesion();
			_storeManagementService?.CleanSession();
        }


}
}
