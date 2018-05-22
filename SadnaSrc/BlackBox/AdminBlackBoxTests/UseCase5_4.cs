using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.AdminBlackBoxTests
{
	[TestClass]
	public class UseCase5_4
	{
		private IUserBridge _adminSignInBridge;
		private IUserBridge _userBridge;
		private IUserBridge _userBridge2;
		private IAdminBridge _adminBridge;
		private IStoreShoppingBridge _storeShopping;
		private IStoreShoppingBridge _storeShopping2;
		private IStoreManagementBridge _managerBridge;
		private IStoreManagementBridge _managerBridge2;
		private IOrderBridge _orderBridge;
		private IOrderBridge _orderBridge2;
		private string userToCheck = "Pnina";
		private string storeToCheck = "blahblah";
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";
		private readonly string nonExistingUser = "Bore";
		private readonly string nonExistingStore = "Ben-Gurion";

		[TestInitialize]

		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            _adminBridge = AdminDriver.getBridge();

			CreateUser1();
			CreateUser2();
			CreateStoreBlahblah();
			CreateStoreBlahblah2();
			AddProductsToBlahblah();
			AddProductsToBlahblah2();
			User1AddToCart();
			User2AddToCart();
			User1MakeOrder();
			User2MakeOrder();
		}

		private void User2MakeOrder()
		{
			_orderBridge2 = OrderDriver.getBridge();
			_orderBridge2.GetOrderService(_userBridge2.GetUserSession());
			_orderBridge2.BuyItemFromImmediate("hello2", "blahblah", 2, 20, null);
			_orderBridge2.BuyItemFromImmediate("Goodbye2", "blahblah2", 2, 20, null);
		}

		private void User1MakeOrder()
		{
			_orderBridge = OrderDriver.getBridge();
			_orderBridge.GetOrderService(_userBridge.GetUserSession());
			_orderBridge.BuyItemFromImmediate("hello", "blahblah", 2, 10, null);
			_orderBridge.BuyItemFromImmediate("Goodbye", "blahblah2", 2, 10, null);
		}

		private void User2AddToCart()
		{
			_storeShopping2.AddProductToCart("blahblah", "hello2", 5);
			_storeShopping2.AddProductToCart("blahblah2", "Goodbye2", 10);
		}

		private void User1AddToCart()
		{
			_storeShopping.AddProductToCart("blahblah", "hello", 5);
			_storeShopping.AddProductToCart("blahblah2", "Goodbye", 3);
		}

		private void AddProductsToBlahblah2()
		{
			_managerBridge2 = StoreManagementDriver.getBridge();
			_managerBridge2.GetStoreManagementService(_userBridge2.GetUserSession(), "blahblah2");
			_managerBridge2.AddNewProduct("Goodbye", 10, "nice product", 8);
			_managerBridge2.AddNewProduct("Goodbye2", 20, "nice product2", 20);
		}

		private void AddProductsToBlahblah()
		{
			_managerBridge = StoreManagementDriver.getBridge();
			_managerBridge.GetStoreManagementService(_userBridge.GetUserSession(), "blahblah");
			_managerBridge.AddNewProduct("hello", 10, "nice product", 8);
			_managerBridge.AddNewProduct("hello2", 20, "nice product2", 20);
		}

		private void CreateStoreBlahblah2()
		{
			_storeShopping2 = StoreShoppingDriver.getBridge();
			_storeShopping2.GetStoreShoppingService(_userBridge2.GetUserSession());
			MarketAnswer res20 = _storeShopping2.OpenStore("blahblah2", "blah");
			Assert.AreEqual((int)OpenStoreStatus.Success, res20.Status);
		}

		private void CreateStoreBlahblah()
		{
			_storeShopping = StoreShoppingDriver.getBridge();
			_storeShopping.GetStoreShoppingService(_userBridge.GetUserSession());
			_storeShopping.OpenStore("blahblah", "blah");
		}

		private void CreateUser2()
		{
			_userBridge2 = UserDriver.getBridge();
			_userBridge2.EnterSystem();
			_userBridge2.SignUp("Maor", "vinget", "9999", "99999999");
		}

		private void CreateUser1()
		{
			_userBridge = UserDriver.getBridge();
			_userBridge.EnterSystem();
			_userBridge.SignUp("Pnina", "misholSusia", "852852", "77777777");
		}

		[TestMethod]
		public void SuccessHistoryPurchaseUser()
		{
			SignIn(adminName,adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
				"User: Pnina Product: Goodbye Store: blahblah2 Sale: Immediate Quantity: 2 Price: 20 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
				"User: Pnina Product: hello Store: blahblah Sale: Immediate Quantity: 2 Price: 20 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
		
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success,res.Status);
			for (int i = 0; i < purchaseUserHistory.Length; i++)
			{
				Assert.AreEqual(expectedHistory[i], purchaseUserHistory[i]);
			}
		}

		[TestMethod]
		public void SuccessHistoryPurchaseStore()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
			    "User: Maor Product: hello2 Store: blahblah Sale: Immediate Quantity: 2 Price: 40 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Pnina Product: hello Store: blahblah Sale: Immediate Quantity: 2 Price: 20 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
			
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, res.Status);
			for (int i = 0; i < purchaseUserHistory.Length; i++)
			{
				Assert.AreEqual(expectedHistory[i], purchaseUserHistory[i]);
			}

		}

		[TestMethod]
		public void NotAdminWrongUserNameUserHistory()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);

		}

		[TestMethod]
		public void NotAdminWrongUserNameStoreHistory()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);

		}

		[TestMethod]
		public void NotAdminWrongPasswordStoreHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);

		}

		[TestMethod]
		public void NotAdminWrongPasswordUserHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);

		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordStoreHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);
		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordUserHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, res.Status);
			Assert.IsNull(res.ReportList);

		}

		[TestMethod]
		public void UserNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(nonExistingUser);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound,res.Status);
			Assert.IsNull(res.ReportList);
		}

		[TestMethod]
		public void StoreNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.GetUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(nonExistingStore);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, _adminBridge.ViewPurchaseHistoryByStore(nonExistingStore).Status);
		}

		private void SignUp(ref IUserBridge _userBridge, string userName, string address, string password, string creditCard)
		{
			_userBridge = UserDriver.getBridge();
			_userBridge.EnterSystem();
			_userBridge.SignUp(userName, address, password, creditCard);
		}

		private void SignIn(string userName, string password)
		{
			_adminSignInBridge = UserDriver.getBridge();
			_adminSignInBridge.EnterSystem();
			_adminSignInBridge.SignIn(userName, password);
		}


		[TestCleanup]

		public void UserTestCleanUp()
		{
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();

        }
		

	}
}