using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_3
	{
		
		private IUserBridge _bridgeSignUp;
		private IUserBridge _userToPromoteBridge;
		private IUserBridge _userToPromoteBridge2;
		private IStoreBridge _storeBridge;
		private IStoreBridge _promoteBridge;
		
		private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";

		[TestInitialize]
		public void MarketBuilder()
		{
			SignUp(ref _bridgeSignUp, "LAMA","ANI TZRIHA","121112","85296363");
			SignUp(ref _userToPromoteBridge,"eurovision","France","852963","78945678");
			SignUp(ref _userToPromoteBridge2,"blah","NotNice","98989","88888888");
			_storeBridge = StoreDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.getUserSession());
			MarketAnswer res =_storeBridge.OpenStore("basush", "rezahhhhh");
			Assert.AreEqual((int)OpenStoreStatus.Success,res.Status);
			_promoteBridge = null;
		}

		[TestMethod]
		public void StoreOwnerSucceededPromote()
		{
			_storeBridge.GetStoreManagementService(_bridgeSignUp.getUserSession(), "basush");
			_storeBridge.PromoteToStoreManager("eurovision", "StoreOwner");
			//check if eurovision can promote someone himself - if not, he is not an owner
			_promoteBridge = StoreDriver.getBridge();
			_promoteBridge.GetStoreManagementService(_userToPromoteBridge.getUserSession(),"basush");
			Assert.AreEqual((int)ManageStoreStatus.Success, _promoteBridge.PromoteToStoreManager("blah", "StoreOwner"));
		}

		[TestMethod]
		public void AdminSystemSucceededPromote()
		{


		}



		private void SignUp(ref IUserBridge userBridge,string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			_bridgeSignUp.EnterSystem();
			_bridgeSignUp.SignUp(name, address, password, creditCard);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_storeBridge.CleanSession();
			_bridgeSignUp.CleanSession();
			_userToPromoteBridge.CleanSession();
			_userToPromoteBridge2.CleanSession();
			_promoteBridge?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}
	}
}
