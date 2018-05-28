using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace BlackBox.UserBlackBoxTests
{
	[TestClass]
	public class UseCase1_6_1
	{
		private IUserBridge _bridge;
		private IUserBridge _bridge1;
		private IUserBridge _bridgeGuest;
		private IUserBridge _bridgeGuest2;
		private IStoreShoppingBridge _storeBridge;
		private IStoreShoppingBridge _storeGuestBridge;
		private IStoreManagementBridge _storeManage1;
		private IStoreManagementBridge _storeManage2;
		private string usertoSignIn = "vika";
		private string userPass = "6666";

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
		public void SuccessChangeQuantityOfProductRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			MarketAnswer res2 = _bridge.EditCartItem("BlahStore", "bisli", 1, 200);
			Assert.AreEqual((int)EditCartItemStatus.Success, res2.Status);
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 6 Unit Price : 200 Final Price : 1200",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void SuccessChangeQuantityOfProductGuest()
		{
			MarketAnswer res2 = _bridgeGuest.EditCartItem("BlahStore2", "doritos", 1, 30);
			Assert.AreEqual((int)EditCartItemStatus.Success, res2.Status);
			MarketAnswer res = _bridgeGuest.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
				"Name : cheaps Store : BlahStore Quantity : 2 Unit Price : 20 Final Price : 40",
				"Name : doritos Store : BlahStore2 Quantity : 4 Unit Price : 30 Final Price : 120"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void DidntEnterSystemChangeQuantity()
		{
			_bridgeGuest2 = UserDriver.getBridge();
			MarketAnswer res = _bridgeGuest2.EditCartItem("BlahStore", "bisli", 1, 200);
			Assert.AreEqual((int)EditCartItemStatus.DidntEnterSystem, res.Status);
		}

		[TestMethod]
		public void NoItemFoundChangeQuantityStoreNameRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("BLAH", "bisli", 5, 25.0).Status);
			//didn't edit, everything is the same
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void NoItemFoundChangeQuantityStoreNameGuest()
		{
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridgeGuest.EditCartItem("Blah", "cheaps", 1, 20).Status);
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
		public void NoItemFoundChangeQuantityProductNameRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("BlahStore", "Gunoooo", 5, 25.0).Status);
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void NoItemFoundChangeQuantityProductNameGuest()
		{
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridgeGuest.EditCartItem("BlahStore", "Gunoooo", 5, 25.0).Status);
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
		public void NoItemFoundChangeQuantityUnitPriceRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("M", "Gun", 5, 50).Status);
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void NoItemFoundChangeQuantityUnitPriceGuest()
		{
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridgeGuest.EditCartItem("M", "Gun", 5, 50).Status);
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
		public void NegativeQuantityRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridge.EditCartItem("BlahStore", "bisli", -200, 200).Status);
			//check the item wasn't changed
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90",
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void NegativeQuantityGuest()
		{
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridgeGuest.EditCartItem("BlahStore", "cheaps", -200, 20).Status);
			//check the item wasn't changed
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
		public void ZeroQuantityRegisterUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(usertoSignIn, userPass);
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridge.EditCartItem("BlahStore", "bisli", -5, 200).Status);
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success, res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
			    "Name : bisli Store : BlahStore Quantity : 5 Unit Price : 200 Final Price : 1000",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
			}
		}

		[TestMethod]
		public void ZeroQuantityGuest()
		{
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridgeGuest.EditCartItem("BlahStore", "cheaps", -2, 20).Status);
			//check the item wasn't changed
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

		[TestCleanup]
		public void UserTestCleanUp()
		{
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();
        }
	}
}
