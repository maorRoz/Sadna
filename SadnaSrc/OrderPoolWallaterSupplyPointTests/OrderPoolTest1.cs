using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class OrderPoolTest1
    {
        private MarketYard market;
        private OrderItem item1;
        private OrderItem item2;
        private OrderItem item3;
        private IUserService userService;
        private OrderService orderService;
        private OrderPoolSlave slave;


        [TestInitialize]
        public void BuildOrderPool()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
            orderService= (OrderService)market.GetOrderService(ref userService);
            IUserBuyer buyer = new UserBuyerHarmony(ref userService);
            slave = new OrderPoolSlave(ref buyer, new StoresSyncherHarmony(), OrderDL.Instance);
            orderService.GiveDetails("Big Smoke", "Grove Street", "54238521");
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
        }

        [TestMethod]
        public void TestNewService()
        {
            Assert.AreEqual(0, orderService.Orders.Count);
        }

        [TestMethod]
        public void TestNoOrder()
        {
            Assert.AreEqual(orderService.GetOrder(918721), null);
        }

        [TestMethod]
        public void TestEmptyOrder()
        {
            var order = slave.InitOrder("Big Smoke", "Grove Street");
            Assert.AreEqual("Big Smoke", order.GetUserName());
            Assert.AreEqual("Grove Street", order.GetShippingAddress());
        }

        [TestMethod]
        public void TestOrderWithOneItem()
        {
            OrderItem[] wrap = {item2};
            var order = slave.InitOrder(wrap, "Big Smoke", "Grove Street");
            Assert.AreEqual(1, order.GetItems().Count);
            Assert.IsNotNull(order.GetOrderItem("#9 Large", "Cluckin Bell"));
            Assert.AreEqual(7.0, order.GetOrderItem("#9 Large", "Cluckin Bell").Price);
        }


        [TestMethod]
        public void TestBrokenItem1()
        {
            try
            {
                item2 = new OrderItem(null, "#9 Large", 5.0, 2);
                OrderItem[] wrap = {item2};
                slave.InitOrder(wrap, "Big Smoke", "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)OrderItemStatus.InvalidDetails,e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenItem2()
        {
            try
            {
                item2 = new OrderItem("Cluckin Bell", null, 5.0, 2);
                OrderItem[] wrap = { item2 };
                slave.InitOrder(wrap, "Big Smoke", "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)OrderItemStatus.InvalidDetails, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenItem3()
        {
            try
            {
                item2 = new OrderItem("Cluckin Bell", "#9", 5.0, 0);
                OrderItem[] wrap = { item2 };
                slave.InitOrder(wrap, "Big Smoke", "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)OrderItemStatus.InvalidDetails, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenItem4()
        {
            try
            {
                OrderItem[] wrap = { };
                slave.InitOrder(wrap, "Big Smoke", "Grove Street");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)OrderItemStatus.InvalidDetails, e.Status);
            }
        }

        [TestMethod]
        public void TestOrderWithItems1()
        {
            OrderItem[] wrap = { item1 , item2, item3};
            var order = slave.InitOrder(wrap, "Big Smoke", "Grove Street");
            Assert.AreEqual(25.50,order.GetPrice());
        }

        [TestMethod]
        public void TestOrderWithItems2()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = slave.InitOrder(wrap, "Big Smoke", "Grove Street");
            Assert.AreEqual(2, order.GetOrderItem("#9","Cluckin Bell").Quantity);
        }

        [TestMethod]
        public void TestOrderWithItems3()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = slave.InitOrder(wrap, "Big Smoke", "Grove Street");
            Assert.AreEqual(2, order.GetOrderItem("#9", "Cluckin Bell").Quantity);
        }

        /*
        * Interface Tests
        */
        [TestMethod]
        public void TestDetailsFromUser()  
        {
            MarketAnswer ans = orderService.GiveDetails("Big Smoke", "Grove Street", "12345678");
            Assert.AreEqual((int)OrderStatus.Success, ans.Status);

        }

        [TestMethod]
        public void TestBadUserDetails1() 
        {
            MarketAnswer ans = orderService.GiveDetails(null, "Grove Street", "12345678");
            Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, ans.Status);
        }

        [TestMethod]
        public void TestBadUserDetails2()
        {
            MarketAnswer ans = orderService.GiveDetails("Big SMoke", null, "12345678");
            Assert.AreEqual((int) GiveDetailsStatus.InvalidNameOrAddress, ans.Status);
        }

        [TestMethod]
        public void TestBadUserDetails3()
        {
            MarketAnswer ans = orderService.GiveDetails("Big SMoke", "Grove Street", "123478");
            Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, ans.Status);
        }

        [TestMethod]
        public void TestBadUserDetails4()
        {
            MarketAnswer ans = orderService.GiveDetails("Big SMoke", "Grove Street", "asdfghjk");
            Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, ans.Status);
        }

        [TestMethod]
        public void TestBadUserDetails5()
        {
            MarketAnswer ans = orderService.GiveDetails("Big SMoke", "Grove Street", null);
            Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, ans.Status);
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            orderService.CleanSession();
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
