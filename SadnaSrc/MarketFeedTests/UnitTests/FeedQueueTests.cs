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

        [TestInitialize]
        public void FeedQueueTestBuilder()
        {

        }
    }
}
