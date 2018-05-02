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
    public class UserBuyer_Checkout_Test
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
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            userBuyerHarmony = new UserBuyerHarmony(ref userServiceSession);
        }

        /*
         * CheckoutAll tests
         */

        [TestMethod]
        public void CheckoutAllTest()
        {
            try
            {
                string result = getItemsFromCart(user, pass);
                string expected = "20 OCB, 24. 18 Bamba, The Red Rock. 33 Goldstar, The Red Rock. ";
                Assert.AreEqual(result,expected);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CheckoutAllSingleItemTest()
        {
            try
            {
                string result = getItemsFromCart(singleItemUser, pass);
                string expected = "80 Coated Peanuts, 24. ";
                Assert.AreEqual(result, expected);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CheckoutAllEmptyCartTest()
        {
            try
            {
                string result = getItemsFromCart(emptyUser, pass);
                Assert.AreEqual("", result);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * CheckoutFromStore tests
         */

        [TestMethod]
        public void CheckoutSingleStoreTest()
        {
            try
            {
                string result = getSingleStoreItems("24");
                string expected = "20 OCB, 24. ";
                Assert.AreEqual(result, expected);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CheckoutNonExistantStoreTest()
        {
            try
            {
                string result = getSingleStoreItems("The Blue Rock");
                Assert.AreEqual("", result);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CheckoutStoreNotInOrderTest()
        {
            try
            {
                string result = getSingleStoreItems("Cluckin' Bell");
                Assert.AreEqual("", result);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * CheckoutItem tests
         */

        [TestMethod]
        public void CheckoutSingleItemTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bamba", "The Red Rock", 3, 6.00);
                string result = getOrderItemString(item);
                string expected = "18 Bamba, The Red Rock";
                Assert.AreEqual(result, expected);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CheckoutItemNotInOrderTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Coated Peanuts", "24", 1, 10.00);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CheckoutItemNameErrorTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bomba", "The Red Rock", 3, 6.00);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CheckoutItemStoreErrorTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bamba", "Teh Red Rok", 3, 6.00);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CheckoutItemPriceErrorTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bamba", "The Red Rock", 3, 6.90);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CheckoutItemLargeQuantityTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bamba", "The Red Rock", 999, 6.00);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void CheckoutItemNegativeQuantityTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                OrderItem item = userBuyerHarmony.CheckoutItem("Bamba", "The Red Rock", -1, 6.00);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        /*
         * Private helper functions
         */

        private string getItemsFromCart(string username, string password)
        {
            userServiceSession.SignIn(username, password);
            OrderItem[] items = userBuyerHarmony.CheckoutAll();
            return getOrderString(items);
        }

        private string getSingleStoreItems(string store)
        {
            userServiceSession.SignIn(user, pass);
            OrderItem[] items = userBuyerHarmony.CheckoutFromStore(store);
            return getOrderString(items);
        }

        private string getOrderString(OrderItem[] items)
        {
            string result = "";
            for (int i = 0; i < items.Length; i++)
            {

                result += getOrderItemString(items[i]) + ". ";
            }

            return result;
        }

        private string getOrderItemString(OrderItem item)
        {
            return "" + item.Price + " " + item.Name + ", " + item.Store;
        }
    }
}
