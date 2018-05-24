using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.PolicyComponent;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace IntegrationTests.PolicyOrder_Integration
{
    [TestClass]
    public class CheckPolicy_IntegrationTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserBuyer> userBuyerMocker;
        private Mock<IPublisher> publisherMock;

        private IUserService userServiceSession;
        private PurchaseItemSlave slave;

        private MarketYard marketSession;
        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";

        private OrderItem item1;
        private OrderItem item2;

        [TestInitialize]
        public void MarketBuilder()
        {
            publisherMock = new Mock<IPublisher>();
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userBuyerMocker = new Mock<IUserBuyer>();
            SupplyService.Instance.FixExternal();
            PaymentService.Instance.FixExternal();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            slave = new PurchaseItemSlave(userBuyerMocker.Object, new StoresSyncherHarmony(), OrderDL.Instance, publisherMock.Object, marketSession.GetPolicyChecker());
            InitPolicies();
        }

        [TestMethod]
        public void PurchasePolicyOKTest()
        {
            item1 = new OrderItem(store1, product1, 18.00, 3);
            slave.CheckPurchasePolicy(CreateOrder1());
        }

        [TestMethod]
        public void NoPurchasePolicyOKTest()
        {
            item1 = new OrderItem("Some store", "Some product", 20.00, 5);
            slave.CheckPurchasePolicy(CreateOrder1());
        }

        [TestMethod]
        public void PurchasePolicyFailedOnePolicy1()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 18.00, 1);
                slave.CheckPurchasePolicy(CreateOrder1());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedOnePolicy2()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 9.00, 3);
                slave.CheckPurchasePolicy(CreateOrder1());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedTwoPolicies()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 9.00, 1);
                slave.CheckPurchasePolicy(CreateOrder1());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedOneCondition1()
        {
            try
            {
                item2 = new OrderItem(store2, product2, 48.00, 8);
                slave.CheckPurchasePolicy(CreateOrder2());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedOneCondition2()
        {
            try
            {
                item2 = new OrderItem(store2, product2, 52.00, 4);
                slave.CheckPurchasePolicy(CreateOrder2());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedBothConditions()
        {
            try
            {
                item2 = new OrderItem(store2, product2, 52.00, 7);
                slave.CheckPurchasePolicy(CreateOrder2());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicySuccessBothConditions()
        {
            try
            {
                item2 = new OrderItem(store2, product2, 48.00, 4);
                slave.CheckPurchasePolicy(CreateOrder2());
            }
            catch (OrderException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedOneItem1()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 9.00, 1);
                item2 = new OrderItem(store2, product2, 48.00, 4);
                slave.CheckPurchasePolicy(CreateLargeOrder());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedOneItem2()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 18.00, 3);
                item2 = new OrderItem(store2, product2, 52.00, 8);
                slave.CheckPurchasePolicy(CreateLargeOrder());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicyFailedBothItems()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 9.00, 1);
                item2 = new OrderItem(store2, product2, 52.00, 8);
                slave.CheckPurchasePolicy(CreateLargeOrder());
                Assert.Fail();
            }
            catch (OrderException e)
            {
                Assert.AreEqual((int)OrderItemStatus.NotComplyWithPolicy, e.Status);
            }
        }

        [TestMethod]
        public void PurchasePolicySuccessBothItems()
        {
            try
            {
                item1 = new OrderItem(store1, product1, 18.00, 3);
                item2 = new OrderItem(store2, product2, 48.00, 4);
                slave.CheckPurchasePolicy(CreateLargeOrder());
            }
            catch (OrderException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
            PolicyHandler.Instance.CleanSession();
        }

        private void InitPolicies()
        {
            var policyHandler = PolicyHandler.Instance;
            policyHandler.CreateProductSimplePolicy(product1, ConditionType.QuantityGreater, "2");
            policyHandler.AddPolicy(0);
            policyHandler.CreateStoreSimplePolicy(store1, ConditionType.PriceGreater, "10.00");
            policyHandler.AddPolicy(0);
            policyHandler.CreateStockItemSimplePolicy(product2, store2, ConditionType.PriceLesser, "50.00");
            policyHandler.CreateStockItemSimplePolicy(product2, store2, ConditionType.QuantityLesser, "5");
            policyHandler.CreateStockItemPolicy(store2, product2, OperatorType.AND, 0, 1);
            policyHandler.AddPolicy(2);
        }

        private Order CreateOrder1()
        {
            Order order = slave.InitOrder("Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            return order;
        }

        private Order CreateOrder2()
        {
            Order order = slave.InitOrder("Big Smoke", "Grove Street");
            order.AddOrderItem(item2);
            return order;
        }

        private Order CreateLargeOrder()
        {
            Order order = slave.InitOrder("Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            order.AddOrderItem(item2);
            return order;
        }
    }
}
