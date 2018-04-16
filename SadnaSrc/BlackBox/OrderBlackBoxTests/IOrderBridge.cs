using SadnaSrc.Main;

namespace BlackBox
{
	interface IOrderBridge
	{
		void GetOrderService(IUserService userService);
		MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice);
		MarketAnswer BuyEverythingFromCart();
		MarketAnswer GiveDetails(string userName, string address, string creditCard);
		void DisableSupplySystem();
		void DisablePaymentSystem();
		void EnableSupplySystem();
		void EnablePaymentSystem();
		void CleanSession();

	}
}
