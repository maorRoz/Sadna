using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserShopper_Integration
{
    [TestClass]
    public class User_Store_Shopping_IntegrationTests
    {
        private IUserService userServiceSession;
        private StoreShoppingService storeServiceSession;
        private UserShopperHarmony userShopperHarmony;
        private MarketYard marketSession;

        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";
        private string user = "Vova";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userShopperHarmony = new UserShopperHarmony(ref userServiceSession);
            storeServiceSession = (StoreShoppingService)marketSession.GetStoreShoppingService(ref userServiceSession);
        }

        /*
         * User validation tests
         */

        [TestMethod]
        public void NotEnteredBrowseMarketTest()
        {
            try
            {
                userShopperHarmony.ValidateCanBrowseMarket();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CanBrowseMarketAsGuestTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                userShopperHarmony.ValidateCanBrowseMarket();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NotRegisteredUserTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                userShopperHarmony.ValidateRegistered();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ValidateRegisteredTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                userShopperHarmony.ValidateRegistered();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NonExistantUserLoginTest()
        {
            try
            {
                storeServiceSession.LoginShoper("A" + user, pass);
                userShopperHarmony.ValidateRegistered();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestIDTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                Assert.AreEqual(((UserService)userServiceSession).MarketUser.SystemID, userShopperHarmony.GetShopperID());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RegUserTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                Assert.AreEqual(((UserService)userServiceSession).MarketUser.SystemID, userShopperHarmony.GetShopperID());
                Assert.AreEqual(((RegisteredUser)((UserService)userServiceSession).MarketUser).Name, 
                    userShopperHarmony.GetShopperName());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * AddToCart tests
         */

        [TestMethod]
        public void AddItemToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store1, product1, 5);
                CartItem expected = ((UserService) userServiceSession).MarketUser.Cart.SearchInCart(store1, product1, 6);
                Assert.AreEqual(5, expected.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddExistingItemToCartTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                storeServiceSession.AddProductToCart(store2, product2, 2);
                CartItem expected = ((UserService)userServiceSession).MarketUser.Cart.SearchInCart(store2, product2, 10);
                Assert.AreEqual(10, expected.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddNonExistingItemToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store2, "A" + product2, 2);
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddItemFromNonExistingStoreToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart("A" + store2, product2, 2);
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddTooMuchItemToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store2, product2, 5000);
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddNegativeItemToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store2, product2, -4);
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddZeroItemToCartTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store2, product2, -4);
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * OpenStore tests
         */

        [TestMethod]
        public void OpenStoreAsGuestTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.OpenStore("Newstore1", "Nowhere");
                Assert.IsFalse(((UserService)userServiceSession).MarketUser.HasStorePolicies());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OpenStoreTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                storeServiceSession.OpenStore("Super Mordoch", "David Elazar");
                Assert.IsTrue(((UserService) userServiceSession).MarketUser.HasStorePolicies());
                StoreManagerPolicy[] policies = ((UserService)userServiceSession).MarketUser.GetStoreManagerPolicies("Super Mordoch");
                bool flag = false;
                foreach (StoreManagerPolicy policy in policies)
                {
                    if (policy.Action == StoreManagerPolicy.StoreAction.StoreOwner)
                    {
                        flag = true;
                    }
                }
                Assert.IsTrue(flag);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OpenStoreAlreadyOpenTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                storeServiceSession.OpenStore("24", "Mezada");
                Assert.AreEqual(0, ((UserService) userServiceSession).MarketUser.GetStoreManagerPolicies("24").Length);
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
            storeServiceSession.CleanSeesion();
            MarketYard.CleanSession();
        }

    }
}
