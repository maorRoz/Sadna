using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.OrderPool;

namespace MarketFeedTests.IntegrationTests
{
    [TestClass]
    public class LotteryCancelFeedTests
    {
        private Mock<IListener> serverMocker;
        private int countMessagesToServer;
        private OrderService orderService1;
        private OrderService orderService2;
        private int buyerId1 = 6;
        private int buyerId2 = 7;
        private string storeLottery = "Cluckin Bell";
        private string productLottery = "#45 With Cheese";

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            var marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            serverMocker.Setup(x => x.GetMessage(buyerId1.ToString(), "You've been fully refunded on a lottery you " +
                                                                      "were participating on")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(buyerId2.ToString(), "You've been fully refunded on a lottery you " +
                                                                      "were participating on")).Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService1 = marketSession.GetUserService();
            orderService1 = (OrderService)marketSession.GetOrderService(ref userService1);
            orderService1.LoginBuyer("Ryder", "123");
            var userService2 = marketSession.GetUserService();
            orderService2 = (OrderService)marketSession.GetOrderService(ref userService2);
            orderService2.LoginBuyer("Vadim Chernov", "123");
            MarketYard.SetDateTime(Convert.ToDateTime("14/04/2018"));
        }

        [TestMethod]
        public void NoObserversAtAllOnLotteryCancelTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId1.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId2.ToString());
            ToCancelLottery();
            Assert.AreEqual(0, countMessagesToServer);
        }

        [TestMethod]
        public void NoObserversForSomeParticipantsOnLotteryCancelTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            FeedSubscriber.UnSubscribeSocket(buyerId1.ToString());
            ToCancelLottery();
            Assert.AreEqual(1, countMessagesToServer);
        }


        [TestMethod]
        public void GetLotteryCancelMessagesOfflineTest()
        {
            ToCancelLottery();
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId1, buyerId1.ToString());
            Assert.AreEqual(1, countMessagesToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            Assert.AreEqual(2, countMessagesToServer);
        }

        [TestMethod]
        public void SignUpThenGetRefundOnTicketTest()
        {
            var newUserid = RegisterEvent();
            serverMocker.Setup(x => x.GetMessage(newUserid.ToString(), "You've been fully refunded on a lottery you " +
                                                                       "were participating on")).Callback(SendMessageToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, buyerId2, buyerId2.ToString());
            try
            {
                ToCancelLottery();
                Assert.AreEqual(1, countMessagesToServer);
                FeedSubscriber.SubscribeSocket(serverMocker.Object, newUserid, newUserid.ToString());
                Assert.AreEqual(2, countMessagesToServer);
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

        private void ToCancelLottery()
        {
            var answer = orderService1.BuyLotteryTicket(productLottery, storeLottery, 1, 6);
            Assert.AreEqual(0, answer.Status);
            answer = orderService2.BuyLotteryTicket(productLottery, storeLottery, 1, 6);
            Assert.AreEqual(0, answer.Status);
            MarketYard.SetDateTime(Convert.ToDateTime("01/01/2019"));
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
