using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace MarketFeedTests.IntegrationTests
{
    [TestClass]
    public class BuyFromStoreFeedTests
    {
        
        private Mock<IListener> serverMocker;
        private int countMessagesToServer;
        private Publisher publisher;
        private OrderService orderService;

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            serverMocker.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>())).Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
            publisher = Publisher.Instance;
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            orderService = (OrderService)marketSession.GetOrderService(ref userService);
            orderService.LoginBuyer("Ryder", "123");
        }

        [TestMethod]
        public void NoObserversTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object,6,"6");
            FeedSubscriber.UnSubscribeSocket("6");
            var answer = orderService.BuyItemFromImmediate("#9","Cluckin Bell",1,5.00,null);
            Assert.AreEqual(0,answer.Status);
            Assert.AreEqual(0,countMessagesToServer);
            answer = orderService.BuyEverythingFromCart(null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(0, countMessagesToServer);
        }

        [TestMethod]
        public void OneStoreOwnerTest()
        {

        }

        [TestMethod]
        public void ManyStoreOwnersTest()
        {

        }

        [TestMethod]
        public void StoreOwnerOfBothStoresTest()
        {

        }

        [TestMethod]
        public void GetMessagesOfflineTest()
        {

        }

        [TestMethod]
        public void StoreOwnerLosePromotionTest()
        {

        }

        [TestMethod]
        public void StoreOwnerDeletedTest()
        {

        }

        [TestMethod]
        public void NoOwnersStoreClosedTest()
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
