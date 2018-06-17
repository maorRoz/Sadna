using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;
using SadnaSrc.OrderPool;

namespace MarketFeedTests.IntegrationTests
{
    [TestClass]
    public class LotteryFinishFeedTests
    {
        private Mock<IListener> serverMocker;
        private int countMessagesToServer;
        private OrderService orderService1;
        private OrderService orderService2;
        private OrderService orderService3;
        private int buyerId1 = 6;
        private int buyerId2 = 7;
        private int buyerId3 = 8;
        private string storeLottery = "Cluckin Bell";
        private string productLottery = "#45 With Cheese";

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            var marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            serverMocker.Setup(x => x.GetMessage(buyerId2.ToString(), "You have won the lottery on " + productLottery + " in " 
                                                                      + storeLottery + "!")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(buyerId1.ToString(), "You have lost the lottery on " + productLottery + " in "
                                                                      + storeLottery + "...")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(buyerId3.ToString(), "You have lost the lottery on " + productLottery + " in "
                                                                      + storeLottery + "...")).Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService1 = marketSession.GetUserService();
            orderService1 = (OrderService)marketSession.GetOrderService(ref userService1);
            orderService1.LoginBuyer("Ryder", "123");
            var userService2 = marketSession.GetUserService();
            orderService2 = (OrderService)marketSession.GetOrderService(ref userService2);
            orderService2.LoginBuyer("Vadim Chernov", "123");
            var userService3 = marketSession.GetUserService();
            orderService3 = (OrderService)marketSession.GetOrderService(ref userService3);
            orderService3.LoginBuyer("Vova", "123");
            MarketYard.SetDateTime(new DateTime(2018,4,14));

        }

        [TestMethod]
        public void NoObserversAtAllOnCloseLotteryTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId3, buyerId3.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId1.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId2.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId3.ToString());
            BuyoutLottery();
            Assert.AreEqual(0, countMessagesToServer);
        }

        [TestMethod]
        public void NoObserversForSomeLosersOnCloseLotteryTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId3, buyerId3.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId1.ToString());
            BuyoutLottery();
            Assert.AreEqual(2, countMessagesToServer);
        }

        [TestMethod]
        public void NoObserversForWinnerOnCloseLotteryTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId3, buyerId3.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId1.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId2.ToString());
            BuyoutLottery();
            Assert.AreEqual(1, countMessagesToServer);

        }

        [TestMethod]
        public void GetLotteryFinishMessagesOfflineTest()
        {
            BuyoutLottery();
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            Assert.AreEqual(1, countMessagesToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId3, buyerId3.ToString());
            Assert.AreEqual(3, countMessagesToServer);
        }

        [TestMethod]
        public void SignUpThenLoseLotteryTest()
        {
            var newUserid = RegisterEvent();
            serverMocker.Setup(x => x.GetMessage(newUserid.ToString(), "You have lost the lottery on " + productLottery + " in "
                                                                       + storeLottery + "...")).Callback(SendMessageToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId3, buyerId3.ToString());
            try
            {
                BuyoutLottery();
                Assert.AreEqual(2, countMessagesToServer);
                FeedSubscriber.SubscribeSocket(serverMocker.Object, newUserid, newUserid.ToString());
                Assert.AreEqual(3, countMessagesToServer);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }


        [TestCleanup]
        public void IntegrationFeedTestsCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }


        private void SendMessageToServer()
        {
            countMessagesToServer++;
        }

        private void BuyoutLottery()
        {
            var answer = orderService1.BuyLotteryTicket(productLottery, storeLottery, 1, 6);
            Assert.AreEqual(0,answer.Status);
            answer = orderService2.BuyLotteryTicket(productLottery, storeLottery, 1, 6);
            Assert.AreEqual(0, answer.Status);
            orderService3.Cheat(7);
            answer = orderService3.BuyLotteryTicket(productLottery, storeLottery, 1, 6);
            Assert.AreEqual(0, answer.Status);
        }

        private int RegisterEvent()
        {
            var userService = MarketYard.Instance.GetUserService();
            var answer = userService.EnterSystem();
            var newGuestId = Convert.ToInt32(answer.ReportList[0]);
            answer = userService.SignUp("meow", "mmm", "123", "12345678");
            Assert.AreEqual(0, answer.Status);
            orderService1 = (OrderService)MarketYard.Instance.GetOrderService(ref userService);
            return newGuestId;
        }
    }
}
