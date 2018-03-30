using SadnaSrc.UserSpot;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : UserBridge
	{
		MarketYard _market;

		public RealBridge()
		{
			_market = new MarketYard();
		}

		public MarketAnswer EnterSystem()
		{
			return _market.GetUserService().EnterSystem();
		}

		public MarketAnswer SignUp(string name, string address, string password)
		{
			return _market.GetUserService().SignUp(name, address, password);
		}

		public void CleanSession()
		{
			_market.GetUserService().CleanSession();

		}

		public void ExitMarket()
		{
			_market.Exit();
		}
	}
}
