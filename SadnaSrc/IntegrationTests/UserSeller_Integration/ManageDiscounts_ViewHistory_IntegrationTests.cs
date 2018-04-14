using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class ManageDiscounts_ViewHistory_IntegrationTests
    {
        private IUserService userServiceSession;
        private IUserService userServiceSession2;
        private StoreManagementService storeServiceSession;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string owner = "Vova";
        private string manager = "Vadim Chernov";
        private string manager2 = "Big Smoke";
        private string shopper = "Arik2";
        private string sysadmin = "Arik1";
        private string pass = "123";
        private string storeAction1 = "ViewPurchaseHistory";
        private string storeAction2 = "ManageProducts";
        private string storeAction3 = "PromoteStoreAdmin";
        private string storeAction4 = "DeclareDiscountPolicy";
        private string storeOwner = "StoreOwner";
        private string existingProduct = "Bamba";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession2.EnterSystem();
            storeServiceSession =
                (StoreManagementService) marketSession.GetStoreManagementService(userServiceSession, store);
        }

        /*
         * Add Product tests
         */

        [TestMethod]
        public void GuestAddDiscount()
        {
            try
            {
                SignInAndAddDiscount("guest", existingProduct);
                Assert.IsNull(ModuleGlobalHandler.GetInstance().GetProductFromStore(store, existingProduct).Discount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerAddDiscount()
        {
            try
            {
                SignInAndAddDiscount(manager, existingProduct);
                Assert.IsNull(ModuleGlobalHandler.GetInstance().GetProductFromStore(store, existingProduct).Discount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerAddDiscount2()
        {
            try
            {
                SignInAndAddDiscount(manager2, existingProduct);
                Assert.AreEqual("D2",
                    ModuleGlobalHandler.GetInstance().GetProductFromStore(store, existingProduct).Discount.discountCode);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerAddDiscount()
        {
            try
            {
                SignInAndAddDiscount(owner, existingProduct);
                Assert.AreEqual("D3",
                    ModuleGlobalHandler.GetInstance().GetProductFromStore(store, existingProduct).Discount.discountCode);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * View History tests
         */

        [TestMethod]
        public void GuestViewHistory()
        {
            try
            {
                Assert.AreNotEqual(0, storeServiceSession.ViewStoreHistory());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShopperViewHistory()
        {
            try
            {
                SignIn(shopper);
                Assert.AreNotEqual(0, storeServiceSession.ViewStoreHistory().Status);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerViewHistory()
        {
            try
            {
                SignIn(manager);
                Assert.AreNotEqual(0, storeServiceSession.ViewStoreHistory().Status);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerViewHistory()
        {
            try
            {
                SignIn(owner);
                Assert.AreEqual(0, storeServiceSession.ViewStoreHistory().Status);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SysAdminViewHistory()
        {
            try
            {
                SignIn(sysadmin);
                Assert.AreEqual(0, storeServiceSession.ViewStoreHistory().Status);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            userServiceSession2.CleanSession();
            storeServiceSession.CleanSession();
            MarketYard.CleanSession();
        }

        private void SignInAndPromote(string promoter, string toPromote, string action)
        {
            SignIn(promoter);
            storeServiceSession.PromoteToStoreManager(toPromote, action);
            userServiceSession2.SignIn(toPromote, pass);
        }

        private void SignInAndAddDiscount(string user, string product)
        {
            SignIn(user);
            storeServiceSession.AddDiscountToProduct(product, new DateTime(2018, 5, 12), new DateTime(2018, 6, 20), 5,
                "VISIBLE", false);

        }

        private void SignInAndEditProduct(string user, string product)
        {
            SignIn(user);
            storeServiceSession.EditProduct(product, "Name", "Bambaa");
            ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, "Bambaa");
        }

        private void SignIn(string user)
        {
            if (user != "guest")
                userServiceSession.SignIn(user, pass);
        }
    }
}
