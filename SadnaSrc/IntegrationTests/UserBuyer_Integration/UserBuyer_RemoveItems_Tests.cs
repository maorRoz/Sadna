using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserBuyer_Integration
{
    [TestClass]
    public class UserBuyer_RemoveItems_Tests
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private UserBuyerHarmony userBuyerHarmony;

        private MarketYard marketSession;
        private string user = "Vadim Chernov";
        private string emptyUser = "Arik1";
        private string singleItemUser = "Vova";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            userBuyerHarmony = new UserBuyerHarmony(ref userServiceSession);
        }

        /*
         * EmptyCart (all) tests
         */

        [TestMethod]
        public void EmptyCartTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                userBuyerHarmony.EmptyCart();
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void EmptyCartAlreadyEmptyTest()
        {
            try
            {
                userServiceSession.SignIn(emptyUser, pass);
                userBuyerHarmony.EmptyCart();
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * EmptyCart (store) tests
         */

        [TestMethod]
        public void EmptyCartSingleStoreTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                userBuyerHarmony.EmptyCart("The Red Rock");
                Assert.AreEqual(1, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void EmptyCartNonExistantStoreTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                userBuyerHarmony.EmptyCart("The Blue Rock");
                Assert.AreEqual(3, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void EmptyCartAlreadyEmptySingleStoreTest()
        {
            try
            {
                userServiceSession.SignIn(emptyUser, pass);
                userBuyerHarmony.EmptyCart("24");
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void EmptyCartWithOneStoreTest()
        {
            try
            {
                userServiceSession.SignIn(singleItemUser, pass);
                userBuyerHarmony.EmptyCart("24");
                Assert.AreEqual(0, userServiceSession.ViewCart().ReportList.Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * RemoveItemFromCart tests
         */

        [TestMethod]
        public void RemoveItemTest()
        {
            try
            {
                removeItem(user, "Bamba", 3);
                Assert.IsNull(((UserService)userServiceSession).MarketUser.Cart.SearchInCart("The Red Rock", "Bamba", 6.00));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveItemPartiallyTest()
        {
            try
            {
                removeItem(user, "Bamba", 2);
                Assert.AreEqual(1,
                    ((UserService)userServiceSession).MarketUser.Cart.SearchInCart("The Red Rock", "Bamba", 6.00).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveItemLargeQuantityTest()
        {
            try
            {
                removeItem(user, "Bamba", 999);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void RemoveItemNegativeQuantityTest()
        {
            try
            {
                removeItem(user, "Bamba", -5);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void RemoveNonExistantItemTest()
        {
            try
            {
                removeItem(user, "Bisli", 2);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void RemoveItemEmptyCartTest()
        {
            try
            {
                removeItem(emptyUser, "Bamba", 2);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            orderServiceSession.CleanSession();
            userBuyerHarmony.CleanSession();
            MarketYard.CleanSession();
        }

        /*
         * Private helper functions
         */

        private void removeItem(string username, string item, int quantity)
        {
            userServiceSession.SignIn(username, pass);
            userBuyerHarmony.RemoveItemFromCart(item, "The Red Rock", quantity, 6.00);
        }
    }
}
