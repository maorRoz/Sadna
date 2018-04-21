using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests

{
	[TestClass]
	public class UseCase3_1_3
	{
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManage1;
		private IStoreManagementBridge _storeManage2;
		private IUserBridge _userBridge2;
		private IUserBridge _userBridge;

		[TestInitialize]
		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            SignUp(ref _userBridge, "Pnina", "lo kef", "777777", "88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_userBridge.GetUserSession());
			_storeBridge.OpenStore("lokef", "li");
			_storeManage1 = StoreManagementDriver.getBridge();
			_storeManage1.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
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

			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba-osem base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}

		}

		[TestMethod]
		public void SuccessUpdatingBasePrice()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "102020");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);

			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 102020 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void SuccessUpdatingDescription()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "Description", "nice snack ++");
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack ++ , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}

		}

		[TestMethod]
		public void FailUpdatingPriceNegativeNumbers()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "-20");
			Assert.AreEqual((int)StoreEnum.UpdateProductFail, result2.Status);
			
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void FailUpdatingInvalidPriceNotAllDigits()
		{
			MarketAnswer result2 = _storeManage1.EditProduct("bamba", "BasePrice", "20abc");
			Assert.AreEqual((int)StoreEnum.UpdateProductFail, result2.Status);
			
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void FailUpdatingProductNameIsTaken()
		{
			MarketAnswer result2 = _storeManage1.AddNewProduct("bamba200", 100, "bad snack", 10);
			Assert.AreEqual((int)StoreEnum.Success, result2.Status);
			MarketAnswer result3 = _storeManage1.EditProduct("bamba", "Name", "bamba200");
			Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, result3.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult =
			{
				" name: bamba base price: 90 description: nice snack , Immediate , 30",
				" name: bamba200 base price: 100 description: bad snack , Immediate , 10"
			};
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void StoreDoesntExist()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.GetUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.EditProduct("bamba", "Name", "bamba500");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, res2.Status);

		}

		[TestMethod]
		public void NoPermissionsToEditProduct()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.GetUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.EditProduct("bambush", "BasePrice", "100");
			Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);

			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
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
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 60" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}

		}

		[TestMethod]
		public void ChangeProductsQuantityProductNotFound()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer res = _storeManage2.AddQuanitityToProduct("bambuuuu", 5);
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void ChangeProductsQuantityIsNegative()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.GetUserSession(), "lokef");
			MarketAnswer res = _storeManage2.AddQuanitityToProduct("bambuuuu", -10);
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}

		}

		[TestMethod]
		public void ChangeProductsQuantityNoUserPermissions()
		{
			SignUp(ref _userBridge2, "BASH", "lo kef2", "777777", "88888888");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge2.GetUserSession(), "lokef");
			MarketAnswer res2 = _storeManage2.AddQuanitityToProduct("bamba", 30);
			Assert.AreEqual((int)StoreEnum.NoPremmision,res2.Status);
			MarketAnswer stockAnswer = _storeBridge.ViewStoreStock("lokef");
			string[] actualResult = stockAnswer.ReportList;
			string[] expectedResult = { " name: bamba base price: 90 description: nice snack , Immediate , 30" };
			Assert.AreEqual(expectedResult.Length, actualResult.Length);
			for (int i = 0; i < actualResult.Length; i++)
			{
				Assert.AreEqual(expectedResult[i], actualResult[i]);
			}
		}

		[TestMethod]
		public void ChangeProductsQuantityStoreDoesntExist()
		{
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_userBridge.GetUserSession(), "hahaha");
			MarketAnswer res2 = _storeManage2.AddQuanitityToProduct("bamba", 30);
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
			_userBridge.CleanMarket();
		}

	}
}