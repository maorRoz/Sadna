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
    public class BuyFromStoreFeedTests
    {
        
        private Mock<IListener> serverMocker;
        private int countMessagesToServer;
        private OrderService orderService;
        private int owner1 = 5;
        private int owner2 = 2;
        private int owner3 = 3;

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            var marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            serverMocker.Setup(x => x.GetMessage(owner1.ToString(), "#9 has been sold in Cluckin Bell!")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(owner1.ToString(), "The March Hare has been sold in T!")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(owner1.ToString(), "BOX has been sold in X!")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(owner2.ToString(), "BOX has been sold in X!")).Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(owner3.ToString(), "BOX has been sold in X!")).Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            orderService = (OrderService)marketSession.GetOrderService(ref userService);
            orderService.LoginBuyer("Ryder", "123");
        }

        [TestMethod]
        public void NewQueueOnRegisterationTest()
        {
            var newUserid = RegisterEvent();
            try
            {
                FeedSubscriber.SubscribeSocket(serverMocker.Object, newUserid, newUserid.ToString());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NoObserversTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            FeedSubscriber.UnSubscribeSocket(owner1.ToString());
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
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            var answer = orderService.BuyItemFromImmediate("#9", "Cluckin Bell", 1, 5.00, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(1, countMessagesToServer);

        }


        [TestMethod]
        public void OneStoreOwnerLotteryTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            var answer = orderService.BuyLotteryTicket("The March Hare", "T", 1, 50);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(1, countMessagesToServer);

        }

        [TestMethod]
        public void ManyStoreOwnersTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner2, owner2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner3, owner3.ToString());
            var answer = orderService.BuyItemFromImmediate("BOX", "X", 1, 100, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(3, countMessagesToServer);
        }

        [TestMethod]
        public void StoreOwnerOfBothStoresTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            var answer = orderService.BuyItemFromImmediate("BOX", "X", 1, 100, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(1, countMessagesToServer);
            answer = orderService.BuyItemFromImmediate("#9", "Cluckin Bell", 1, 5.00, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(2, countMessagesToServer);
        }

        [TestMethod]
        public void GetBuyMessagesOfflineTest()
        {
            var answer = orderService.BuyItemFromImmediate("#9", "Cluckin Bell", 1, 5.00, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(0, countMessagesToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            Assert.AreEqual(1, countMessagesToServer);
        }

        [TestMethod]
        public void StoreOwnerLosePromotionTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner2, owner2.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner3, owner3.ToString());
            var answer = orderService.BuyItemFromImmediate("BOX", "X", 1, 100, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(2, countMessagesToServer);
            DemoteOwner2Action();
            answer = orderService.BuyItemFromImmediate("BOX", "X", 1, 100, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(3, countMessagesToServer);

        }

        [TestMethod]
        public void NoOwnersStoreClosedTest()
        {
            FeedSubscriber.SubscribeSocket(serverMocker.Object, owner1, owner1.ToString());
            RemoveOwner1FromSystem();
            var answer = orderService.BuyItemFromImmediate("#9", "Cluckin Bell", 1, 5.00, null);
            Assert.AreEqual(0, answer.Status);
            Assert.AreEqual(0, countMessagesToServer);
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

        private int RegisterEvent()
        {
            var userService = MarketYard.Instance.GetUserService();
            var answer = userService.EnterSystem();
            var newGuestId = Convert.ToInt32(answer.ReportList[0]);
            answer = userService.SignUp("meow", "mmm", "123", "12345678");
            Assert.AreEqual(0, answer.Status);
            return newGuestId;
        }

        private void DemoteOwner2Action()
        {
            var userService = MarketYard.Instance.GetUserService();
            userService.EnterSystem();
            userService.SignIn("Arik3", "123");
            var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, "X");
            storeManagementService.PromoteToStoreManager("Arik2", "");
        }

        private void RemoveOwner1FromSystem()
        {
            var userService = MarketYard.Instance.GetUserService();
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            var systemAdminService = MarketYard.Instance.GetSystemAdminService(userService);
            systemAdminService.RemoveUser("CJ");
        }

    }
}
