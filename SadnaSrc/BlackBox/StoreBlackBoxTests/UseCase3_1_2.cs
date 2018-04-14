using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

//TODO: change statuses.
namespace BlackBoxStoreTests
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
			_storeBridge.GetStoreShoppingService(_userBridge.getUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_storeManage1 = StoreManagementDriver.getBridge();
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
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
			//TODO: check in the stock to see if the product was indeed removed.
		}

		[TestMethod]
		public void ProductNotFoundInTheStore()
		{
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer res = _storeManage1.RemoveProduct("bambuu");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
		}

		[TestMethod]
		public void NoPermissionsToAddAProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.getUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.RemoveProduct("bamba");
			//Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
			//TODO: to complete this when lior changes the statuses.
			//TODO: to check the product was not added.
		}

		[TestMethod]
		public void NoSuchStore()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.getUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.RemoveProduct("bamba");
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
			_storeManage1.CleanSession();
			_userBridge2?.CleanSession();
			_storeBridge.CleanSession();
			_storeManage2?.CleanSession();
			_userBridge.CleanMarket();
		}
	}
}
