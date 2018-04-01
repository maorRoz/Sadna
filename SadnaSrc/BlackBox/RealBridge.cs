using SadnaSrc.UserSpot;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : IUserBridge
	{
		private MarketYard _market;
		private IUserService _userService;

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
