using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	class RealAdminBridge :IAdminBridge
	{
		private readonly MarketYard _market;
		private ISystemAdminService _systemAdminService;

		public RealAdminBridge()
		{
			_market = MarketYard.Instance;
		}

		public void GetAdminService(IUserService userService)
		{
			_systemAdminService = _market.GetSystemAdminService(userService);
		}

		public MarketAnswer RemoveUser(string userName)
		{
			return _systemAdminService.RemoveUser(userName);
		}

		public MarketAnswer ViewPurchaseHistoryByUser(string userName)
		{
			return _systemAdminService.ViewPurchaseHistoryByUser(userName);
		}

		public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
		{
			return _systemAdminService.ViewPurchaseHistoryByStore(storeName);
		}

	}
}
