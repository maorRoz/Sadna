using SadnaSrc.Main;

namespace BlackBox
{
	interface IStoreShoppingBridge
	{
		void GetStoreShoppingService(IUserService userService);
		MarketAnswer OpenStore(string name, string address);
		MarketAnswer ViewStoreInfo(string store);
		MarketAnswer AddProductToCart(string store, string productName, int quantity);
		void CleanSession();
	}
}
