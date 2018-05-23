using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase3_4
	{
		
		private IUserBridge _bridgeSignUp;
		private IUserBridge _userToPromoteBridge;
		private IUserBridge _userToPromoteBridge2;
	    private IUserBridge _signInBridge;
        private IStoreShoppingBridge _storeBridge;
		private IStoreManagementBridge _storeManager1;
		private IStoreManagementBridge _storeManager2;
		private IUserBridge _adminBridge;
	    private IUserBridge _guestBridge;

        private readonly string adminName = "Arik1";
		private readonly string adminPass = "123";

	    private readonly string storeAction1 = "PromoteStoreAdmin";
	    private readonly string storeAction2 = "ManageProducts";
	    private readonly string storeAction3 = "DeclareDiscountPolicy";
	    private readonly string storeAction4 = "ViewPurchaseHistory";

	    private readonly string product = "NewProduct1";

        [TestInitialize]
		public void MarketBuilder()
		{
		    MarketDB.Instance.InsertByForce();
            SignUp(ref _bridgeSignUp, "Odin", "Valhalla", "121112", "85296363");
			SignUp(ref _userToPromoteBridge,"Thor","Midgard", "121112", "78945678");
			SignUp(ref _userToPromoteBridge2,"Loki","Somewhere Else", "121112", "88888888");
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res =_storeBridge.OpenStore("Volcano", "Iceland");
			Assert.AreEqual((int)OpenStoreStatus.Success,res.Status);
			_storeManager1 = StoreManagementDriver.getBridge();
            _storeManager2 = null;
			_signInBridge = null;
		    _adminBridge = null;
		    _guestBridge = null;
        }

        /*
         * Store owner tests
         */
		[TestMethod]
		public void StoreOwnerSucceededPromotePromoteStoreAdmin()
		{

			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
		    TryPromote("Thor", storeAction1, true);
            AssertActions(new bool[]{true, false, false, false});
        }

        [TestMethod]
	    public void StoreOwnerSucceededPromoteManageProducts()
	    {

	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction2, true);
	        AssertActions(new bool[] { false, true, false, false });
        }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteDeclareDiscountPolicy()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
            TryPromote("Thor", storeAction3, true);
	        AssertActions(new bool[] { false, false, true, false });
        }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteViewPurchaseHistory()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
            TryPromote("Thor", storeAction4, true);
	        AssertActions(new bool[] { false, false, false, true });
        }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteMultipleActions1()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction2+","+storeAction3, true);
	        AssertActions(new bool[] { false, true, true, false });
	    }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteMultipleActions2()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3, true);
	        AssertActions(new bool[] { true, true, true, false });
	    }

        [TestMethod]
	    public void StoreOwnerSucceededPromoteAllActions()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3 + "," + storeAction4, true);
	        AssertActions(new bool[] { true, true, true, true });
	    }

        /*
         * System admin tests
         */

	    [TestMethod]
	    public void AdminSucceededPromotePromoteStoreAdmin()
	    {
            AdminSignIn();
	        _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction1, true);
	        AssertActions(new bool[] { true, false, false, false });
	    }

	    [TestMethod]
	    public void AdminSucceededPromoteManageProducts()
	    {
	        AdminSignIn();
            _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction2, true);
	        AssertActions(new bool[] { false, true, false, false });
	    }

	    [TestMethod]
	    public void AdminSucceededPromoteDeclareDiscountPolicy()
	    {
	        AdminSignIn();
            _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction3, true);
	        AssertActions(new bool[] { false, false, true, false });
	    }

	    [TestMethod]
	    public void AdminSucceededPromoteViewPurchaseHistory()
	    {
	        AdminSignIn();
            _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction4, true);
	        AssertActions(new bool[] { false, false, false, true });
	    }

	    [TestMethod]
	    public void AdminSucceededPromoteMultipleActions1()
	    {
	        AdminSignIn();
            _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction2 + "," + storeAction3, true);
	        AssertActions(new bool[] { false, true, true, false });
	    }

	    [TestMethod]
	    public void AdminSucceededPromoteMultipleActions2()
	    {
	        AdminSignIn();
	        _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction4, true);
	        AssertActions(new bool[] { true, true, false, true });
	    }

        [TestMethod]
	    public void AdminSucceededPromoteAllActions()
	    {
	        AdminSignIn();
            _storeManager1.GetStoreManagementService(_adminBridge.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction1 + "," + storeAction2 + "," + storeAction3 + "," + storeAction4, true);
	        AssertActions(new bool[] { true, true, true, true });
	    }

	    /*
         * Regular user tests
         */

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

	    /*
         * Guest tests
         */

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

        

        [TestCleanup]
		public void UserTestCleanUp()
		{
		    MarketDB.Instance.CleanByForce();
		    MarketYard.CleanSession();
        }

        /*
         * Private helper functions
         */

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

	    private void AdminSignIn()
	    {
	        _adminBridge = UserDriver.getBridge();
	        _adminBridge.EnterSystem();
	        _adminBridge.SignIn(adminName, adminPass);
	    }

	    private void GuestEnter()
	    {
	        _guestBridge = UserDriver.getBridge();
	        _guestBridge.EnterSystem();
	    }

        private void TryPromote(string toPromote, string actions, bool success)
	    {
	        MarketAnswer res = _storeManager1.PromoteToStoreManager(toPromote, actions);
            if(success)
	            Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
            else
                Assert.AreEqual((int)PromoteStoreStatus.NoAuthority, res.Status);
	        SignIn(toPromote, "121112");
	        _storeManager2 = StoreManagementDriver.getBridge();
	        _storeManager2.GetStoreManagementService(_signInBridge.GetUserSession(), "Volcano");
	    }

	    private void AssertActions(bool[] permissions)
	    {
            if(permissions[0])
	            Assert.AreEqual((int)PromoteStoreStatus.Success, _storeManager2.PromoteToStoreManager("Loki", storeAction1).Status);
            else
                Assert.AreEqual((int)PromoteStoreStatus.NoAuthority, _storeManager2.PromoteToStoreManager("Loki", storeAction1).Status);

	        if (permissions[1])
	        {
	            Assert.AreEqual((int)StoreEnum.Success, _storeManager2.AddNewProduct(product, 50, "tool", 5).Status);
	            Assert.AreEqual((int)StoreEnum.Success, _storeManager2.EditProduct(product, "BasePrice", "3").Status);
	            Assert.AreEqual((int)StoreEnum.Success, _storeManager2.RemoveProduct(product).Status);
            }
	        else
	        {
	            Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.AddNewProduct(product, 50, "tool", 5).Status);
	            Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.EditProduct(product, "Price", "3").Status);
	            Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.RemoveProduct(product).Status);
            }

	        if (permissions[2])
	        {
	            Assert.AreEqual((int)DiscountStatus.ProductNotFound,
	                _storeManager2.AddDiscountToProduct("Product", DateTime.Today, DateTime.Today.AddDays(3), 50,
	                    "VISIBLE", true).Status);
	            Assert.AreEqual((int)StoreEnum.ProductNotFound, _storeManager2.EditDiscount(product, "DiscountAmount", "20").Status);
	            Assert.AreEqual((int)StoreEnum.ProductNotFound, _storeManager2.RemoveDiscountFromProduct(product).Status);
            }
	        else
	        {
	            Assert.AreEqual((int)StoreEnum.NoPermission,
	                _storeManager2.AddDiscountToProduct("Product", DateTime.Today, DateTime.Today.AddDays(3), 50,
	                    "VISIBLE", true).Status);
	            Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.EditDiscount(product, "DiscountAmount", "20").Status);
	            Assert.AreEqual((int)StoreEnum.NoPermission, _storeManager2.RemoveDiscountFromProduct(product).Status);
            }

	        if (permissions[3])
	            Assert.AreEqual((int)ManageStoreStatus.Success, _storeManager2.ViewStoreHistory().Status);
            else
	            Assert.AreEqual((int)ManageStoreStatus.InvalidManager, _storeManager2.ViewStoreHistory().Status);
        }
    }
}