using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace BlackBox.BlackBoxStoreTests
{
	[TestClass]
	public class UseCase3_3
	{
		
		private IUserBridge _bridgeSignUp;
		private IUserBridge _userToPromoteBridge;
		private IUserBridge _userToPromoteBridge2;
		private IUserBridge _signInBridge;
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManager1;
		private IStoreManagementBridge _storeManager2;
		private IUserBridge _adminBridge;
		
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _bridgeSignUp, "LAMA", "ANI TZRIHA", "121112", "85296363");
			SignUp(ref _userToPromoteBridge,"eurovision","France","852963","78945678");
			SignUp(ref _userToPromoteBridge2,"blah","NotNice","98989","88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res =_storeBridge.OpenStore("basush", "rezahhhhh");
			Assert.AreEqual((int)OpenStoreStatus.Success,res.Status);
			_storeManager1 = StoreManagementDriver.getBridge();
			_storeManager2 = null;
			_signInBridge = null;
			_adminBridge = null;
		}

		[TestMethod]
		public void StoreOwnerSucceededPromote()
		{

			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "basush");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
			//check if eurovision can promote someone himself - if not, he is not an owner
			SignIn("eurovision", "852963");
			_storeManager2 = StoreManagementDriver.getBridge();
			_storeManager2.GetStoreManagementService(_signInBridge.GetUserSession(),"basush");
			Assert.AreEqual((int)PromoteStoreStatus.Success, _storeManager2.PromoteToStoreManager("blah", "StoreOwner").Status);
		}

		[TestMethod]
		public void AdminSystemSucceededPromote()
		{
			_adminBridge = UserDriver.getBridge();
			_adminBridge.EnterSystem();
			_adminBridge.SignIn(adminName, adminPass);
			_storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(),"basush");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
			//check if eurovision can promote someone himself - if not, he is not an owner
			SignIn("eurovision", "852963");
			_storeManager2 = StoreManagementDriver.getBridge();
			_storeManager2.GetStoreManagementService(_signInBridge.GetUserSession(), "basush");
			Assert.AreEqual((int)PromoteStoreStatus.Success, _storeManager2.PromoteToStoreManager("blah", "StoreOwner").Status);

		}

		[TestMethod]
		public void PromotesHimselfToOwner()
		{
			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "basush");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("LAMA", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.PromoteSelf, res.Status);
		}

		[TestMethod]
		public void NoUserFoundToPromote()
		{
			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "basush");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("euro", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.NoUserFound, res.Status);
		}

		[TestMethod]
		public void InvalidStore()
		{
			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "mahar");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.InvalidStore, res.Status);
		}

		[TestMethod]
		public void NotOwnerTriesToPromoteToOwner()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(),"basush");
			MarketAnswer res = _storeManager1.PromoteToStoreManager("blah", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.NoAuthority,res.Status);

		}

		private void SignUp(ref IUserBridge userBridge,string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
		}

		private void SignIn(string name, string password)
		{
			_signInBridge = UserDriver.getBridge();
			_signInBridge.EnterSystem();
			_signInBridge.SignIn(name, password);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_storeBridge.CleanSession();
			_bridgeSignUp.CleanSession();
			_userToPromoteBridge.CleanSession();
			_userToPromoteBridge2.CleanSession();
			_storeManager1.CleanSession();
			_storeManager2?.CleanSession();
			_signInBridge?.CleanSession();
			_adminBridge?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}
	}
}