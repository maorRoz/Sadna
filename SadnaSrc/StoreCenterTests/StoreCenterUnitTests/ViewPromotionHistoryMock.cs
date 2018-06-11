using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class ViewPromotionHistoryMock
    {
        private Mock<IStoreDL> storeDbMocker;
        private Mock<IUserSeller> userServiceMocker;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private ViewPromotionHistorySlave slave;
        private string store = "HistoryStore";
        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            storeDbMocker = new Mock<IStoreDL>();
            userServiceMocker = new Mock<IUserSeller>();
            storeDbMocker.Setup(x => x.GetStorebyName(store)).Returns(new Store(store, ""));
        }
        [TestMethod]
        public void ViewPromotionHistorySuccessTest()
        {
            storeDbMocker.Setup(x => x.IsStoreExistAndActive(store)).Returns(true);

            var expected = new[]
            {
                "some history",
                "some more history"
            };
            storeDbMocker.Setup(x => x.GetPromotionHistory(store)).Returns(expected);

            slave = new ViewPromotionHistorySlave(store, userServiceMocker.Object, storeDbMocker.Object);
            slave.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.Success,slave.Answer.Status);
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expected.Length,actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i],actual[i]);
            }
        }

        [TestMethod]
        public void StoreNotValidTest()
        {
            storeDbMocker.Setup(x => x.IsStoreExistAndActive(store)).Returns(false);
            slave = new ViewPromotionHistorySlave(store, userServiceMocker.Object, storeDbMocker.Object);
            slave.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
        }

        [TestMethod]

        public void NoPermissionToViewPromotionHistoryTest()
        {
            storeDbMocker.Setup(x => x.IsStoreExistAndActive(store)).Returns(true);
            userServiceMocker.Setup(x => x.CanPromoteStoreOwner()).Throws(new MarketException(0, ""));
            slave = new ViewPromotionHistorySlave(store, userServiceMocker.Object, storeDbMocker.Object);
            slave.ViewPromotionHistory();
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
