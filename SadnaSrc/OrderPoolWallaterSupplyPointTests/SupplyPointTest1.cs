using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.SupplyPoint;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class SupplyPointTest1
    {
        private MarketYard market;
        private OrderItem item1;
        private OrderItem item2;
        private OrderItem item3;
        private SupplyService supplyService;

        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
            supplyService = (SupplyService)market.GetSupplyService();
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
                List<OrderItem> items = new List<OrderItem>();
                items.Add(item1);
                Order order = new Order(123456,"Grove Street");
                supplyService.CreateDelivery(order);
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
            Order order = new Order(123456, "Grove Street");
            supplyService.CreateDelivery(order);
        }

        [TestMethod]
        public void TestExternalSystemError()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, "Grove Street");
            supplyService.BreakExternal();
            try
            {
                supplyService.CreateDelivery(order);
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

            MarketYard.CleanSession();
        }
    }
}
