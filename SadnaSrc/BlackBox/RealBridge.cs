﻿
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : IUserBridge
	{
		private readonly MarketYard _market;
		private readonly IUserService _userService;
		private ISystemAdminService _systemAdminService;

		public RealBridge()
		{
			_market = MarketYard.Instance;
			_userService = _market.GetUserService();
		}

		public MarketAnswer EnterSystem()
		{
			return _userService.EnterSystem();
		}

		public MarketAnswer SignUp(string name, string address, string password)
		{
			return _userService.SignUp(name, address, password);
		}

		public MarketAnswer SignIn(string name, string password)
		{
			return _userService.SignIn(name, password);
		}

		public MarketAnswer ViewCart()
		{
			return _userService.ViewCart();
		}

		public MarketAnswer EditCartItem(string store, string product, double unitPrice, string sale, int quantity)
		{
			return _userService.EditCartItem(store, product, unitPrice, sale, quantity);
		}

		public MarketAnswer RemoveFromCart(string store, string product, double unitPrice, string sale)
		{
			return _userService.RemoveFromCart(store, product, unitPrice, sale);
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
	}
}
