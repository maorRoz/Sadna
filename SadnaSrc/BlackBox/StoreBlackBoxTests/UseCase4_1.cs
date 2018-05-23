using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase4_1
	{
		private IUserBridge _bridgeSignUp;
		private IUserBridge _userToPromoteBridge;
		private IUserBridge _userToPromoteBridge2;
		private IUserBridge _signInBridge;
		private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManager1;
		private IStoreManagementBridge _storeManager2;
		private IUserBridge _guestBridge;
		private IOrderBridge _orderBridge;
		private readonly string storeAction1 = "PromoteStoreAdmin";
		private readonly string storeAction2 = "ManageProducts";
		private readonly string storeAction3 = "DeclareDiscountPolicy";
		private readonly string storeAction4 = "ViewPurchaseHistory";

		private string product = "Ouch";

		[TestInitialize]
		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            SignUp(ref _bridgeSignUp, "Odin", "Valhalla", "121112", "85296363");
			SignUp(ref _userToPromoteBridge, "Thor", "Midgard", "121112", "78945678");
			SignUp(ref _userToPromoteBridge2, "Loki", "Somewhere Else", "121112", "88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res = _storeBridge.OpenStore("Volcano", "Iceland");
			Assert.AreEqual((int)OpenStoreStatus.Success, res.Status);
			_storeManager1 = StoreManagementDriver.getBridge();
			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
			_storeManager2 = null;
			_signInBridge = null;
			_guestBridge = null;
			_orderBridge = null;
		}


		[TestMethod]
		public void SuccessDoingPromoteStoreAdmin()
		{
			TryPromote("Thor", storeAction1, true);
			AssertActions(new bool[] {true, false, false, false});
		}

	
		[TestMethod]
		public void SuccessDoingManageProducts()
		{
			TryPromote("Thor", storeAction2, true);
			AssertActions(new bool[] { false, true, false, false });
		}

		[TestMethod]
		public void SuccessDefiningDiscountPolicy()
		{
			TryPromote("Thor", storeAction3, true);
			AssertActions(new bool[] { false, false, true, false });
		}

		[TestMethod]
		public void StoreOwnerSucceededPromoteViewPurchaseHistory()
		{
			TryPromote("Thor", storeAction4, true);
			AssertActions(new bool[] { false, false, false, true });
		}

		[TestMethod]
		public void StoreOwnerSucceededPromoteMultipleActions1()
		{
			TryPromote("Thor", storeAction2 + "," + storeAction3, true);
			AssertActions(new bool[] { false, true, true, false });
		}

		[TestMethod]
		public void StoreOwnerSucceededPromoteMultipleActions2()
		{
			TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3, true);
			AssertActions(new bool[] { true, true, true, false });
		}

		[TestMethod]
		public void StoreOwnerSucceededPromoteAllActions()
		{
			TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3 + "," + storeAction4, true);
			AssertActions(new bool[] { true, true, true, true });
		}


		[TestMethod]
		public void ShopperFailedPromotePromoteStoreAdmin()
		{

			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction1, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteManageProducts()
		{

			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction2, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteDeclareDiscountPolicy()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction3, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteViewPurchaseHistory()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction4, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteMultipleActions1()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction2 + "," + storeAction3, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteMultipleActions2()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction2 + "," + storeAction3 + "," + storeAction4, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void ShopperFailedPromoteAllActions()
		{
			_storeManager1.GetStoreManagementService(_userToPromoteBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3 + "," + storeAction4, false);
			AssertActions(new bool[] { false, false, false, false });
		}


		[TestMethod]
		public void GuestFailedPromotePromoteStoreAdmin()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction1, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteManageProducts()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction2, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteDeclareDiscountPolicy()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction3, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteViewPurchaseHistory()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction4, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteMultipleActions1()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction2 + "," + storeAction3, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteMultipleActions2()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction1 + "," + storeAction3 + "," + storeAction3, false);
			AssertActions(new bool[] { false, false, false, false });
		}

		[TestMethod]
		public void GuestFailedPromoteAllActions()
		{
			GuestEnter();
			_storeManager1.GetStoreManagementService(_guestBridge.GetUserSession(), "Volcano");
			TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3 + "," + storeAction4, false);
			AssertActions(new bool[] { false, false, false, false });
		}


		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
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

		private void TryPromote(string toPromote, string actions, bool success)
		{
			MarketAnswer res = _storeManager1.PromoteToStoreManager(toPromote, actions);
			if (success)
				Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
			else
				Assert.AreEqual((int)PromoteStoreStatus.NoAuthority, res.Status);
			SignIn(toPromote, "121112");
			_storeManager2 = StoreManagementDriver.getBridge();
			_storeManager2.GetStoreManagementService(_signInBridge.GetUserSession(), "Volcano");
		}

		private void AssertActions(bool[] permissions)
		{
			if (permissions[0])
			{
				MarketAnswer res1 = _storeManager2.PromoteToStoreManager("Loki", storeAction1);
				Assert.AreEqual((int)PromoteStoreStatus.Success, res1.Status);
			}

			else
			{
				MarketAnswer res1 = _storeManager2.PromoteToStoreManager("Loki", storeAction1);
				Assert.AreEqual((int)PromoteStoreStatus.NoAuthority, res1.Status);
			}
				

			if (permissions[1])
			{
				Assert.AreEqual((int)StoreEnum.Success, _storeManager2.AddNewProduct(product, 50, "tool", 5).Status);
				CheckProductAddedToStock();

				Assert.AreEqual((int)StoreEnum.Success, _storeManager2.EditProduct(product, "BasePrice", "3").Status);
				CheckProductEditedInStock();

				Assert.AreEqual((int)StoreEnum.Success, _storeManager2.RemoveProduct(product).Status);
				MarketAnswer stock2 = _storeBridge.ViewStoreStock("Volcano");
				Assert.AreEqual(0,stock2.ReportList.Length);

			}
			else
			{
				Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.AddNewProduct(product, 50, "tool", 5).Status);
				Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.EditProduct(product, "Price", "3").Status);
				Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.RemoveProduct(product).Status);
			}

			if (permissions[2])
			{
				_storeManager1.AddNewProduct("Lets", 10, "haha", 10);

				Assert.AreEqual((int)DiscountStatus.Success,
					_storeManager2.AddDiscountToProduct("Lets", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("20/04/2018"), 5, "VISIBLE", false).Status);
				MarketAnswer stock = _storeBridge.ViewStoreStock("Volcano");
				string[] receivedStock = stock.ReportList;
				string[] expectedStock =
				{
					" name: Lets base price: 10 description: haha Discount: {DiscountAmount: 5 Start Date: "+Convert.ToDateTime("14/04/2018").Date.ToString("d")+"" +
					" End Date: "+ Convert.ToDateTime("20/04/2018").Date.ToString("d")+" type is: visible} Purchase Way: Immediate Quantity: 10"
                };
				Assert.AreEqual(expectedStock.Length, receivedStock.Length);
				for (int i = 0; i < receivedStock.Length; i++)
				{
					Assert.AreEqual(expectedStock[i], receivedStock[i]);
				}

			}
			else
			{
				Assert.AreEqual((int)StoreEnum.NoPermission,
					_storeManager2.AddDiscountToProduct("Lets", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("20/04/2018"), 5, "VISIBLE", false).Status);

			}

			if (permissions[3])
			{
				CreateOrder();

				MarketAnswer purchaseHistory = _storeManager2.ViewStoreHistory();
				string[] received = purchaseHistory.ReportList;
				string[] expected =
				{
					"User: Odin Product: Yolo Store: Volcano Sale: Immediate Quantity: 2 Price: 10 Date: " +
				    DateTime.Now.Date.ToString("dd/MM/yyyy"),
				};

				Assert.AreEqual(expected.Length, received.Length);
				for (int i = 0; i < received.Length; i++)
				{
					Assert.AreEqual(expected[i],received[i]);
				}

			}
			else
			{
				CreateOrder();
				Assert.AreEqual((int)ManageStoreStatus.InvalidManager, _storeManager2.ViewStoreHistory().Status);
			}
		}

		private void CreateOrder()
		{
			_storeManager1.AddNewProduct("Yolo", 5, "Once", 10);
			_storeBridge.AddProductToCart("Volcano", "Yolo", 2);
			_orderBridge = OrderDriver.getBridge();
			_orderBridge.GetOrderService(_bridgeSignUp.GetUserSession());
			_orderBridge.BuyEverythingFromCart(new string[]{null});
		}

		private void CheckProductEditedInStock()
		{
			MarketAnswer stock1 = _storeBridge.ViewStoreStock("Volcano");
			string[] receivedStock1 = stock1.ReportList;
			string[] expectedStock1 =
			{
                " name: Ouch base price: 3 description: tool Discount: {null} Purchase Way: Immediate Quantity: 5"
            };
			Assert.AreEqual(expectedStock1.Length, receivedStock1.Length);
			for (int i = 0; i < expectedStock1.Length; i++)
			{
				Assert.AreEqual(expectedStock1[i], receivedStock1[i]);
			}

		}

		private void CheckProductAddedToStock()
		{
			MarketAnswer stock = _storeBridge.ViewStoreStock("Volcano");
			string[] receivedStock = stock.ReportList;
			string[] expectedStock =
			{
                " name: Ouch base price: 50 description: tool Discount: {null} Purchase Way: Immediate Quantity: 5"

            };
			Assert.AreEqual(expectedStock.Length, receivedStock.Length);
			for (int i = 0; i < expectedStock.Length; i++)
			{
				Assert.AreEqual(expectedStock[i], receivedStock[i]);
			}
		}

		private void GuestEnter()
		{
			_guestBridge = UserDriver.getBridge();
			_guestBridge.EnterSystem();
		}


		[TestCleanup]
		public void UserTestCleanUp()
		{
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();
        }
	}
}
