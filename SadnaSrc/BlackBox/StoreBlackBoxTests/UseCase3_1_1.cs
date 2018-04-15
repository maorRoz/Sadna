using System;
using BlackBox;
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
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManage1;
		private IUserBridge _userBridge2;
		private IUserBridge _userBridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _userBridge, "Pnina", "lo kef", "777777", "88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_userBridge.getUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_storeManage1 = StoreManagementDriver.getBridge();
			_userBridge2 = null;
		}

		[TestMethod]
		public void SuccessAddingProductToStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, result.Status);
			//TODO: check in the stock to see if the product was indeed added.
		}

		[TestMethod]
		public void ProductAlreadyExistsInStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			_storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 1);
			Assert.AreEqual((int)StoreEnum.ProductNotFound, result.Status);
			//TODO: after lior changes the statuses, change this status.
		}

		[TestMethod]
		public void NoPermissionsToAddAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage1.GetStoreManagementService(_userBridge2.getUserSession(), "lokef");
			MarketAnswer res = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.NoPremmision, res.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsNegative()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", -1);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsZero()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 0);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void StoreDoesntExist()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lamaaaa");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 0);
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
			_storeManage1.CleanSession();
			_userBridge.CleanMarket();
		}

	}
}
