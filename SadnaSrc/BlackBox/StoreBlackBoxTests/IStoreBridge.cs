using SadnaSrc.Main;

namespace BlackBox
{
	interface IStoreBridge
	{
		void GetStoreShoppingService(IUserService userService);
		void GetStoreManagementService(IUserService userService, string store);
		MarketAnswer OpenStore(string name, string address);
		MarketAnswer ViewStoreInfo(string store);
		MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions);
		void CleanSession();
	}
}
