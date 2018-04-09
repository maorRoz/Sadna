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
        public void TestOrderWithOneItem()
        {
            OrderItem[] wrap = {item1};
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(7.0,
                orderService.FindOrderItemInOrder(id, "#9 Large" , "Cluckin Bell").Price);
        }

        //TODO: add more here

        [TestMethod]
        public void TestOrderWithItems()
        {
            OrderItem[] wrap = { item1 , item2, item3};
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();
            Assert.AreEqual(25.50,
                orderService.GetOrder(id).GetPrice());
        }

       


        /*
        * Interface Tests
        */


        /*
         * DB Tests
         */
        [TestMethod]
        public void TestDB()
        {
            OrderItem[] wrap = { item1, item2, item3 };
            var order = orderService.InitOrder(wrap);
            int id = order.GetOrderID();

            orderService.SaveToDB();
            Assert.AreEqual(25.50,
                orderService.GetOrderFromDB(id).GetPrice());
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
