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
            market = MarketYard.Instance;
            userService = market.GetUserService();
            orderService= (OrderService)market.GetOrderService(ref userService, market.GetPaymentService(),market.GetSupplyService()); 
            orderService.LoginBuyer("Big Smoke","123");
            item1= new OrderItem("Cluckin Bell", "#9", 5.00, 2);
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
        public void TestOrderWithItems()
        {
            OrderItem[] wrap = { item1 , item2, item3};
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(25.50,
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

        /*
         * DB Tests
         */
        [TestMethod]
        public void TestGetOrderFromDB1()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            orderService.SaveOrderToDB(order);
            Assert.AreEqual(25.50,
                orderService.GetOrderFromDB(id).GetPrice());
        }

        [TestMethod]
        public void TestGetOrderFromDB2()
        {
            OrderItem[] wrap1 = { item1 };
            var order1 = orderService.InitOrder(wrap1);
            OrderItem[] wrap2 = { item2, item3 };
            var order2 = orderService.InitOrder(wrap2);
            orderService.SaveToDB();
            double price1 = orderService.GetOrderFromDB(order1.GetOrderID()).GetPrice();
            double price2 = orderService.GetOrderFromDB(order2.GetOrderID()).GetPrice();
            Assert.IsTrue(price1 < price2);

        }

        [TestMethod]
        public void TestRemoveOrderFromDB2()
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
        //TODO: Test wonk work until Rebase
        [TestMethod]
        public void TestImmediateBuy()
        {
            orderService.LoginBuyer("Big Smoke","Grove Street");
            MarketAnswer ans = orderService.BuyItemFromImmediate("#9 Large", "Cluckin Bell", 1, 7.0);
            Assert.AreEqual((int)OrderStatus.Success,ans.Status);

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
