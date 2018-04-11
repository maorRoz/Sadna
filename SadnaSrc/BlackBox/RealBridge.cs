
using System;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : IUserBridge
	{
		private readonly MarketYard _market;
		private IUserService _userService;
		private ISystemAdminService _systemAdminService;
		private IStoreShoppingService _storeShoppingService;

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

		public MarketAnswer ViewCart()
		{
			return _userService.ViewCart();
		}

		public MarketAnswer EditCartItem(string store, string product, double unitPrice, int quantity)
		{
			return _userService.EditCartItem(store, product, unitPrice, quantity);
		}

		public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
		{
			return _userService.RemoveFromCart(store, product, unitPrice);
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

		public MarketAnswer OpenStore(string name, string address)
		{
			return _storeShoppingService.OpenStore(name, address);
		}

		public void GetStoreShoppingService()
		{
			_storeShoppingService = _market.GetStoreShoppingService(ref _userService);
		}


	}
}
