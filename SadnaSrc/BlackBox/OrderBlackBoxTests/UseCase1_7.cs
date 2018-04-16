using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.OrderBlackBoxTests
{
	[TestClass]
	public class UseCase1_7
	{
		private IUserBridge _userAdminBridge;
		private IAdminBridge _adminBridge;
		private IUserBridge _buyerRegisteredUserBridge;
		private IUserBridge _buyerGuestBridge;
		private IUserBridge _storeOwnerBridge; 
		private IStoreShoppingBridge _shoppingBridge;
		private IStoreShoppingBridge _shoppingBridge2;
		private IStoreManagementBridge _storeManagementBridge;
		private IStoreManagementBridge _storeManagementBridge2;
		private IOrderBridge _orderBridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp("Pnina", "mishol", "666", "66666666");
			OpenStoreAndAddProducts();
			_orderBridge = OrderDriver.getBridge();
			_userAdminBridge = UserDriver.getBridge();
			_userAdminBridge.EnterSystem();
			MarketAnswer res = _userAdminBridge.SignIn("Arik1", "123");
			_adminBridge = AdminDriver.getBridge();
			_adminBridge.GetAdminService(_userAdminBridge.GetUserSession());
			_buyerRegisteredUserBridge = null;
			_buyerGuestBridge = null;
		}

		private void SignUp(string name, string address, string password, string creditCard)
		{
			_storeOwnerBridge = UserDriver.getBridge();
			_storeOwnerBridge.EnterSystem();
			_storeOwnerBridge.SignUp(name, address, password, creditCard);
		}

		private void OpenStoreAndAddProducts()
		{
			_shoppingBridge = StoreShoppingDriver.getBridge();
			_shoppingBridge.GetStoreShoppingService(_storeOwnerBridge.GetUserSession());
			_shoppingBridge.OpenStore("Yalla", "Balagan");
			_storeManagementBridge = StoreManagementDriver.getBridge();
			_storeManagementBridge.GetStoreManagementService(_storeOwnerBridge.GetUserSession(),"Yalla");
			_storeManagementBridge.AddNewProduct("Tea", 10, "CherryFlavour", 6);
			_shoppingBridge.OpenStore("HAHAHA", "LOLOLO");
			_storeManagementBridge2 = StoreManagementDriver.getBridge();
			_storeManagementBridge2.GetStoreManagementService(_storeOwnerBridge.GetUserSession(),"HAHAHA");
			_storeManagementBridge2.AddNewProduct("Coffee", 10, "Black", 6);
		}

		private void AddProductsToCartRegisteredUser()
		{
			_buyerRegisteredUserBridge = UserDriver.getBridge();
			_buyerRegisteredUserBridge.EnterSystem();
			_buyerRegisteredUserBridge.SignUp("Shalom", "Bye", "555", "55555555");
			_shoppingBridge2 = StoreShoppingDriver.getBridge();
			_shoppingBridge2.GetStoreShoppingService(_buyerRegisteredUserBridge.GetUserSession());
			_shoppingBridge2.AddProductToCart("Yalla", "Tea", 4);
			_shoppingBridge2.AddProductToCart("HAHAHA", "Coffee", 3);
		}

		private void AddProductsToCartGuest()
		{
			_buyerGuestBridge = UserDriver.getBridge();
			_buyerGuestBridge.EnterSystem();
			_shoppingBridge2 = StoreShoppingDriver.getBridge();
			_shoppingBridge2.GetStoreShoppingService(_buyerGuestBridge.GetUserSession());
			_shoppingBridge2.AddProductToCart("Yalla", "Tea", 4);
			_shoppingBridge2.AddProductToCart("HAHAHA", "Coffee", 3);
		}

		private void AddProductOutOfStock()
		{
			_buyerRegisteredUserBridge = UserDriver.getBridge();
			_buyerRegisteredUserBridge.EnterSystem();
			_buyerRegisteredUserBridge.SignUp("Shalom", "Bye", "555", "55555555");
			_shoppingBridge2 = StoreShoppingDriver.getBridge();
			_shoppingBridge2.GetStoreShoppingService(_buyerRegisteredUserBridge.GetUserSession());
			_shoppingBridge2.AddProductToCart("Yalla", "Tea", 7);
			_shoppingBridge2.AddProductToCart("HAHAHA", "Coffee", 3);
		}

		[TestMethod]
		public void SuccessBuyingProductsRegisterUser()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			MarketAnswer res = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderStatus.Success,res.Status);
			//TODO: check in the purchase history
			MarketAnswer puchaseHistory = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, puchaseHistory.Status);
			string[] receivedHistory = puchaseHistory.ReportList;
			string[] expectedHistory =
			{
				"User: Shalom Product: Coffee Store: HAHAHA Sale: Immediate Quantity: 3 Price: 30 Date: " +
				DateTime.Now.Date.ToString("d"),
				"User: Shalom Product: Tea Store: Yalla Sale: Immediate Quantity: 4 Price: 40 Date: " +
				DateTime.Now.Date.ToString("d")
			};
			Assert.AreEqual(expectedHistory.Length,receivedHistory.Length);
			for (int i = 0; i < expectedHistory.Length; i++)
			{
				Assert.AreEqual(expectedHistory[i],receivedHistory[i]);
			}
			//TODO: check the products are no longer in the cart
			MarketAnswer cartDetails =_buyerRegisteredUserBridge.ViewCart();
			string[] expectedCart = { };
			string[] receivedCart = cartDetails.ReportList;
			Assert.AreEqual(expectedCart.Length,receivedCart.Length);
			//TODO: check the stock
			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
				" name: Tea base price: 10 description: CherryFlavour , Immediate , 2"
			};
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
				" name: Coffee base price: 10 description: Black , Immediate , 3"
			};
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);
		}

		[TestMethod]
		public void SuccessBuyingProductsGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", "MisholGuest", "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderStatus.Success, order.Status);
			
			//TODO: check the products are no longer in the cart
			MarketAnswer cartDetails = _buyerGuestBridge.ViewCart();
			string[] expectedCart = { };
			string[] receivedCart = cartDetails.ReportList;
			Assert.AreEqual(expectedCart.Length, receivedCart.Length);
			//TODO: check the stock
			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
				" name: Tea base price: 10 description: CherryFlavour , Immediate , 2"
			};
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
				" name: Coffee base price: 10 description: Black , Immediate , 3"
			};
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);
		}

		[TestMethod]
		public void FailPurchaseWrongUserNameToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails(null, "MisholGuest", "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			//TODO: check cart stayed the same

		}

		[TestMethod]
		public void FailPurchaseWrongAddressToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", null, "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			//TODO: check cart stayed the same

		}

		[TestMethod]
		public void FailPurchaseWrongCreditCartToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", "MisholGuest", "");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			//TODO: check there is nothing in the puchase history
			MarketAnswer history = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.IsNull(history.ReportList);
		}

		[TestMethod]
		public void NonDetailsToPaymentSystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			MarketAnswer order = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			
		}

		[TestMethod]
		public void FailPurchaseSupplySystemCollapsed()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			_orderBridge.DisableSupplySystem();
			MarketAnswer res = _orderBridge.BuyEverythingFromCart();
			Assert.AreEqual((int)OrderItemStatus.NoOrderItemInOrder, res.Status);
			//TODO: check there is nothing in the puchase history
			MarketAnswer history = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.IsNull(history.ReportList);

		}

		[TestMethod]
		public void FailPchasePaymentSystemCollapsed()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			_orderBridge.DisablePaymentSystem();
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("Tea", "Yalla", 2, 1);
			Assert.AreEqual((int)OrderItemStatus.NoOrderItemInOrder, res.Status);
			//TODO: check there is nothing in the puchase history
			MarketAnswer history = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.IsNull(history.ReportList);
		}


		[TestMethod]
		public void FailPurchaseProductBuyMoreThanExistsInCart()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("Tea","Yalla",2100,10);
			Assert.AreEqual((int)OrderItemStatus.NoOrderItemInOrder, res.Status);
			//TODO: purchase history is null
			MarketAnswer history = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.IsNull(history.ReportList);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_userAdminBridge.CleanSession();
			_buyerRegisteredUserBridge?.CleanSession();
			_buyerGuestBridge?.CleanSession();
			_storeOwnerBridge.CleanSession();
			_shoppingBridge.CleanSession();
			_shoppingBridge2?.CleanSession();
			_storeManagementBridge.CleanSession();
			_storeManagementBridge2.CleanSession();
			_orderBridge.CleanSession();
			_orderBridge.EnableSupplySystem();
			_orderBridge.EnablePaymentSystem();
			_userAdminBridge.CleanMarket();

		}

	

	}
}
