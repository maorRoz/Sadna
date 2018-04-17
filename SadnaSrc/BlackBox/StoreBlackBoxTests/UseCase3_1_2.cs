using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_1_2
	{
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManage1;
		private IStoreManagementBridge _storeManage2;
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
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer res1 = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, res1.Status);
			_userBridge2 = null;
			_storeManage2 = null;
		}

		[TestMethod]
		public void SuccessRemovingAProducFromStore()
		{
			MarketAnswer result2 = _storeManage1.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			Assert.AreEqual(0, actualResult.Length);
		}

		[TestMethod]
		public void ProductNotFoundInTheStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer res = _storeManage1.RemoveProduct("bambuu");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
		}

		[TestMethod]
		public void NoPermissionsToRemoveAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.GetUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			//didn't succeed in removing the product, there is still one product
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}

		}

		[TestMethod]
		public void NoSuchStore()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.GetUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.RemoveProduct("bamba");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, res2.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			//didn't succeed in removing the product, there is still one product
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
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
			_storeManage1.CleanSession();
			_userBridge2?.CleanSession();
			_storeBridge.CleanSession();
			_storeManage2?.CleanSession();
			_userBridge.CleanMarket();
		}
	}
}
