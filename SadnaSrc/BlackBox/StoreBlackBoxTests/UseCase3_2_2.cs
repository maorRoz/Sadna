using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_2_2
	{
		private IUserBridge _storeOwnerUserBridge;
		private IStoreShoppingBridge _storeShoppingBridge;
		private IStoreShoppingBridge _storeShoppingBridge2;
		private IStoreManagementBridge _storeManagementBridge;
		private IStoreManagementBridge _storeManagementBridge2;
		private IUserBridge _userBuyer;

		[TestInitialize]
		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            SignUp(ref _storeOwnerUserBridge, "Pnina", "Mishol", "7777", "77777777");
			OpenStoreAndProducts();
			_storeShoppingBridge2 = null;
			_storeManagementBridge2 = null;
			_userBuyer = null;
		}

		[TestMethod]
		public void DefineSuccessfullyVidibleDiscount()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.Success, res.Status);

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

		}

		[TestMethod]
		public void DefineSuccessfullyHiddenDiscount()
		{
			//check there is no discount for ouch
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "HIDDEN", false);
			Assert.AreEqual((int)DiscountStatus.Success, res.Status);
			string coupon = res.ReportList[0];

			//check the discount was added to the product in the stock
			MarketAnswer stock = _storeShoppingBridge.ViewStoreStock("Toy");
			string[] receivedStock = stock.ReportList;
			string[] expectedStock =
			{
				" name: Ouch base price: 30 description: Ouchouch , type is: hidden , Immediate , 6"
			};
			Assert.AreEqual(expectedStock.Length, receivedStock.Length);
			for (int i = 0; i < receivedStock.Length; i++)
			{
				Assert.AreEqual(expectedStock[i], receivedStock[i]);
			}

		}

		[TestMethod]
		public void AddDiscountFailedNoStore()
		{
			CheckNoDiscountAdded();

			_storeManagementBridge2 = StoreManagementDriver.getBridge();
			_storeManagementBridge2.GetStoreManagementService(_storeOwnerUserBridge.GetUserSession(), "StoreNotExists");
			MarketAnswer res = _storeManagementBridge2.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.NoStore, res.Status);

			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedProductNotFound()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Oucheeeee", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.ProductNotFound, res.Status);

			CheckNoDiscountAdded();

		}

		[TestMethod]
		public void AddDiscountFailedPrecentagesTooBig()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 120, "HIDDEN", true);
			Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, res.Status);

			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedAmountIsNegativeOrZero()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), -5, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, res.Status);

			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedDiscountGreaterThanProductPrice()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 50, "VISIBLE", false);
			Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, res.Status);

			CheckNoDiscountAdded();
		}

		[TestMethod]
		public void AddDiscountFailedNotherDiscountAlreadyExists()
		{
			CheckNoDiscountAdded();

			_storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "HIDDEN", true);
			Assert.AreEqual((int)DiscountStatus.thereIsAlreadyAnotherDiscount, _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "HIDDEN", true).Status);

		}

		[TestMethod]
		public void AddDiscountFailedNoUserPermissions()
		{
			SignUp(ref _userBuyer, "Vika", "Arad", "5555", "55555555");
			_storeManagementBridge2 = StoreManagementDriver.getBridge();
			_storeManagementBridge2.GetStoreManagementService(_userBuyer.GetUserSession(), "Toy");
			MarketAnswer res = _storeManagementBridge2.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10, "HIDDEN", true);
			Assert.AreEqual((int)StoreEnum.NoPremmision, res.Status);
		}


		[TestMethod]
		public void AddDiscountFailedDatesAreWrong()
		{
			CheckNoDiscountAdded();

			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("13/04/2018"), 10, "HIDDEN", false);
			Assert.AreEqual((int)DiscountStatus.DatesAreWrong, res.Status);

			CheckNoDiscountAdded();
		}



		private void OpenStoreAndProducts()
		{
			_storeShoppingBridge = StoreShoppingDriver.getBridge();
			_storeShoppingBridge.GetStoreShoppingService(_storeOwnerUserBridge.GetUserSession());
			_storeShoppingBridge.OpenStore("Toy", "notYour");
			_storeManagementBridge = StoreManagementDriver.getBridge();
			_storeManagementBridge.GetStoreManagementService(_storeOwnerUserBridge.GetUserSession(), "Toy");
			_storeManagementBridge.AddNewProduct("Ouch", 30, "Ouchouch", 6);
		}

		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
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
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();
        }


	}
}
