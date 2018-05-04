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
        private int countMessagesToServer;
        private Publisher publisher;

        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            countMessagesToServer = 0;
            serverMocker.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>())).Callback(SendMessageToServer);
            publisher = Publisher.Instance;
            MarketDB.Instance.InsertByForce();
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
