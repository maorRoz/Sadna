using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxAdminTests
{
	[TestClass]
	public class UseCase5_4
	{
		private IUserBridge _adminSignInBridge;
		private IUserBridge _userBridge;
		private IAdminBridge _adminBridge;
		private IStoreShoppingBridge _storeShopping;
		private IStoreManagementBridge _managerBridge;
		private IOrderBridge _orderBridge;
		private string userToCheck = "Arik1";
		private string storeToCheck = "Y";
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";
		private readonly string nonExistingUser = "Pnina";
		private readonly string nonExistingStore = "Pnin";

		[TestInitialize]

		public void MarketBuilder()
		{
			_adminBridge = AdminDriver.getBridge();
			_storeShopping = StoreShoppingDriver.getBridge();
			//SignUp(ref _userBridge, "Pnina", "misholSusia", "852852", "77777777");
			_userBridge = UserDriver.getBridge();
			_userBridge.EnterSystem();
			_userBridge.SignUp("Pnina", "misholSusia", "852852", "77777777");


			_storeShopping.GetStoreShoppingService(_userBridge.getUserSession());
			_storeShopping.OpenStore("blahblah", "blah");
			_managerBridge = StoreManagementDriver.getBridge();
			_managerBridge.GetStoreManagementService(_userBridge.getUserSession(),"blahblah");
			_managerBridge.AddNewProduct("hello", 10, "nice product", 8);
			_managerBridge.AddNewProduct("hello2", 20, "nice product2", 20);
			_storeShopping.AddProductToCart("blahblah", "hello", 5);
			_storeShopping.AddProductToCart("blahblah", "hello2", 10);
			_orderBridge = OrderDriver.getBridge();
			_orderBridge.GetOrderService(_userBridge.getUserSession());
			MarketAnswer res = _orderBridge.BuyItemFromImmediate("hello", "blahblah", 2, 10);
			Assert.AreEqual(OrderStatus.Success, res.Status);
		}


		[TestMethod]
		public void SuccessHistoryPurchaseUser()
		{
			SignIn(adminName,adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
                "User: Arik1 Product: Health Potion Store: X Sale: Immediate Quantity: 2 Price: 11.5 Date: Today",
                "User: Arik1 Product: INT Potion Store: Y Sale: Lottery Quantity: 2 Price: 8 Date: Yesterday",
			    "User: Arik1 Product: Mana Potion Store: Y Sale: Lottery Quantity: 3 Price: 12 Date: Yesterday"
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
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
                "User: Arik1 Product: Mana Potion Store: Y Sale: Lottery Quantity: 3 Price: 12 Date: Yesterday",
                "User: Arik1 Product: INT Potion Store: Y Sale: Lottery Quantity: 2 Price: 8 Date: Yesterday",
			    "User: Arik3 Product: STR Potion Store: Y Sale: Immediate Quantity: 1 Price: 4 Date: Today"
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
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameStoreHistory()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongPasswordStoreHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongPasswordUserHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordStoreHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordUserHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void UserNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(nonExistingUser);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound,_adminBridge.ViewPurchaseHistoryByUser(nonExistingUser).Status);
		}

		[TestMethod]
		public void StoreNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService(_adminSignInBridge.getUserSession());
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


	/*	[TestCleanup]

		public void UserTestCleanUp()
		{
			_adminSignInBridge.CleanSession();
			_adminSignInBridge.CleanMarket();
			
		}
		*/

	}
}
