using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.UserBlackBoxTests
{
	[TestClass]
	public class UseCase1_6
	{
		private IUserBridge _bridge;
		private IUserBridge _bridge1;
		private IUserBridge _bridgeGuest;
		private IUserBridge _bridgeGuest2;
		private IStoreShoppingBridge _storeBridge;
		private IStoreShoppingBridge _storeGuestBridge;
		private IStoreManagementBridge _storeManage1;
		private IStoreManagementBridge _storeManage2;

		[TestInitialize]
		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            _bridge = UserDriver.getBridge();
			_bridge1 = UserDriver.getBridge();
			_bridgeGuest = UserDriver.getBridge();
			_bridgeGuest.EnterSystem();
			_bridge1.EnterSystem();
			_bridge1.SignUp("vika", "arad", "6666", "11111111");
			CreateStore1AndProducts();
			CreateStore2AndProducts();
			User1AddToCart();
			GuestAddToCart();
			_storeGuestBridge = null;
			_bridgeGuest2 = null;
		}

		private void GuestAddToCart()
		{
			_storeGuestBridge = StoreShoppingDriver.getBridge();
			_storeGuestBridge.GetStoreShoppingService(_bridgeGuest.GetUserSession());
			_storeGuestBridge.AddProductToCart("BlahStore", "cheaps", 2);
			_storeGuestBridge.AddProductToCart("BlahStore2", "doritos", 3);
		}

		private void CreateStore1AndProducts()
		{
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridge1.GetUserSession());
			_storeBridge.OpenStore("BlahStore", "BlahStreet");
			_storeManage1 = StoreManagementDriver.getBridge();
			_storeManage1.GetStoreManagementService(_bridge1.GetUserSession(), "BlahStore");
			_storeManage1.AddNewProduct("bisli", 200, "yammy!", 5);
			_storeManage1.AddNewProduct("cheaps", 20, "yammy2!", 80);
		}

		private void CreateStore2AndProducts()
		{
			_storeBridge.OpenStore("BlahStore2", "BlahStreet2");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_bridge1.GetUserSession(), "BlahStore2");
			_storeManage2.AddNewProduct("doritos", 30, "yammy3!", 30);
		}

		private void User1AddToCart()
		{
			_storeBridge.AddProductToCart("BlahStore", "bisli", 5);
			_storeBridge.AddProductToCart("BlahStore2", "doritos", 3);
		}

		[TestMethod]
		public void SuccessViewCartOfRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn("vika","6666");
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success,res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90",
                "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000"
			};
            Assert.AreEqual(cartItemsExpected.Length,cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i],cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void SuccessViewCartGuest()
		{
			
			MarketAnswer res = _bridgeGuest.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
				"Name : cheaps Store : BlahStore Quantity : 2 Unit Price : 20 Final Price : 40",
				"Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}
		}

		[TestMethod]
		public void DidntEnterSystemViewCart()
		{
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int) ViewCartStatus.DidntEnterSystem, res.Status);
			Assert.IsNull(res.ReportList);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridge.CleanSession();
			_bridge1.CleanSession();
			_bridgeGuest2?.CleanSession();
			_storeBridge.CleanSession();
			_storeManage1.CleanSession();
			_storeManage2.CleanSession();
			_bridgeGuest.CleanSession();
			_storeGuestBridge?.CleanSession();
			_bridge.CleanMarket();
		}
		
	}
}
