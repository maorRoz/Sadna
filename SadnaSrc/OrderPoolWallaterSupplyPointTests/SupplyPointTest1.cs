using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class SupplyPointTest1
    {
        private MarketYard market;
        private OrderItem item1;
        private OrderItem item2;
        private OrderItem item3;
        private UserService userService;
        private StoreService storeService;
        private OrderService orderService;
        private SupplyService supplyService;

        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            userService = new UserService();
            storeService = new StoreService(userService);
            orderService = (OrderService)market.GetOrderService(userService, storeService);
            orderService.setUsername("Big Smoke");
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
            supplyService = new SupplyService(userService,orderService);
        }

        [TestMethod]
        public void TestExternalSystemAttachment()
        {
            MarketAnswer ans = supplyService.AttachExternalSystem();
            Assert.AreEqual((int)SupplyStatus.Success, ans.Status);
        }

        [TestMethod]
        public void TestNoExternalSystem()
        {
            try
            {
                int orderId;
                orderService.CreateOrder(out orderId);
                supplyService.CreateDelivery(orderId, "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.NoSupplySystem, e.Status);
            }
        }

        [TestMethod]
        public void TestSuccesfulOrder()
        {
            supplyService.AttachExternalSystem();
            int orderId;
            orderService.CreateOrder(out orderId);
            orderService.AddItemToOrder(orderId, item1);
            orderService.AddItemToOrder(orderId, item2);
            MarketAnswer ans = supplyService.CreateDelivery(orderId, "Grove Street");
            Assert.AreEqual((int)SupplyStatus.Success, ans.Status);
        }

        [TestMethod]
        public void TestExternalSystemError()
        {
            supplyService.AttachExternalSystem();
            int orderId;
            orderService.CreateOrder(out orderId);
            orderService.AddItemToOrder(orderId, item1);
            orderService.AddItemToOrder(orderId, item2);
            supplyService.FuckUpExternal();
            try
            {
                MarketAnswer ans = supplyService.CreateDelivery(orderId, "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.SupplySystemError, e.Status);

            }
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {

            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
