
using System;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : IUserBridge
	{
		private readonly MarketYard _market;
		private readonly IUserService _userService;
		private ISystemAdminService _systemAdminService;
		//private IStoreService _storeService;

		public RealBridge()
		{
			_market = MarketYard.Instance;
			_userService = _market.GetUserService();
		}

		public MarketAnswer EnterSystem()
		{
			return _userService.EnterSystem();
		}

		public MarketAnswer SignUp(string name, string address, string password, string creditCard)
		{
			return _userService.SignUp(name, address, password, creditCard);
		}

		public MarketAnswer SignIn(string name, string password)
		{
			return _userService.SignIn(name, password);
		}

		public void CleanSession()
		{
			_userService.CleanSession();

		}

		public void CleanMarket()
		{
			MarketYard.CleanSession();
		}

		public void GetAdminService()
		{
			_systemAdminService = _market.GetSystemAdminService(_userService);
		}

		public void GetStoreService()
		{
			//_storeService = _market.GetStoreService(_userService);
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

		//TODO: change this function according to the implementation
		public MarketAnswer createStore(int id, string address, string status)
		{
			return new MarketAnswer(0,"");
		}

		public MarketAnswer OpenStore(string name, string store)
		{
			throw new NotImplementedException();
		}


	}
}
