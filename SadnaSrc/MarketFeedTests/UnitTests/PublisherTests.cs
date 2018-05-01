using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;

namespace MarketFeedTests.UnitTests
{
    [TestClass]
    public class PublisherTests
    {

        private Publisher publisher;
        private Mock<IFeedDL> feedDbMocker;
        private Mock<IFeedQueue> feedQueueMocker;
        private int userId1 = 6001;
        private int userId2 = 6002;
        private int userId3 = 6003;

        [TestInitialize]
        public void PublisherTestBuilder()
        {
            feedDbMocker = new Mock<IFeedDL>();
            feedDbMocker.Setup(x => x.GetUserIds()).Returns(new[] { userId1, userId2, userId3 });
            Publisher.FeedDl = feedDbMocker.Object;
            publisher = Publisher.Instance;
        }


        [TestMethod]
        public void QueueForEachUserTest()
        {
            var expectedEmptyQueue = new Notification[0];
            try
            {
                var queue1 = publisher.GetFeedQueue(userId1);
                Assert.Equals(expectedEmptyQueue, queue1.GetPendingFeeds());
                var queue2 = publisher.GetFeedQueue(userId2);
                Assert.Equals(expectedEmptyQueue, queue2.GetPendingFeeds());
                var queue3 = publisher.GetFeedQueue(userId3);
                Assert.Equals(expectedEmptyQueue, queue3.GetPendingFeeds());
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
    }
}
