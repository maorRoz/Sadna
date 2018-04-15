using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxStoreTests
{

	[TestClass]
	public class UseCase3_1_1
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
			_storeBridge.GetStoreShoppingService(_userBridge.GetUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_storeManage1 = StoreManagementDriver.getBridge();
			_userBridge2 = null;
		}

		[TestMethod]
		public void SuccessAddingProductToStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, result.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30"};
			Assert.AreEqual(expectedResult.Length,actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void ProductAlreadyExistsInStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			_storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 1);
			Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, result.Status);
		}

		[TestMethod]
		public void NoPermissionsToAddAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage1.GetStoreManagementService(_userBridge2.GetUserSession(), "lokef");
			MarketAnswer res = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.NoPremmision, res.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsNegative()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", -1);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void QuantityOfProductAddedIsZero()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 0);
			Assert.AreEqual((int)StoreEnum.quantityIsNegatie, result.Status);
		}

		[TestMethod]
		public void StoreDoesntExist()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lamaaaa");
			MarketAnswer result = _storeManage1.AddNewProduct("bamba", 80, "nice snack", 0);
			Assert.AreEqual((int)StoreEnum.StoreNotExists, result.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			//didn't succeed in removing the product, there is still one product
			string[] expectedResult = { };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
		}

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
