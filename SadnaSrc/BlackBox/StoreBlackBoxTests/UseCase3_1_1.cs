using System;
using BlackBox;
using BlackBox.StoreBlackBoxTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

//TODO: change statuses.
namespace BlackBoxStoreTests
{
	//TODO: to change the status of store enum when it's ready
	//TODO: pay attention, dear pnina to ProductNofFound and NotAvailableProductName
	//TODO: which appear to be the same.
	[TestClass]
	public class UseCase3_1
	{
		private IStoreBridge _storeBridge;
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
		}

		[TestMethod]
		public void SuccessAddingProductToStore()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, result.Status);
			//TODO: check in the stock to see if the product was indeed added.
		}

		[TestMethod]
		public void ProductAlreadyExistsInStore()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			_storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			MarketAnswer result = _storeBridge.AddNewProduct("bamba", 80, "nice snack", 1);
			Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, result.Status);
			//TODO: after lior changes the statuses, change this status.
		}

		[TestMethod]
		public void NoPermissionsToAddAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeBridge.GetStoreManagementService(_userBridge2.getUserSession(), "lokef");
			MarketAnswer res = _storeBridge.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.NoPremmision, res.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsNegative()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeBridge.AddNewProduct("bamba", 80, "nice snack", -1);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsZero()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeBridge.AddNewProduct("bamba", 80, "nice snack", 0);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void StoreDoesntExist()
		{
			_storeBridge.GetStoreManagementService(_userBridge.getUserSession(), "lamaaaa");
			MarketAnswer result = _storeBridge.AddNewProduct("bamba", 80, "nice snack", 0);
			Assert.AreEqual((int)StoreEnum.StoreNotExists, result.Status);
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
			_userBridge.CleanMarket();
		}

	}
}
