using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase8_1
	{
		private IUserBridge _storeOwnerUserBridge;
		private IStoreShoppingBridge _storeShoppingBridge;
		private IStoreShoppingBridge _storeShoppingBridge2;
		private IStoreManagementBridge _storeManagementBridge;
		private IStoreManagementBridge _storeManagementBridge2;
		private IUserBridge _userBuyer;
		private IUserBridge _userAdmin;
		private IAdminBridge _userAdminBridge;
		private IOrderBridge _orderBridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			_storeOwnerUserBridge = UserDriver.getBridge();
			_storeOwnerUserBridge.EnterSystem();
			_storeOwnerUserBridge.SignUp("Pnina", "Mishol", "7777", "77777777");
			_storeShoppingBridge = StoreShoppingDriver.getBridge();
			_storeShoppingBridge.GetStoreShoppingService(_storeOwnerUserBridge.GetUserSession());
			_storeShoppingBridge.OpenStore("Toy", "notYour");
			_storeManagementBridge = StoreManagementDriver.getBridge();
			_storeManagementBridge.GetStoreManagementService(_storeOwnerUserBridge.GetUserSession(), "Toy");
			_storeManagementBridge.AddNewProduct("Ouch", 30, "Ouchouch", 6);
			_storeShoppingBridge2 = null;
			_storeManagementBridge2 = null;
			_userBuyer = null;
			_userAdminBridge = null;
			_orderBridge = null;

		}

		[TestMethod]
		public void AddDiscountAndReceiveItInOrderSuccessfully()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("20/04/2018"), 10,"VISIBLE",false);
			Assert.AreEqual((int)DiscountStatus.Success,res.Status);

			//check the discount was added to the product in the stock
			MarketAnswer stock = _storeShoppingBridge.ViewStoreStock("Toy");
			string[] receivedStock = stock.ReportList;
			string[] expectedStock =
			{
				" name: Ouch base price: 30 description: Ouchouch , DiscountAmount: 10 Start Date: "+Convert.ToDateTime("14/04/2018").Date.ToString("d")+"" +
				" End Date: "+ Convert.ToDateTime("20/04/2018").Date.ToString("d")+" type is: visible , Immediate , 6"
			};
			Assert.AreEqual(expectedStock.Length, receivedStock.Length);
			for (int i = 0; i < receivedStock.Length; i++)
			{
				Assert.AreEqual(expectedStock[i], receivedStock[i]);
			}

			_userBuyer = UserDriver.getBridge();
			_userBuyer.EnterSystem();
			_userBuyer.SignUp("Vika", "Arad", "5555", "55555555");
			_storeShoppingBridge2 = StoreShoppingDriver.getBridge();
			_storeShoppingBridge2.GetStoreShoppingService(_userBuyer.GetUserSession());
			_storeShoppingBridge2.AddProductToCart("Toy", "Ouch", 3);

			_orderBridge = OrderDriver.getBridge();
			_orderBridge.GetOrderService(_userBuyer.GetUserSession());
			_orderBridge.BuyEverythingFromCart();
			SignInAdmin("Arik1", "123");
			_userAdminBridge = AdminDriver.getBridge();
			_userAdminBridge.GetAdminService(_userAdmin.GetUserSession());
			MarketAnswer purchaseHistory = _userAdminBridge.ViewPurchaseHistoryByUser("Vika");
			
			//make sure the price presented is after the discount
			string[] purchaseReceived = purchaseHistory.ReportList;
			string[] purchaseExpected =
			{
				"User: Vika Product: Ouch Store: Toy Sale: Immediate Quantity: 3 Price: 60 Date: " +
				DateTime.Now.Date.ToString("d"),
			};
			Assert.AreEqual(purchaseExpected.Length, purchaseReceived.Length);
			for (int i = 0; i < purchaseReceived.Length; i++)
			{
				Assert.AreEqual(purchaseExpected[i],purchaseReceived[i]);
			}

		}

		[TestMethod]
		public void AddDiscountAndDontReceiveItBecauseDatePassed()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("15/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.Success, res.Status);

			MarketYard.SetDateTime(Convert.ToDateTime("16/04/2018"));

			//check the discount was added to the product in the stock
			MarketAnswer stock = _storeShoppingBridge.ViewStoreStock("Toy");
			string[] receivedStock = stock.ReportList;
			string[] expectedStock =
			{
				" name: Ouch base price: 30 description: Ouchouch , DiscountAmount: 10 Start Date: "+Convert.ToDateTime("14/04/2018").Date.ToString("d")+"" +
				" End Date: "+ Convert.ToDateTime("15/04/2018").Date.ToString("d")+" type is: visible , Immediate , 6"
			};
			Assert.AreEqual(expectedStock.Length, receivedStock.Length);
			for (int i = 0; i < receivedStock.Length; i++)
			{
				Assert.AreEqual(expectedStock[i], receivedStock[i]);
			}

			_userBuyer = UserDriver.getBridge();
			_userBuyer.EnterSystem();
			_userBuyer.SignUp("Vika", "Arad", "5555", "55555555");
			_storeShoppingBridge2 = StoreShoppingDriver.getBridge();
			_storeShoppingBridge2.GetStoreShoppingService(_userBuyer.GetUserSession());
			_storeShoppingBridge2.AddProductToCart("Toy", "Ouch", 3);

			_orderBridge = OrderDriver.getBridge();
			_orderBridge.GetOrderService(_userBuyer.GetUserSession());
			_orderBridge.BuyEverythingFromCart();
			SignInAdmin("Arik1", "123");
			_userAdminBridge = AdminDriver.getBridge();
			_userAdminBridge.GetAdminService(_userAdmin.GetUserSession());
			MarketAnswer purchaseHistory = _userAdminBridge.ViewPurchaseHistoryByUser("Vika");

			//make sure the price presented is without the discount
			string[] purchaseReceived = purchaseHistory.ReportList;
			string[] purchaseExpected =
			{
				"User: Vika Product: Ouch Store: Toy Sale: Immediate Quantity: 3 Price: 90 Date: " +
				DateTime.Now.Date.ToString("d"),
			};
			Assert.AreEqual(purchaseExpected.Length, purchaseReceived.Length);
			for (int i = 0; i < purchaseReceived.Length; i++)
			{
				Assert.AreEqual(purchaseExpected[i], purchaseReceived[i]);
			}
		}

		[TestMethod]
		public void AddDiscountFailedNoStore()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			_storeManagementBridge2 = StoreManagementDriver.getBridge();
			_storeManagementBridge2.GetStoreManagementService(_storeOwnerUserBridge.GetUserSession(),"StoreNotExists");
			MarketAnswer res = _storeManagementBridge2.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.NoStore, res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedProductNotFound()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Oucheeeee", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.ProductNotFound, res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();

		}

		[TestMethod]
		public void AddDiscountFailedDatesAreWrong()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("13/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.DatesAreWrong, res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedPrecentagesTooBig()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 120, "VISIBLE", true);
			Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges ,res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedAmountIsNegativeOrZero()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), -5, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedDiscountGreaterThanProductPrice()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//add the discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 50, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, res.Status);

			//check that the product in the stock doesn't have a discout
			CheckNoDiscountAdded();
		}


		[TestMethod]
		public void AddDiscountFailedNotherDiscountAlreadyExists()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			//create a new discount
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", true);
			Assert.AreEqual((int)DiscountStatus.thereIsAlreadyAnotherDiscount, _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", true).Status);


		}

		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
		}

		private void SignInAdmin(string name, string password)
		{
			_userAdmin = UserDriver.getBridge();
			_userAdmin.EnterSystem();
			_userAdmin.SignIn(name, password);
		}

		private void CheckNoDiscountAdded()
		{
			MarketAnswer stock1 = _storeShoppingBridge.ViewStoreStock("Toy");
			string[] receivedStock1 = stock1.ReportList;
			string[] expectedStock1 =
			{
				" name: Ouch base price: 30 description: Ouchouch , Immediate , 6"
			};

			Assert.AreEqual(expectedStock1.Length, receivedStock1.Length);
			for (int i = 0; i < receivedStock1.Length; i++)
			{
				Assert.AreEqual(expectedStock1[i], receivedStock1[i]);
			}
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			MarketYard.SetDateTime(Convert.ToDateTime("14/04/2018"));
			_userBuyer?.CleanSession();
			_storeOwnerUserBridge.CleanSession();
			_storeShoppingBridge.CleanSession();
			_storeShoppingBridge2?.CleanSession();
			_storeManagementBridge.CleanSession();
			_storeManagementBridge2?.CleanSession();
			_orderBridge?.CleanSession();
			_storeOwnerUserBridge.CleanMarket();
		}
	}
}
