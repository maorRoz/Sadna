using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase1_3_1
	{
		private IUserBridge _bridgeSignUp;
		private IStoreShoppingBridge _storeBridge;
		private IStoreShoppingBridge _storeBridgeGuest;
		private IStoreManagementBridge _storeManagementBridge;
		private IUserBridge _userWatchStock;

		[TestInitialize]
		public void MarketBuilder()
		{
			_storeBridge = StoreShoppingDriver.getBridge();
			SignUp("Pnina", "mishol", "7894", "12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int)OpenStoreStatus.Success, _storeBridge.OpenStore("OOF", "BASA").Status);
			_storeManagementBridge = StoreManagementDriver.getBridge();
			_storeManagementBridge.GetStoreManagementService(_bridgeSignUp.GetUserSession(),"OOF");
			_storeManagementBridge.AddNewProduct("Toy", 20, "OuchOuch", 10);
			_userWatchStock = null;
			_storeBridgeGuest = null;
		}

		[TestMethod]
		public void GuestViewStock()
		{
			UserWatchStockInitialize();
			MarketAnswer stockDetails = _storeBridgeGuest.ViewStoreStock("OOF");
			WatchStockAndCompare(stockDetails);
		}


		[TestMethod]
		public void RegisteredUserViewStock()
		{
			MarketAnswer stockDetails = _storeBridge.ViewStoreStock("OOF");
			WatchStockAndCompare(stockDetails);
		}

		[TestMethod]
		public void NoStoreExistsGuestViewStock()
		{
			UserWatchStockInitialize();
			MarketAnswer stockDetails = _storeBridgeGuest.ViewStoreStock("OOFA");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, stockDetails.Status);
			Assert.IsNull(stockDetails.ReportList);
		}

		[TestMethod]
		public void InvalidUserDidntEnterSystem()
		{
			_userWatchStock = UserDriver.getBridge();
			_storeBridgeGuest = StoreShoppingDriver.getBridge();
			_storeBridgeGuest.GetStoreShoppingService(_userWatchStock.GetUserSession());
			MarketAnswer storeDetails = _storeBridgeGuest.ViewStoreInfo("OOF");
			Assert.AreEqual((int)ViewStoreStatus.InvalidUser, storeDetails.Status);
			Assert.AreEqual(null, storeDetails.ReportList);
		}

		private void UserWatchStockInitialize()
		{
			_userWatchStock = UserDriver.getBridge();
			_userWatchStock.EnterSystem();
			_storeBridgeGuest = StoreShoppingDriver.getBridge();
			_storeBridgeGuest.GetStoreShoppingService(_userWatchStock.GetUserSession());
		}

		[TestMethod]
		public void NoStoreExistsRegisteredUserViewStock()
		{
			MarketAnswer stockDetails = _storeBridge.ViewStoreStock("OOFA");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, stockDetails.Status);
			Assert.IsNull(stockDetails.ReportList);
		}

		private static void WatchStockAndCompare(MarketAnswer stockDetails)
		{
			Assert.AreEqual((int)StoreEnum.Success, stockDetails.Status);
			string[] expectedAnswer =
			{
				" name: Toy base price: 20 description: OuchOuch , Immediate , 10"
			};

			string[] receivedAnswer = stockDetails.ReportList;
			Assert.AreEqual(expectedAnswer.Length, receivedAnswer.Length);
			for (int i = 0; i < expectedAnswer.Length; i++)
			{
				Assert.AreEqual(expectedAnswer[i], receivedAnswer[i]);
			}
		}

		private void SignUp(string name, string address, string password, string creditCard)
		{
			_bridgeSignUp = UserDriver.getBridge();
			_bridgeSignUp.EnterSystem();
			_bridgeSignUp.SignUp(name, address, password, creditCard);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridgeSignUp.CleanSession();
			_storeBridge.CleanSession();
			_userWatchStock?.CleanSession();
			_storeBridgeGuest?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}

	}
}
