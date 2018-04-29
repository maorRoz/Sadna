using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class OrderPoolApiTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IOrderDL> orderDbMocker;
        private Mock<IUserBuyer> userBuyerMocker;
        private Mock<IStoresSyncher> storeSyncherMock;

        private OrderPoolSlave slave;
        private OrderItem item;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            orderDbMocker = new Mock<IOrderDL>();
            userBuyerMocker = new Mock<IUserBuyer>();
            item = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
        }

        [TestMethod]
        public void BuySingleItemTest()
        {
            /*userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            storeSyncherMock.Setup(x => x.RemoveProducts(It.IsAny<OrderItem[]>()))

            //orderDbMocker.Setup(x => x.(It.IsAny<string>())).Returns(true);*/
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
