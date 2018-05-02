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
    public class OrderPoolBuyItemTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IOrderDL> orderDbMocker;
        private Mock<IUserBuyer> userBuyerMocker;
        private Mock<IStoresSyncher> storeSyncherMock;

        private PurchaseItemSlave slave;
        private OrderItem item;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            orderDbMocker = new Mock<IOrderDL>();
            userBuyerMocker = new Mock<IUserBuyer>();
            storeSyncherMock = new Mock<IStoresSyncher>();
            item = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            SupplyService.Instance.FixExternal();
            PaymentService.Instance.FixExternal();
        }

        [TestMethod]
        public void BuySingleItemTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, "Big Smoke", "Grove Street",
                "12345678");
            Assert.AreEqual(1, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            OrderItem actual = order.GetOrderItem("#9 Large", "Cluckin Bell");
            Assert.AreEqual(item.Name, actual.Name);
            Assert.AreEqual(item.Price, actual.Price);
            Assert.AreEqual(item.Quantity, actual.Quantity);
            Assert.AreEqual(item.Store, actual.Store);
        }

        [TestMethod]
        public void BuyNonExistantItemTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>()))
                .Throws(new MarketException(MarketError.LogicError, "some message"));
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, "Big Smoke", "Grove Street",
                "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)OrderStatus.InvalidUser, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyItemUserFailTest1()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                    (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, null, "Grove Street",
                "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)SupplyStatus.InvalidOrder, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyItemUserFailTest2()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, "Big Smoke", null,
                "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)SupplyStatus.InvalidOrder, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyItemUserFailTest3()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, "Big Smoke", "Grove Street",
                null);
            Assert.IsNull(order);
            Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyItemUserFailTest4()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, null, "Big Smoke", "Grove Street",
                "2");
            Assert.IsNull(order);
            Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyItemWithCouponTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(3.0);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, "D1", "Big Smoke", "Grove Street",
                "12345678");
            Assert.AreEqual(1, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            OrderItem actual = order.GetOrderItem("#9 Large", "Cluckin Bell");
            Assert.AreEqual(3.0, actual.Price);
        }

        [TestMethod]
        public void BuyItemFailedCouponTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutItem
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>())).Returns(item);
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new MarketException(MarketError.LogicError, "some message"));
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseItemSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object);
            Order order = slave.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.00, "D1", "Big Smoke", "Grove Street",
                "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)OrderStatus.InvalidCoupon, slave.Answer.Status);
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
