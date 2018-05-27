using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
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
            item1 = new OrderItem("Cluckin Bell", null, "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", null, "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", null, "#6 Extra Dip", 8.50, 1);
            supplyService = (SupplyService)market.GetSupplyService();
            supplyService.FixExternal();
        }


       

        [TestMethod]
        public void TestCheckOrderDetails()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            supplyService.CheckOrderDetails(order);
        }

        [TestMethod]
        public void TestBrokenOrder1()
        {
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
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            order.AddOrderItem(item1);
            supplyService.CreateDelivery(order);
        }

        [TestMethod]
        public void TestBadDelivery()
        {
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

            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
