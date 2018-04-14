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
		MarketAnswer AddNewProduct(string _name, int _price, string _description, int quantity);
		MarketAnswer RemoveProduct(string productName);
		MarketAnswer EditProduct(string productName, string whatToEdit, string newValue);
		MarketAnswer AddQuanitityToProduct(string productName, int quantity);
		void CleanSession();
	}
}
