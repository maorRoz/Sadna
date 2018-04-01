using SadnaSrc.Main;

namespace BlackBox
{
	public interface IUserBridge
	{
		MarketAnswer EnterSystem();
		MarketAnswer SignUp(string name, string address, string password);
		void CleanSession();
		void CleanMarket();
	}
}
