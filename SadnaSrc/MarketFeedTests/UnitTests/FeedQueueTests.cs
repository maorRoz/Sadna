using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;

namespace MarketFeedTests.UnitTests
{
    [TestClass]
    public class FeedQueueTests
    {

        private Mock<IFeedDL> feedDbMocker;
        private Mock<IObserver> observerMocker;
        private int userId = 6001;
        private FeedQueue queue;
        private int countUpdate;
        private Notification notification1;
        private Notification notification2;

        [TestInitialize]
        public void FeedQueueTestBuilder()
        {
            countUpdate = 0;
            feedDbMocker = new Mock<IFeedDL>();
            notification1 = new Notification(userId,"message 1");
            notification2 = new Notification(userId, "message 2");
            feedDbMocker.Setup(x => x.GetUnreadNotifications(userId)).Returns(new[] {notification1, notification2});
            observerMocker = new Mock<IObserver>();
            observerMocker.Setup(x => x.Update()).Callback(RaiseMockCount);
            queue = new FeedQueue(feedDbMocker.Object,userId);
        }

        [TestMethod]
        public void AttachTest()
        {
            queue.Attach(observerMocker.Object);
            Assert.AreEqual(1,countUpdate);
        }

        [TestMethod]
        public void DetachTest()
        {
            queue.Attach(observerMocker.Object);
            queue.Detach(observerMocker.Object);
            queue.AddFeed(new Notification(userId,"message 3"));
            Assert.AreEqual(1,countUpdate);
        }

        [TestMethod]
        public void NotifyNoObserversTest()
        {
            queue.Notify();
            Assert.AreEqual(0,countUpdate);
        }

        [TestMethod]
        public void NotifyHasObserverTest()
        {
            queue.Attach(observerMocker.Object);
            queue.AddFeed(new Notification(userId, "message 3"));
            queue.Notify();
            Assert.AreEqual(2,countUpdate);
        }

        [TestMethod]
        public void AddFeedTest()
        {
            queue.Attach(observerMocker.Object);
            queue.Notify();
            Assert.AreEqual(1,countUpdate);
            queue.AddFeed(new Notification(userId, "message 3"));
            queue.Notify();
            Assert.AreEqual(2,countUpdate);
        }

        [TestMethod]
        public void MarkQueueAsReadTest()
        {
            queue.Attach(observerMocker.Object);
            var actualQueue = queue.GetPendingFeeds();
            Assert.AreEqual(2,actualQueue.Length);
            notification1.Status = "Read";
            notification2.Status = "Read";
            Assert.AreEqual(notification1,actualQueue[0]);
            Assert.AreEqual(notification2, actualQueue[1]);
        }

        [TestMethod]
        public void RefreshQueueTest()
        {
            queue.Attach(observerMocker.Object);
            Assert.AreEqual(2, queue.GetPendingFeeds().Length);
            queue.Notify();
            Assert.AreEqual(0, queue.GetPendingFeeds().Length);
        }

        private void RaiseMockCount()
        {
            countUpdate++;
        }
    }
}
