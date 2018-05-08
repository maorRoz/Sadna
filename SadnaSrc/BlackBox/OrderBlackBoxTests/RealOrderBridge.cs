using System;
using SadnaSrc.Main;

namespace BlackBox
{
	class RealOrderBridge :IOrderBridge
	{
		private readonly MarketYard _market;
		private IOrderService _orderService;
		private ISupplyService _supplyService;
		private IPaymentService _paymentService;

		public RealOrderBridge()
		{
			_market = MarketYard.Instance;
			_supplyService = _market.GetSupplyService();
			_paymentService = _market.GetPaymentService();
		}

		public void GetOrderService(IUserService userService)
		{
			_orderService = _market.GetOrderService(ref userService);
		}

		public MarketAnswer BuyEverythingFromCart(string[] coupons)
		{
			return _orderService.BuyEverythingFromCart(coupons);
		}

	    public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
	    {
	        return _orderService.BuyLotteryTicket(itemName, store, quantity, unitPrice);
	    }
		public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string coupon)
		{
			return _orderService.BuyItemFromImmediate(itemName, store, quantity, unitPrice, coupon);
		}

		public MarketAnswer GiveDetails(string userName, string address, string creditCard)
		{
			return _orderService.GiveDetails(userName, address, creditCard);
		}

		public void DisableSupplySystem()
		{
			_supplyService.BreakExternal();
		}

		public void DisablePaymentSystem()
		{
			_paymentService.BreakExternal();
		}

		public void EnableSupplySystem()
		{
			_supplyService.FixExternal();
		}

		public void EnablePaymentSystem()
		{
			_paymentService.FixExternal();
		}

	    public void Cheat(int cheatCode)
	    {
	        _orderService.Cheat(cheatCode);
	    }
    }
}
