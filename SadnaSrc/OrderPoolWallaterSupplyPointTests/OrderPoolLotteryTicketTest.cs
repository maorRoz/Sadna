using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class OrderPoolLotteryTicketTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IOrderDL> orderDbMocker;
        private Mock<IUserBuyer> userBuyerMocker;
        private Mock<IStoresSyncher> storeSyncherMock;

        private LotteryTicketSlave slave;
        private OrderItem item1;
        private OrderItem item2;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            orderDbMocker = new Mock<IOrderDL>();
            userBuyerMocker = new Mock<IUserBuyer>();
            storeSyncherMock = new Mock<IStoresSyncher>();
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            SupplyService.Instance.FixExternal();
            PaymentService.Instance.FixExternal();
        }

        [TestMethod]
        public void BuyTicketTest()
        {
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new LotteryTicketSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyLotteryTicket("#9 Large", "Cluckin Bell", 1, 5.00, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            OrderItem actual = order.GetOrderItem("#9 Large", "Cluckin Bell");
            Assert.AreEqual(item2.Name, actual.Name);
            Assert.AreEqual(5.00, actual.Price);
            Assert.AreEqual(item2.Quantity, actual.Quantity);
            Assert.AreEqual(item2.Store, actual.Store);
        }

        [TestMethod]
        public void NoLotteryTicketFailTest()
        {
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            storeSyncherMock.Setup(x => x.ValidateTicket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Throws(new OrderException(OrderStatus.InvalidCoupon, "some message"));
            slave = new LotteryTicketSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyLotteryTicket("#9 Large", "Cluckin Bell", 1, 5.00, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)OrderStatus.InvalidCoupon, slave.Answer.Status);
        }

        [TestMethod]
        public void GuestBuyFailTest()
        {
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            userBuyerMocker.Setup(x => x.ValidateRegisteredUser())
                .Throws(new MarketException((int)EditCartItemStatus.DidntEnterSystem, ""));
            slave = new LotteryTicketSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyLotteryTicket("#9 Large", "Cluckin Bell", 1, 5.00, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)EditCartItemStatus.DidntEnterSystem, slave.Answer.Status);
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
