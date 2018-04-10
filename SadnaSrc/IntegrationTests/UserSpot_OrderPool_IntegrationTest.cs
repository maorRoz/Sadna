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

        private string user = "Vadim Chernov";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession.EnterSystem();
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

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            orderServiceSession.CleanSession();
            userBuyerHarmony.CleanSession();
            MarketYard.CleanSession();
        }
    }
}