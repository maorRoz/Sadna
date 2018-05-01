using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;

namespace MarketFeedTests.UnitTests
{
    [TestClass]
    public class FeedObserverTests
    {
        private FeedObserver observer;
        private Mock<IPublisher> publisherMocker;
        private Mock<IListener> serverMocker;
        private Mock<IFeedQueue> queueMocker;
        private int userId = 6001;
        private int countMessageSent;
        private bool isAttached;
        [TestInitialize]
        public void FeedObserverTestBuilder()
        {
            countMessageSent = 0;
            isAttached = false;
            publisherMocker = new Mock<IPublisher>();
            serverMocker = new Mock<IListener>();
            queueMocker = new Mock<IFeedQueue>();
            var notification1 = new Notification(userId,"message 1");
            var notification2 = new Notification(userId,"Message 2");
            queueMocker.Setup(x => x.GetPendingFeeds()).Returns(new[]{ notification1,notification2});
            queueMocker.Setup(x => x.Attach(It.IsAny<FeedObserver>())).Callback(SetAttached);
            queueMocker.Setup(x => x.Detach(It.IsAny<FeedObserver>())).Callback(SetDetached);
            publisherMocker.Setup(x => x.GetFeedQueue(userId)).Returns(queueMocker.Object);
            serverMocker.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>())).Callback(RaiseMockCount);
            observer = new FeedObserver(publisherMocker.Object,serverMocker.Object,userId,"");

        }


        [TestMethod]
        public void AttachedTest()
        {
            Assert.IsTrue(isAttached);
        }

        [TestMethod]
        public void DetachedTest()
        {
            observer.DetachFromQueue();
            Assert.IsFalse(isAttached);
        }

        [TestMethod]
        public void SendAfterUpdateTest()
        {
            observer.Update();
            Assert.AreEqual(2,countMessageSent);
        }

        private void SetAttached()
        {
            isAttached = true;
        }

        private void SetDetached()
        {
            isAttached = false;
        }

        private void RaiseMockCount()
        {
            countMessageSent++;
        }
    }
}
