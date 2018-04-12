using SadnaSrc.Main;

namespace BlackBox
{
	interface IStoreBridge
	{
		void GetStoreShoppingService(IUserService userService);
		MarketAnswer OpenStore(string name, string address);
		MarketAnswer ViewStoreInfo(string store);
	    void CleanSession();
	}
}
