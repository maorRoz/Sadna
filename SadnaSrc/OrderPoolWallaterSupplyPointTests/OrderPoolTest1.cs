using System;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class OrderPoolTest1
    {
        private MarketYard market;
        private OrderService orderService;
        private User user;
        private OrderItem item1;
        private OrderItem item2;
        private OrderItem item3;

        // TODO add marketyard to init DB

        [TestInitialize]
        public void BuildOrderPool()
        {
            user = new User(5);
            market = new MarketYard();
            orderService= (OrderService)market.GetOrderService();
            orderService.setUsername("Big Smoke");
            item1= new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra dip", 8.50, 1);

        }

        [TestMethod]
        public void TestNewService()
        {
            Assert.AreEqual(0, orderService.getOrders().Count);
        }

        [TestMethod]
        public void TestNewWorldOrder() // the new world order is upon us ... 
        {
            orderService.CreateOrder();
            Assert.AreEqual(1, orderService.getOrders().Count);
        }

        [TestMethod]
        public void TestOrderWithOneItem()
        {
            var o= orderService.CreateOrder();
            int id = o.GetOrderID();
            orderService.AddItemToOrder(id, item2);
            Assert.AreEqual(7.0,
                orderService.FindOrderItemInOrder(id, "Cluckin Bell", "#9 Large").GetPrice());
        }
    }
}
