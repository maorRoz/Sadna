using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;
using SadnaSrc.Main;

namespace MarketFeedTests.IntegrationTests
{
    public class BuyFromStoreFeedTests
    {
        [TestInitialize]
        public void IntegrationFeedTestsBuilder()
        {
            MarketDB.Instance.InsertByForce();
        }

        [TestMethod]
        public void NoObserversTest()
        {

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

    }
}
