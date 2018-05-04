using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class OrderPoolBuyEverythingTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IOrderDL> orderDbMocker;
        private Mock<IUserBuyer> userBuyerMocker;
        private Mock<IStoresSyncher> storeSyncherMock;
        private Mock<IPublisher> publisherMock;

        private PurchaseEverythingSlave slave;
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
            publisherMock = new Mock<IPublisher>();
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            PaymentService.Instance.FixExternal();
            SupplyService.Instance.FixExternal();

        }

        [TestMethod]
        public void BuyOneItemTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new[]{item2});
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object,publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(null, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            OrderItem actual = order.GetOrderItem("#9 Large", "Cluckin Bell");
            Assert.AreEqual(item2.Name, actual.Name);
            Assert.AreEqual(item2.Price, actual.Price);
            Assert.AreEqual(item2.Quantity, actual.Quantity);
            Assert.AreEqual(item2.Store, actual.Store);
        }

        [TestMethod]
        public void BuySeveralItemTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new [] {item1, item2 });
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object,publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(null, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNotNull(order);
            Assert.AreEqual(2, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            Assert.IsNotNull(order.GetOrderItem("#9", "Cluckin Bell"));
            Assert.AreEqual(17.00, order.GetPrice());
        }

        [TestMethod]
        public void BuyEmptyCartTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new OrderItem[] {});
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object, publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(null, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)SupplyStatus.InvalidOrder, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyWithDiscountTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new OrderItem[] { item1, item2 });
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (item1.Name, item1.Store, item1.Quantity, It.IsAny<string>())).Returns(3.0);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object, publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(new []{"D1", null}, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNotNull(order);
            Assert.AreEqual(2, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            Assert.IsNotNull(order.GetOrderItem("#9", "Cluckin Bell"));
            Assert.AreEqual(13.00, order.GetPrice());
        }

        [TestMethod]
        public void BuyAllDiscountTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new OrderItem[] { item1, item2 });
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(3.0);
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object, publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(new string[] { "D1", "D2"}, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNotNull(order);
            Assert.AreEqual(2, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            Assert.IsNotNull(order.GetOrderItem("#9", "Cluckin Bell"));
            Assert.AreEqual(9.00, order.GetPrice());
        }

        [TestMethod]
        public void BuyFailedDiscountTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new OrderItem[] { item1 });
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                    (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new MarketException(MarketError.LogicError, "some message"));
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object, publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(new string[] { "D1" }, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)OrderStatus.InvalidCoupon, slave.Answer.Status);
        }

        [TestMethod]
        public void BuyHalfFailedDiscountTest()
        {
            userBuyerMocker.Setup(x => x.CheckoutAll()).Returns(new OrderItem[] { item1, item2 });
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (item1.Name, item1.Store, item1.Quantity, It.IsAny<string>())).Returns(3.0);
            storeSyncherMock.Setup(x => x.GetPriceFromCoupon
                (item2.Name, item2.Store, item2.Quantity, It.IsAny<string>()))
                .Throws(new MarketException(MarketError.LogicError, "some message"));
            orderDbMocker.Setup(x => x.RandomOrderID()).Returns(100010);
            slave = new PurchaseEverythingSlave(userBuyerMocker.Object, storeSyncherMock.Object, orderDbMocker.Object, publisherMock.Object);
            Order order = slave.BuyEverythingFromCart(new string[] { "D1", "D2" }, "Big Smoke", "Grove Street", "12345678");
            Assert.IsNull(order);
            Assert.AreEqual((int)OrderStatus.InvalidCoupon, slave.Answer.Status);
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
