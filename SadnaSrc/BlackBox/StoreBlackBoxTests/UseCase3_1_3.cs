using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

//TODO: change statuses.
namespace BlackBoxStoreTests

{
	[TestClass]
	public class UseCase3_1_3
	{
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManage1;
		private IStoreManagementBridge _storeManage2;
		private IUserBridge _userBridge2;
		private IUserBridge _userBridge;
		private IStoreShoppingBridge _storeBridge2;

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _userBridge, "Pnina", "lo kef", "777777", "88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_userBridge.getUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_storeManage1 = StoreManagementDriver.getBridge();
			_storeManage1.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer result1 = _storeManage1.AddNewProduct("bamba", 90, "nice snack", 30);
			Assert.AreEqual((int)StoreEnum.Success, result1.Status);
			_userBridge2 = null;
			_storeManage2 = null;
		}

		[TestMethod]
		public void SuccessUpdatingProductName()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "Name", "bamba-osem");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			//TODO: check in the stock to see if the product's name was changed.

		}

		[TestMethod]
		public void SuccessUpdatingBasePrice()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "102020");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			//TODO: check in the stock to see if the product's price was changed.
		}

		[TestMethod]
		public void SuccessUpdatingDescription()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "Description", "nice snack ++");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			//TODO: check in the stock to see if the product's description was changed.
		}

		[TestMethod]
		public void FailUpdatingPriceNegativeNumbers()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "-20");
			Assert.AreEqual((int)StoreEnum.UpdateProductFail, result2.Status);
			//TODO: check in the stock to see that the product's price wasn't changed.
		}

		[TestMethod]
		public void FailUpdatingInvalidPriceNotAllDigits()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "20abc");
			Assert.AreEqual((int)StoreEnum.UpdateProductFail, result2.Status);
			//TODO: check in the stock to see that the product's price wasn't changed.
		}

		[TestMethod]
		public void FailUpdatingProductNameIsTaken()
		{
			MarketAnswer result2 = _storeManage1.AddNewProduct("bamba200", 100, "bad snack", 10);
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			MarketAnswer result3 = _storeManage1.EditProduct("bamba", "Name", "bamba200");
			Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, result3.Status);
			//TODO: check in the stock to see that the product's name wasn't changed.
		}

		[TestMethod]
		public void StoreDoesntExist()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.getUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.EditProduct("bamba", "Name", "bamba500");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, res2.Status);

		}

		[TestMethod]
		public void NoPermissionsToEditProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.getUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.EditProduct("bambush", "BasePrice", "100");
			//Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
			//TODO: to complete this when lior changes the statuses.
			//TODO: to check the product was not edited.
		}

		[TestMethod]
		public void ProductToEditWasntFound()
		{
			MarketAnswer res2 = _storeManage1.EditProduct("bambuuu", "Name", "bambee");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res2.Status);
		}

		[TestMethod]
		public void ChangeQuantityOfProductSuccessfully()
		{
			MarketAnswer result2 = _storeManage1.AddQuanitityToProduct("bamba", 30);
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			//TODO: check in the stock to see if the product's quantity was changed.
		}

		[TestMethod]
		public void ChangeProductsQuantityProductNotFound()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.getUserSession(), "lokef");
			MarketAnswer res = _storeManage2.AddQuanitityToProduct("bambuuuu", 5);
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
		}

		[TestMethod]
		public void ChangeProductsQuantityIsNegative()
		{
			//TODO: according to lior, the quantity itself should be more or equal to 0.
			//TODO: I'm not sure, so check it and them implement this function.
		}

		[TestMethod]
		public void ChangeProductsQuantityNoUserPermissions()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.getUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.AddQuanitityToProduct("bamba", 30);
			//Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
			//TODO: to complete this when lior changes the statuses.
			//TODO: to check the product was not edited.
		}

		[TestMethod]
		public void ChangeProductsQuantityStoreDoesntExist()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.getUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.AddQuanitityToProduct("bamba", 30);
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
			_storeBridge2?.CleanSession();
			_userBridge.CleanMarket();
		}

	}
}
