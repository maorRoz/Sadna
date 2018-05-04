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
        private string existingProduct2 = "Goldstar";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession2.EnterSystem();
            storeServiceSession =
                (StoreManagementService) marketSession.GetStoreManagementService(userServiceSession, store);
        }

        /*
         * Add Discount tests
         */

        [TestMethod]
        public void GuestAddDiscount()
        {
            try
            {
                SignInAndAddDiscount("guest", existingProduct);
                Assert.IsNull(StoreDL.Instance.GetProductFromStore(store, existingProduct).Discount);
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
                Assert.IsNull(StoreDL.Instance.GetProductFromStore(store, existingProduct).Discount);
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
                Assert.AreEqual("D7",
                    StoreDL.Instance.GetProductFromStore(store, existingProduct).Discount.discountCode);
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
                Assert.AreEqual("D8",
                    StoreDL.Instance.GetProductFromStore(store, existingProduct).Discount.discountCode);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * Edit Discount tests
         */

        [TestMethod]
        public void GuestEditDiscount()
        {
            try
            {
                SignInAndEditDiscount("guest", existingProduct2);
                Assert.AreEqual(50,
                    StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount.DiscountAmount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerEditDiscount()
        {
            try
            {
                SignInAndEditDiscount(manager, existingProduct2);
                Assert.AreEqual(50,
                    StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount.DiscountAmount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerEditDiscount2()
        {
            try
            {
                SignInAndEditDiscount(manager2, existingProduct2);
                Assert.AreEqual(2,
                    StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount.DiscountAmount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerEditDiscount()
        {
            try
            {
                SignInAndEditDiscount(owner, existingProduct2);
                Assert.AreEqual(2,
                    StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount.DiscountAmount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * Remove Discount tests
         */

        [TestMethod]
        public void GuestRemoveDiscount()
        {
            try
            {
                SignInAndRemoveDiscount("guest", existingProduct2);
                Assert.IsNotNull(StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerRemoveDiscount()
        {
            try
            {
                SignInAndRemoveDiscount(manager, existingProduct2);
                Assert.IsNotNull(StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerRemoveDiscount2()
        {
            try
            {
                SignInAndRemoveDiscount(manager2, existingProduct2);
                Assert.IsNull(StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerRemoveDiscount()
        {
            try
            {
                SignInAndRemoveDiscount(owner, existingProduct2);
                Assert.IsNull(StoreDL.Instance.GetProductFromStore(store, existingProduct2).Discount);
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
            MarketDB.Instance.CleanByForce();
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

        private void SignInAndEditDiscount(string user, string product)
        {
            SignIn(user);
            storeServiceSession.EditDiscount(product, "DiscountAmount", "2");
        }

        private void SignInAndRemoveDiscount(string user, string product)
        {
            SignIn(user);
            storeServiceSession.RemoveDiscountFromProduct(product);
        }

        private void SignInAndEditProduct(string user, string product)
        {
            SignIn(user);
            storeServiceSession.EditProduct(product, "Name", "Bambaa");
            StoreDL.Instance.GetProductFromStore(store, "Bambaa");
        }

        private void SignIn(string user)
        {
            if (user != "guest")
                userServiceSession.SignIn(user, pass);
        }
    }
}
