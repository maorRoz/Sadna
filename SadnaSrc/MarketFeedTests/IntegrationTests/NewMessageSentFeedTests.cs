using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;

namespace MarketFeedTests.IntegrationTests
{
    [TestClass]
    public class NewMessageSentFeedTests
    {
        private Mock<IListener> serverMocker;
        private Mock<IMarketMessenger> messengerMocker;
        private int countMessagesToServer;
        private IMarketMessenger messenger;

        private int receiverId1 = 6;
        private int receiverId2 = 7;

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            countMessagesToServer = 0;
            serverMocker = new Mock<IListener>();
            messengerMocker = new Mock<IMarketMessenger>();
            messengerMocker.Setup(x => x.SendMessage(receiverId1, It.IsAny<string>())).Callback(SendMessageToReceiver1);
            messengerMocker.Setup(x => x.SendMessage(receiverId2, It.IsAny<string>())).Callback(SendMessageToReceiver2);
            serverMocker.Setup(x => x.GetMessage(receiverId1.ToString(), "You've got new message pending in your mailbox!"))
                .Callback(SendMessageToServer);
            serverMocker.Setup(x => x.GetMessage(receiverId2.ToString(), "You've got new message pending in your mailbox!"))
                .Callback(SendMessageToServer);
            MarketDB.Instance.InsertByForce();
        }

        [TestMethod]
        public void NoObserversAtAllTest()
        {
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId1, receiverId1.ToString());
            FeedSubscriber.UnSubscribeSocket(receiverId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            FeedSubscriber.UnSubscribeSocket(receiverId2.ToString());
            messenger.SendMessage(receiverId1,"The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(0, countMessagesToServer);
        }

        [TestMethod]
        public void NoObserverForOneTest()
        {
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId1, receiverId1.ToString());
            FeedSubscriber.UnSubscribeSocket(receiverId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            messenger.SendMessage(receiverId1, "The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(1, countMessagesToServer);
        }

        [TestMethod]
        public void OneMessageForBothTest()
        {
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId1, receiverId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            messenger.SendMessage(receiverId1, "The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(2, countMessagesToServer);
        }

        [TestMethod]
        public void SendManyMessagesTest()
        {
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId1, receiverId1.ToString());
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            messenger.SendMessage(receiverId1, "The Cake is A LIE!");
            messenger.SendMessage(receiverId1, "For The Horde!");
            messenger.SendMessage(receiverId2, "The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(4, countMessagesToServer);
        }

        [TestMethod]
        public void GetNotificiationOfflineForMessageSentTest()
        {
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId1, receiverId1.ToString());
            messenger.SendMessage(receiverId1, "The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(1, countMessagesToServer);
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            Assert.AreEqual(2, countMessagesToServer);
        }

        [TestMethod]
        public void SignUpThenGetMessageNotificationForMessageSentTest()
        {
            var newUserid = RegisterEvent();
            serverMocker.Setup(x => x.GetMessage(newUserid.ToString(), "You've got new message pending in your mailbox!"))
                .Callback(SendMessageToServer);
            messengerMocker.Setup(x => x.SendMessage(newUserid, It.IsAny<string>())).Callback(() => {Publisher.Instance.NotifyMessageReceived(newUserid);});
            messenger = messengerMocker.Object;
            FeedSubscriber.SubscribeSocket(serverMocker.Object, receiverId2, receiverId2.ToString());
            messenger.SendMessage(newUserid, "The Cake is A LIE!");
            messenger.SendMessage(receiverId2, "For The Horde!");
            Assert.AreEqual(1, countMessagesToServer);
            try
            {
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

        private void SendMessageToReceiver1()
        {
            Publisher.Instance.NotifyMessageReceived(receiverId1);
        }
        private void SendMessageToReceiver2()
        {
            Publisher.Instance.NotifyMessageReceived(receiverId2);
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
    }
}
