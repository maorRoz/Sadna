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

        [TestInitialize]
        public void PublisherTestBuilder()
        {
            publisher = Publisher.Instance;
            
        }


        [TestMethod]
        public void QueueForEachUserTest()
        {
        }
    }
}
