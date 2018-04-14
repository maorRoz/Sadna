using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_1_2
	{
		private IStoreBridge _storeBridge;
		private IStoreBridge _storeBridge2;
		private IUserBridge _userBridge2;
		private IUserBridge _userBridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _userBridge, "Pnina", "lo kef", "777777", "88888888");
			_storeBridge = StoreDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_userBridge.getUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_userBridge2 = null;
			_storeBridge2 = null;
		}

		[TestMethod]
		public void SuccessRemovingAProducFromStore()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result1 = _storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, result1.Status);
			MarketAnswer result2 = _storeBridge.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			//TODO: check in the stock to see if the product was indeed removed.
		}

		[TestMethod]
		public void ProductNotFoundInTheStore()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer res = _storeBridge.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
		}

		[TestMethod]
		public void NoPermissionsToAddAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer res1 = _storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, res1.Status);
			_storeBridge2 = StoreDriver.getBridge();
			_storeBridge2.GetStoreManagementService(_userBridge2.getUserSession(),"lokef");
			MarketAnswer res2 = _storeBridge2.RemoveProduct("bamba");
			//Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
		}

		[TestMethod]
		public void NoSuchStore()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer res1 = _storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, res1.Status);
			_storeBridge2 = StoreDriver.getBridge();
			_storeBridge2.GetStoreManagementService(_userBridge.getUserSession(), "hahaha");
			MarketAnswer res2 = _storeBridge2.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, res2.Status);
		}

		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
		}

		//TODO: make sure that cleanSession of store deletes the products as well as their stores.
		[TestCleanup]
		public void UserTestCleanUp()
		{
			_userBridge.CleanSession();
			_userBridge2?.CleanSession();
			_storeBridge.CleanSession();
			_storeBridge2?.CleanSession();
			_userBridge.CleanMarket();
		}
	}
}
