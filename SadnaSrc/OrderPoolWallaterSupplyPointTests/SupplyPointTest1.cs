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
            MarketDB.Instance.InsertByForce();
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
        public void TestCheckOrderDetails()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            supplyService.CheckOrderDetails(order);
        }

        [TestMethod]
        public void TestBrokenOrder1()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, null, "Grove Street");
            order.AddOrderItem(item1);
            try
            {
                supplyService.CheckOrderDetails(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenOrder2()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, "Big SMoke", null);
            order.AddOrderItem(item1);
            try
            {
                supplyService.CheckOrderDetails(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenOrder3()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(12, "Big SMoke", "Grove Street");
            order.AddOrderItem(item1);
            try
            {
                supplyService.CheckOrderDetails(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenOrder4()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(122326565, "Big SMoke", "Grove Street");
            order.AddOrderItem(item1);
            try
            {
                supplyService.CheckOrderDetails(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenOrder5()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, null, "Grove Street");
            try
            {
                supplyService.CheckOrderDetails(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }

        [TestMethod]
        public void TestCreateDelivery()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            supplyService.CreateDelivery(order);
        }

        [TestMethod]
        public void TestBadDelivery()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(12, "Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            try
            {
                supplyService.CreateDelivery(order);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)SupplyStatus.InvalidOrder, e.Status);
            }
        }


        [TestMethod]
        public void TestExternalSystemError()
        {
            supplyService.AttachExternalSystem();
            Order order = new Order(123456, "Big Smoke","Grove Street");
            order.AddOrderItem(item1);

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
