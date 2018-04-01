using SadnaSrc.Main;

namespace BlackBox
{
	public interface UserBridge
	{
		MarketAnswer EnterSystem();
		MarketAnswer SignUp(string name, string address, string password);
		MarketAnswer SignIn(string name, string password);
		void CleanSession();
		void ExitMarket();
	}
}
