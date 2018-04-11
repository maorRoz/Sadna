using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class UserSpot_OrderPool_IntegrationTest
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private UserBuyerHarmony userBuyerHarmony;
        private MarketYard marketSession;

        private IUserService userServiceSession2;

        private string user = "Vadim Chernov";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession2.EnterSystem();
            orderServiceSession = (OrderService) marketSession.GetOrderService(ref userServiceSession);
            userBuyerHarmony = new UserBuyerHarmony(ref userServiceSession);
        }


        [TestMethod]
        public void GuestDetailsNullTest()
        {
            try
            {
                Assert.IsNull(userBuyerHarmony.GetName());
                Assert.IsNull(userBuyerHarmony.GetAddress());
                Assert.IsNull(userBuyerHarmony.GetCreditCard());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RegisteredUserDetailsTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                Assert.AreEqual(user, userBuyerHarmony.GetName());
                Assert.AreEqual("Mivtza Kilshon", userBuyerHarmony.GetAddress());
                Assert.AreEqual("12345678", userBuyerHarmony.GetCreditCard());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        //TODO: Fix all the following tests after implementation of OrderPool is complete

        [TestMethod]
        public void CartItemUpdatedAfterBuyTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyItemFromImmediate("Bamba", "The Red Rock", 1, 6.00);
                userServiceSession2.SignIn(user, pass);
                CartItem item = ((UserService) userServiceSession2).MarketUser.Cart.SearchInCart("The Red Rock", "Bamba", 6.00);
                //Assert.AreEqual(2,item.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CartItemRemovedAfterBuyTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyItemFromImmediate("OCB", "24", 2, 10.00);
                userServiceSession2.SignIn(user, pass);
                //Assert.IsNull(((UserService)userServiceSession2).MarketUser.Cart.SearchInCart("24", "OCB", 10.00));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CartUnchangedHighQuantityTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyItemFromImmediate("Bamba", "The Red Rock", 999, 6.00);
                userServiceSession2.SignIn(user, pass);
                CartItem item = ((UserService)userServiceSession2).MarketUser.Cart.SearchInCart("The Red Rock", "Bamba", 6.00);
                //Assert.AreEqual(3,item.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CartUnchangedNegativeQuantityTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyItemFromImmediate("Bamba", "The Red Rock", -5, 6.00);
                userServiceSession2.SignIn(user, pass);
                CartItem item = ((UserService)userServiceSession2).MarketUser.Cart.SearchInCart("The Red Rock", "Bamba", 6.00);
                //Assert.AreEqual(3,item.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void StoreItemsRemovedFromCartTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyAllItemsFromStore("The Red Rock");
                userServiceSession2.SignIn(user, pass);
                //Assert.AreEqual(0, ((UserService)userServiceSession2).MarketUser.Cart.GetCartStorage("The Red Rock"));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CartUnchangedNonexistantStoreTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyAllItemsFromStore("The Blue Rock");
                userServiceSession2.SignIn(user, pass);
                //Assert.AreEqual(3, ((UserService)userServiceSession2).MarketUser.Cart.GetCartStorage());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CartIsEmptyAfterBuyTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.BuyEverythingFromCart();
                userServiceSession2.SignIn(user, pass);
                //Assert.AreEqual(0, ((UserService)userServiceSession2).MarketUser.Cart.GetCartStorage());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GuestWithoutDetailsErrorTest()
        {
            try
            {
                Assert.AreNotEqual(0, orderServiceSession.BuyEverythingFromCart().Status);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GiveAlternativeDetailsTest()
        {
            try
            {
                userServiceSession.SignIn(user, pass);
                orderServiceSession.GiveDetails("Moshe", "A", "12345678");
                Assert.AreEqual("Moshe", orderServiceSession.UserName);
                Assert.AreEqual("A",orderServiceSession.UserAddress);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            userServiceSession2.CleanSession();
            orderServiceSession.CleanSession();
            userBuyerHarmony.CleanSession();
            MarketYard.CleanSession();
        }
    }
}