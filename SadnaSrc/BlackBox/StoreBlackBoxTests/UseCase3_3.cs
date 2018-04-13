using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_3
	{
		
		private IUserBridge _bridgeSignUp;
		private IUserBridge _userToPromoteBridge;
		private IUserBridge _userToPromoteBridge2;
		private IUserBridge _signInBridge;
		private IStoreBridge _storeBridge;
		private IStoreBridge _promoteBridge;
		private IUserBridge _adminBridge;
		
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _bridgeSignUp, "LAMA", "ANI TZRIHA", "121112", "85296363");
			SignUp(ref _userToPromoteBridge,"eurovision","France","852963","78945678");
			SignUp(ref _userToPromoteBridge2,"blah","NotNice","98989","88888888");
			_storeBridge = StoreDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.getUserSession());
			MarketAnswer res =_storeBridge.OpenStore("basush", "rezahhhhh");
			Assert.AreEqual((int)OpenStoreStatus.Success,res.Status);
			_promoteBridge = null;
			_signInBridge = null;
			_adminBridge = null;
		}

		[TestMethod]
		public void StoreOwnerSucceededPromote()
		{
			
			_storeBridge.GetStoreManagementService(_bridgeSignUp.getUserSession(), "basush");
			MarketAnswer res = _storeBridge.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
			//check if eurovision can promote someone himself - if not, he is not an owner
			SignIn("eurovision", "852963");
			_promoteBridge = StoreDriver.getBridge();
			_promoteBridge.GetStoreManagementService(_signInBridge.getUserSession(),"basush");
			Assert.AreEqual((int)PromoteStoreStatus.Success, _promoteBridge.PromoteToStoreManager("blah", "StoreOwner").Status);
		}

		[TestMethod]
		public void AdminSystemSucceededPromote()
		{
			_adminBridge = UserDriver.getBridge();
			_adminBridge.EnterSystem();
			_adminBridge.SignIn(adminName, adminPass);
			_storeBridge.GetStoreManagementService(_adminBridge.getUserSession(),"basush");
			MarketAnswer res = _storeBridge.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
			//check if eurovision can promote someone himself - if not, he is not an owner
			SignIn("eurovision", "852963");
			_promoteBridge = StoreDriver.getBridge();
			_promoteBridge.GetStoreManagementService(_signInBridge.getUserSession(), "basush");
			Assert.AreEqual((int)PromoteStoreStatus.Success, _promoteBridge.PromoteToStoreManager("blah", "StoreOwner").Status);

		}

		[TestMethod]
		public void PromotesHimselfToOwner()
		{
			_storeBridge.GetStoreManagementService(_bridgeSignUp.getUserSession(), "basush");
			MarketAnswer res = _storeBridge.PromoteToStoreManager("LAMA", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.PromoteSelf, res.Status);
		}

		[TestMethod]
		public void NoUserFoundToPromote()
		{
			_storeBridge.GetStoreManagementService(_bridgeSignUp.getUserSession(), "basush");
			MarketAnswer res = _storeBridge.PromoteToStoreManager("euro", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.NoUserFound, res.Status);
		}

		[TestMethod]
		public void InvalidStore()
		{
			_storeBridge.GetStoreManagementService(_bridgeSignUp.getUserSession(), "mahar");
			MarketAnswer res = _storeBridge.PromoteToStoreManager("eurovision", "StoreOwner");
			Assert.AreEqual((int)PromoteStoreStatus.InvalidStore, res.Status);
		}

		private void SignUp(ref IUserBridge userBridge,string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			MarketAnswer ans = userBridge.SignUp(name, address, password, creditCard);
			Assert.AreEqual((int)SignUpStatus.Success,ans.Status);
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
			_storeBridge?.CleanSession();
			_bridgeSignUp.CleanSession();
			_userToPromoteBridge?.CleanSession();
			_userToPromoteBridge2?.CleanSession();
			_promoteBridge?.CleanSession();
			_signInBridge?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}
	}
}
