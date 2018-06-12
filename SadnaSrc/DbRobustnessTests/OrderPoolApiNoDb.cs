using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace DbRobustnessTests
{
    [TestClass]
    public class OrderPoolApiNoDb
    {
        private IOrderService orderService;
        private MarketAnswer answer;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            answer = userService.SignIn("Big Smoke", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            orderService = marketSession.GetOrderService(ref userService);
            MarketDB.ToDisable = true;
        }
        [TestMethod]
        public void BuyItemFromImmediateNoDBTest()
        {
            answer = orderService.BuyItemFromImmediate("#9", "Cluckin Bell", 1, 5.0, null);
            Assert.AreEqual((int)OrderStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void BuyLotteryTicketNoDBTest()
        {
            answer = orderService.BuyLotteryTicket("The March Hare", "T", 1,6.0);
            Assert.AreEqual((int)OrderStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void BuyEverythingFromCartNoDBTest()
        {
            answer = orderService.BuyEverythingFromCart(null);
            Assert.AreEqual((int)OrderStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void GiveDetailsNoDBTest()
        {
            answer = orderService.GiveDetails("Moshe", "123", "12345678");
            Assert.AreEqual((int)GiveDetailsStatus.Success, answer.Status);
        }

        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
