using SadnaSrc.Main;

namespace BlackBox
{
	interface IOrderBridge
	{
		void GetOrderService(IUserService userService);
		MarketAnswer BuyEverythingFromCart(string[] coupons);
		MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string coupon);
		MarketAnswer GiveDetails(string userName, string address, string creditCard);

        MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice);
        void DisableSupplySystem();
		void DisablePaymentSystem();
		void EnableSupplySystem();
		void EnablePaymentSystem();

	    void Cheat(int cheatCode);
	    void CleanSession();


	}
}
