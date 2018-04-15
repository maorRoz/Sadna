using System;
using SadnaSrc.Main;

namespace BlackBox
{
	class RealOrderBridge :IOrderBridge
	{
		private readonly MarketYard _market;
		private IOrderService _orderService;

		public RealOrderBridge()
		{
			_market = MarketYard.Instance;
		}

		public void GetOrderService(IUserService userService)
		{
			_orderService = _market.GetOrderService(ref userService);
		}

		public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
		{
			return _orderService.BuyItemFromImmediate(itemName, store, quantity,unitPrice);
		}

		public MarketAnswer BuyEverythingFromCart()
		{
			throw new NotImplementedException();
		}

		public MarketAnswer GiveDetails(string userName, string address, string creditCard)
		{
			throw new NotImplementedException();
		}

		public void CleanSession()
		{
			throw new NotImplementedException();
		}
	}
}
