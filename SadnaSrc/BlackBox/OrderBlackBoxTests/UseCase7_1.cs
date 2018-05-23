using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace BlackBox.OrderBlackBoxTests
{
// in this class, the emphasis is on the puchase from the cart and that it was immediate
	[TestClass]
	public class UseCase7_1
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
		    MarketDB.Instance.InsertByForce();
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
		    PaymentService.Instance.FixExternal();
		    SupplyService.Instance.FixExternal();
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
			_storeManagementBridge.GetStoreManagementService(_storeOwnerBridge.GetUserSession(), "Yalla");
			_storeManagementBridge.AddNewProduct("Tea", 10, "CherryFlavour", 6);
			_shoppingBridge.OpenStore("HAHAHA", "LOLOLO");
			_storeManagementBridge2 = StoreManagementDriver.getBridge();
			_storeManagementBridge2.GetStoreManagementService(_storeOwnerBridge.GetUserSession(), "HAHAHA");
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
			MarketAnswer cartDetails = _buyerRegisteredUserBridge.ViewCart();
			string[] received = cartDetails.ReportList;
			string[] expected =
			{
				"Name : Coffee Store : HAHAHA Quantity : 3 Unit Price : 10 Final Price : 30",
				"Name : Tea Store : Yalla Quantity : 4 Unit Price : 10 Final Price : 40"
			};

			Assert.AreEqual(expected.Length, cartDetails.ReportList.Length);
			for (int i = 0; i < received.Length; i++)
			{
				Assert.AreEqual(expected[i], received[i]);
			}

		}


		
		private void AddProductsToCartGuest()
		{
			_buyerGuestBridge = UserDriver.getBridge();
			_buyerGuestBridge.EnterSystem();
			_shoppingBridge2 = StoreShoppingDriver.getBridge();
			_shoppingBridge2.GetStoreShoppingService(_buyerGuestBridge.GetUserSession());
			_shoppingBridge2.AddProductToCart("Yalla", "Tea", 4);
			_shoppingBridge2.AddProductToCart("HAHAHA", "Coffee", 3);
			MarketAnswer cartDetails = _buyerGuestBridge.ViewCart();
			string[] received = cartDetails.ReportList;
			string[] expected =
			{
				"Name : Coffee Store : HAHAHA Quantity : 3 Unit Price : 10 Final Price : 30",
				"Name : Tea Store : Yalla Quantity : 4 Unit Price : 10 Final Price : 40"
			};
			
			Assert.AreEqual(expected.Length, cartDetails.ReportList.Length);
			for (int i = 0; i < received.Length; i++)
			{
				Assert.AreEqual(expected[i],received[i]);
			}

		}


		[TestMethod]
		public void SuccessBuyingProductsRegisterUser()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			MarketAnswer res = _orderBridge.BuyEverythingFromCart(new string[]{null, null});
			Assert.AreEqual((int)OrderStatus.Success, res.Status);
			MarketAnswer puchaseHistory = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, puchaseHistory.Status);
			string[] receivedHistory = puchaseHistory.ReportList;
			string[] expectedHistory =
			{
				"User: Shalom Product: Coffee Store: HAHAHA Sale: Immediate Quantity: 3 Price: 30 Date: " +
			    DateTime.Now.Date.ToString("dd/MM/yyyy"),
				"User: Shalom Product: Tea Store: Yalla Sale: Immediate Quantity: 4 Price: 40 Date: " +
				DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
			Assert.AreEqual(expectedHistory.Length, receivedHistory.Length);
			for (int i = 0; i < expectedHistory.Length; i++)
			{
				Assert.AreEqual(expectedHistory[i], receivedHistory[i]);
			}
			MarketAnswer cartDetails = _buyerRegisteredUserBridge.ViewCart();
			string[] expectedCart = { };
			string[] receivedCart = cartDetails.ReportList;
			Assert.AreEqual(expectedCart.Length, receivedCart.Length);
			//TODO: check the stock
			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
                " name: Tea base price: 10 description: CherryFlavour Discount: {null} Purchase Way: Immediate Quantity: 2"
            };
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
                " name: Coffee base price: 10 description: Black Discount: {null} Purchase Way: Immediate Quantity: 3"
            };
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);
		}

		[TestMethod]
		public void SuccessBuyingProductsGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", "MisholGuest", "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart(new string[] { null, null });
			Assert.AreEqual((int)OrderStatus.Success, order.Status);

			MarketAnswer cartDetails = _buyerGuestBridge.ViewCart();
			string[] expectedCart = { };
			string[] receivedCart = cartDetails.ReportList;
			Assert.AreEqual(expectedCart.Length, receivedCart.Length);
			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
                " name: Tea base price: 10 description: CherryFlavour Discount: {null} Purchase Way: Immediate Quantity: 2"
            };
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
                " name: Coffee base price: 10 description: Black Discount: {null} Purchase Way: Immediate Quantity: 3"
            };
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);
		}

		[TestMethod]
		public void FailPurchaseProductBuyMoreThanExistsInCart()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("Tea", "Yalla", 100, 10, null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, res.Status);
			CheckHistoryNullCartSameStockNotChangedRegisterUser();
		}

		[TestMethod]
		public void FailBuyingProductNotInCart()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("why","HAHAHA",3,10, null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails,res.Status);
			CheckHistoryNullCartSameStockNotChangedRegisterUser();
		}


		[TestMethod]
		public void FailPurchaseWrongUserNameToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails(null, "MisholGuest", "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart(null);
			CheckCartSameStockNotChangedGuest();
		}

		[TestMethod]
		public void FailPurchaseWrongAddressToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", null, "77777777");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart(null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			CheckCartSameStockNotChangedGuest();
		}

		[TestMethod]
		public void FailPurchaseWrongCreditCartToSupplySystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			_orderBridge.GiveDetails("PninaGuest", "MisholGuest", "");
			MarketAnswer order = _orderBridge.BuyEverythingFromCart(null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			CheckCartSameStockNotChangedGuest();
		}

		[TestMethod]
		public void NonDetailsToPaymentSystemGuest()
		{
			AddProductsToCartGuest();
			_orderBridge.GetOrderService(_buyerGuestBridge.GetUserSession());
			MarketAnswer order = _orderBridge.BuyEverythingFromCart(null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, order.Status);
			CheckCartSameStockNotChangedGuest();
		}

		[TestMethod]
		public void FailPurchaseSupplySystemCollapsed()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			_orderBridge.DisableSupplySystem();
			MarketAnswer res = _orderBridge.BuyEverythingFromCart(null);
			Assert.AreEqual((int)OrderItemStatus.NoOrderItemInOrder, res.Status);
			CheckHistoryNullCartSameStockNotChangedRegisterUser();

		}

		[TestMethod]
		public void FailPchasePaymentSystemCollapsed()
		{
			AddProductsToCartRegisteredUser();
			_orderBridge.GetOrderService(_buyerRegisteredUserBridge.GetUserSession());
			_orderBridge.DisablePaymentSystem();
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("Tea", "Yalla", 2, 1, null);
			Assert.AreEqual((int)OrderItemStatus.InvalidDetails, res.Status);
			CheckHistoryNullCartSameStockNotChangedRegisterUser();
		}

		private void CheckHistoryNullCartSameStockNotChangedRegisterUser()
		{
			MarketAnswer purhcaseHistory = _adminBridge.ViewPurchaseHistoryByUser("Shalom");
			Assert.IsNull(purhcaseHistory.ReportList);
			MarketAnswer cartDetails = _buyerRegisteredUserBridge.ViewCart();
			string[] received = cartDetails.ReportList;
			string[] expected =
			{
				"Name : Coffee Store : HAHAHA Quantity : 3 Unit Price : 10 Final Price : 30",
				"Name : Tea Store : Yalla Quantity : 4 Unit Price : 10 Final Price : 40"
			};

			Assert.AreEqual(expected.Length, received.Length);
			for (int i = 0; i < received.Length; i++)
			{
				Assert.AreEqual(expected[i], received[i]);
			}

			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
				" name: Tea base price: 10 description: CherryFlavour Discount: {null} Purchase Way: Immediate Quantity: 6"
			};
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
                " name: Coffee base price: 10 description: Black Discount: {null} Purchase Way: Immediate Quantity: 6"
            };
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);

		}

		private void CheckCartSameStockNotChangedGuest()
		{
			MarketAnswer cartDetails = _buyerGuestBridge.ViewCart();
			string[] received = cartDetails.ReportList;
			string[] expected =
			{
				"Name : Coffee Store : HAHAHA Quantity : 3 Unit Price : 10 Final Price : 30",
				"Name : Tea Store : Yalla Quantity : 4 Unit Price : 10 Final Price : 40"
			};

			Assert.AreEqual(expected.Length, received.Length);
			for (int i = 0; i < received.Length; i++)
			{
				Assert.AreEqual(expected[i], received[i]);
			}
			MarketAnswer stock1 = _shoppingBridge.ViewStoreStock("Yalla");
			string[] expectedYallaStock =
			{
                " name: Tea base price: 10 description: CherryFlavour Discount: {null} Purchase Way: Immediate Quantity: 6"
            };
			Assert.AreEqual(expectedYallaStock[0], stock1.ReportList[0]);
			MarketAnswer stock2 = _shoppingBridge.ViewStoreStock("HAHAHA");
			string[] expectedHahahaStock =
			{
                " name: Coffee base price: 10 description: Black Discount: {null} Purchase Way: Immediate Quantity: 6"
            };
			Assert.AreEqual(expectedHahahaStock[0], stock2.ReportList[0]);
		}

		
		[TestCleanup]
		public void UserTestCleanUp()
		{
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();

        }

	}
}
