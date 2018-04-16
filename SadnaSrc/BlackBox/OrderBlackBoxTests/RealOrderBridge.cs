﻿using System;
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

		public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
		{
			return _orderService.BuyItemFromImmediate(itemName, store, quantity,unitPrice);
		}

		public MarketAnswer BuyEverythingFromCart()
		{
			return _orderService.BuyEverythingFromCart();
		}

	    public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
	    {
	        return _orderService.BuyLotteryTicket(itemName, store, quantity, unitPrice);
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

		public void CleanSession()
		{
			_orderService.CleanSession();
		}

	    public void Cheat(int cheatCode)
	    {
	        _orderService.Cheat(cheatCode);
	    }
    }
}
