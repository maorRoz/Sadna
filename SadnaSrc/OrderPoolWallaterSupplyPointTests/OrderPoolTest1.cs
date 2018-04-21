using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
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


        [TestInitialize]
        public void BuildOrderPool()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
            orderService= (OrderService)market.GetOrderService(ref userService);
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
        public void TestNewWorldOrder() // the new world order is upon us ... 
        {
            var order = orderService.InitOrder();
            int id = order.GetOrderID();
            Assert.AreEqual(1, orderService.Orders.Count);
        }

        [TestMethod]
        public void TestNoOrder()
        {
            Assert.AreEqual(orderService.GetOrder(918721), null);
        }

        [TestMethod]
        public void TestEmptyOrder()
        {
            var order = orderService.InitOrder();
            int id = order.GetOrderID();
            Assert.AreEqual(0.0,
                orderService.GetOrder(id).GetPrice());
        }

        [TestMethod]
        public void TestOrderWithOneItem()
        {
            OrderItem[] wrap = {item2};
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(7.0,
                orderService.FindOrderItemInOrder(id, "Cluckin Bell" , "#9 Large").Price);
        }


        [TestMethod]
        public void TestBrokenItem1()
        {
            try
            {
                item2 = new OrderItem(null, "#9 Large", 5.0, 2);
                OrderItem[] wrap = {item2};
                var order = orderService.InitOrder(wrap);
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
                var order = orderService.InitOrder(wrap);
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
                var order = orderService.InitOrder(wrap);
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
                var order = orderService.InitOrder(wrap);
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
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(25.50,
                orderService.GetOrder(id).GetPrice());
        }

        [TestMethod]
        public void TestOrderWithItems2()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(2,
                orderService.GetOrder(id).GetOrderItem("#9","Cluckin Bell").Quantity);
        }

        [TestMethod]
        public void TestOrderWithItems3()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(2,
                orderService.GetOrder(id).GetOrderItem("#9", "Cluckin Bell").Quantity);
        }

        [TestMethod]
        public void TestRemoveItem()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            order.RemoveOrderItem(item2);
            Assert.AreEqual(18.50,
                orderService.GetOrder(id).GetPrice());
        }

        [TestMethod]
        public void TestTwoOrders()
        {
            OrderItem[] wrap1 = { item1 };
            var order1 = orderService.InitOrder(wrap1);
            OrderItem[] wrap2 = { item2, item3 };
            var order2 = orderService.InitOrder(wrap2);
            Assert.AreEqual(2,orderService.Orders.Count);
        }

        [TestMethod]
        public void TestThreeOrders()
        {
            OrderItem[] wrap1 = { item1 };
            var order1 = orderService.InitOrder(wrap1);
            OrderItem[] wrap2 = { item2 };
            var order2 = orderService.InitOrder(wrap2);
            OrderItem[] wrap3 = { item3 };
            var order3 = orderService.InitOrder(wrap3);
            Assert.AreEqual(3, orderService.Orders.Count);
        }

        /*
         * DB Tests
         */

        [TestMethod]
        public void TestRemoveOrderFromDB()
        {
            OrderItem[] wrap1 = { item1 };
            var order1 = orderService.InitOrder(wrap1);
            int id = order1.GetOrderID();
            orderService.SaveToDB();
            orderService.RemoveOrderFromDB(id);
            Assert.AreEqual(null,
                orderService.GetOrderFromDB(id));

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
            try
            {
                orderService.GiveDetails(null, "Grove Street", "12345678");
                //Assert.Fail();
            }
            catch (MarketException m)
            {
                Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, m.Status);
            }
        }

        [TestMethod]
        public void TestBadUserDetails2()
        {
            try
            {
                orderService.GiveDetails("Big SMoke", null, "12345678");
                //Assert.Fail();
            }
            catch (MarketException m)
            {
                Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, m.Status);
            }
        }

        [TestMethod]
        public void TestBadUserDetails3()
        {
            try
            {
                orderService.GiveDetails("Big SMoke", "Grove Street", "123478");
                //Assert.Fail();
            }
            catch (MarketException m)
            {
                Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, m.Status);
            }
        }

        [TestMethod]
        public void TestBadUserDetails4()
        {
            try
            {
                orderService.GiveDetails("Big SMoke", "Grove Street", "asdfghjk");
                //Assert.Fail();
            }
            catch (MarketException m)
            {
                Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, m.Status);
            }
        }

        [TestMethod]
        public void TestBadUserDetails5()
        {
            try
            {
                orderService.GiveDetails("Big SMoke", "Grove Street", null);
                //Assert.Fail();
            }
            catch (MarketException m)
            {
                Assert.AreEqual((int)GiveDetailsStatus.InvalidNameOrAddress, m.Status);
            }
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
