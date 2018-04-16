using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace BlackBoxStoreTests
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
		private IUserBridge _ownerBridge;
		
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
		    _ownerBridge = null;
		}

		[TestMethod]
		public void StoreOwnerSucceededPromotePromoteStoreAdmin()
		{

			_storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
		    TryPromote("Thor", storeAction1);
            AssertActions(new bool[]{true, false, false, false});
        }

        [TestMethod]
	    public void StoreOwnerSucceededPromoteManageProducts()
	    {

	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
	        TryPromote("Thor", storeAction2);
	        AssertActions(new bool[] { false, true, false, false });
        }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteDeclareDiscountPolicy()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
            TryPromote("Thor", storeAction3);
	        AssertActions(new bool[] { false, false, true, false });
        }

	    [TestMethod]
	    public void StoreOwnerSucceededPromoteViewPurchaseHistory()
	    {
	        _storeManager1.GetStoreManagementService(_bridgeSignUp.GetUserSession(), "Volcano");
            TryPromote("Thor", storeAction4);
	        AssertActions(new bool[] { false, false, false, true });
        }

        /*[TestMethod]
		public void AdminSystemSucceededPromote()
		{
		    _ownerBridge = UserDriver.getBridge();
		    _ownerBridge.EnterSystem();
		    _ownerBridge.SignIn(adminName, adminPass);
			_storeManager1.GetStoreManagementService(_ownerBridge.GetUserSession(),"basush");
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

		}*/

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
		    _ownerBridge?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}

	    private void TryPromote(string toPromote, string actions)
	    {

	        MarketAnswer res = _storeManager1.PromoteToStoreManager(toPromote, actions);
	        Assert.AreEqual((int)PromoteStoreStatus.Success, res.Status);
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
	            Assert.AreEqual((int)StoreEnum.NoPremmision, _storeManager2.AddNewProduct(product, 50, "tool", 5).Status);
	            Assert.AreEqual((int)StoreEnum.NoPremmision, _storeManager2.EditProduct(product, "Price", "3").Status);
	            Assert.AreEqual((int)StoreEnum.NoPremmision, _storeManager2.RemoveProduct(product).Status);
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
	            Assert.AreEqual((int)StoreEnum.NoPremmision,
	                _storeManager2.AddDiscountToProduct("Product", DateTime.Today, DateTime.Today.AddDays(3), 50,
	                    "VISIBLE", true).Status);
	            Assert.AreEqual((int)StoreEnum.NoPremmision, _storeManager2.EditDiscount(product, "DiscountAmount", "20").Status);
	            Assert.AreEqual((int)StoreEnum.NoPremmision, _storeManager2.RemoveDiscountFromProduct(product).Status);
            }

	        if (permissions[3])
	            Assert.AreEqual((int)ManageStoreStatus.InvalidStore, _storeManager2.ViewStoreHistory().Status);
            else
	            Assert.AreEqual((int)ManageStoreStatus.InvalidManager, _storeManager2.ViewStoreHistory().Status);
        }
    }
}