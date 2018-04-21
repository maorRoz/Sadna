using System;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace BlackBox
{
    public class RealUserBridge : IUserBridge
    {
        private readonly MarketYard _market;
        private IUserService _userService;

        public RealUserBridge()
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

        public MarketAnswer EditCartItem(string store, string product, int quantity, double unitPrice)
        {
            return _userService.EditCartItem(store, product, quantity, unitPrice);
        }

        public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
        {
            return _userService.RemoveFromCart(store, product, unitPrice);
        }

		public IUserService GetUserSession()
		{
			return _userService;
		}

        public void CleanSession()
        {
            _userService.CleanSession();

        }

        public void CleanMarket()
        {
            MarketYard.CleanSession();
        }

    }
}