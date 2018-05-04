using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;
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
        private OrderService orderService3;

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            serverMocker.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>())).Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService1 = marketSession.GetUserService();
            orderService1 = (OrderService)marketSession.GetOrderService(ref userService1);
            orderService1.LoginBuyer("Ryder", "123");
            var userService2 = marketSession.GetUserService();
            orderService2 = (OrderService)marketSession.GetOrderService(ref userService2);
            orderService2.LoginBuyer("Ryder", "123");
            var userService3 = marketSession.GetUserService();
            orderService3 = (OrderService)marketSession.GetOrderService(ref userService3);
            orderService3.LoginBuyer("Ryder", "123");
        }

        [TestMethod]
        public void NoObserversAtAllOnLotteryCancelTest()
        {

        }

        [TestMethod]
        public void NoObserversForSomeParticipantsOnLotteryCancelTest()
        {

        }

        [TestMethod]
        public void SignUpThenGetRefundOnTicketTest()
        {

        }

        [TestMethod]
        public void GetLotteryCancelMessagesOfflineTest()
        {

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
    }
}
