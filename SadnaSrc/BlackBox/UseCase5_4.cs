using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox
{
	[TestClass]
	public class UseCase5_4
	{
		private IUserBridge _adminBridge;
		private string userToCheck = "Moshe";
		private string storeToCheck = "YYY";
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";
		private readonly string nonExistingUser = "Pnina";
		private readonly string nonExistingStore = "Pnin";

		[TestInitialize]

		public void MarketBuilder()
		{
			_adminBridge = new RealBridge();
		}


		[TestMethod]
		public void SuccessHistoryPurchaseUser()
		{
			SignIn(adminName,adminPass);
			_adminBridge.GetAdminService();
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(userToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
				"User: Moshe Product: Health Potion Store: XXX Sale: Immediate Quantity: 2 Price: 11.5 Date: Today",
				"User: Moshe Product: INT Potion Store: YYY Sale: Lottery Quantity: 2 Price: 8 Date: Yesterday",
				"User: Moshe Product: Mana Potion Store: YYY Sale: Lottery Quantity: 3 Price: 12 Date: Yesterday"
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
			_adminBridge.GetAdminService();
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(storeToCheck);
			string[] purchaseUserHistory = res.ReportList;
			string[] expectedHistory =
			{
				"User: Moshe Product: Mana Potion Store: YYY Sale: Lottery Quantity: 3 Price: 12 Date: Yesterday",
				"User: Moshe Product: INT Potion Store: YYY Sale: Lottery Quantity: 2 Price: 8 Date: Yesterday",
				"User: MosheYYY Product: STR Potion Store: YYY Sale: Immediate Quantity: 1 Price: 4 Date: Today"
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
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameStoreHistory()
		{
			SignIn("hello", adminPass);
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongPasswordStoreHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongPasswordUserHistory()
		{
			SignIn(adminName, "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordStoreHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, _adminBridge.ViewPurchaseHistoryByStore(storeToCheck).Status);
		}

		[TestMethod]
		public void NotAdminWrongUserNameAndPasswordUserHistory()
		{
			SignIn("Hello", "852963");
			_adminBridge.GetAdminService();
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, _adminBridge.ViewPurchaseHistoryByUser(userToCheck).Status);
		}

		[TestMethod]
		public void UserNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByUser(nonExistingUser);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound,_adminBridge.ViewPurchaseHistoryByUser(nonExistingUser).Status);
		}

		[TestMethod]
		public void StoreNotFound()
		{
			SignIn(adminName, adminPass);
			_adminBridge.GetAdminService();
			MarketAnswer res = _adminBridge.ViewPurchaseHistoryByStore(nonExistingStore);
			Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, _adminBridge.ViewPurchaseHistoryByStore(nonExistingStore).Status);
		}

		private void SignIn(string userName, string password)
		{
			_adminBridge.EnterSystem();
			_adminBridge.SignIn(userName, password);
		}


		[TestCleanup]

		public void UserTestCleanUp()
		{
			_adminBridge.CleanSession();
			_adminBridge.CleanMarket();
			
		}


	}
}
