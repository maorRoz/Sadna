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
        private List<int> orderIDs;
        private IUserService userService;
        private OrderService orderService;


        [TestInitialize]
        public void BuildOrderPool()
        {
            market = MarketYard.Instance;
            userService = market.GetUserService();
            orderService= (OrderService)market.GetOrderService(ref userService); 
            orderService.LoginBuyer("Big Smoke","123");
            item1= new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
            orderIDs = new List<int>();
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
            orderIDs.Add(id); Assert.AreEqual(1, orderService.Orders.Count);
        }

        [TestMethod]
        public void TestNoOrder()
        {
            Assert.AreEqual(orderService.GetOrder(918721), null);
        }

        [TestMethod]
        public void TestOrderWithOneItem()
        {
            var order = orderService.InitOrder();
            int id = order.GetOrderID();
            orderIDs.Add(id);
            orderService.AddItemToOrder(id, item2.Store, item2.Name, item2.Price, item2.Quantity);
            Assert.AreEqual(7.0,
                orderService.FindOrderItemInOrder(id, "Cluckin Bell", "#9 Large").Price);
        }

        [TestMethod]
        public void TestOrderWithItems()
        {
            var order = orderService.InitOrder();
            int id = order.GetOrderID();
            orderIDs.Add(id);

            orderService.AddItemToOrder(id, item1.Store, item1.Name, item1.Price, item1.Quantity);
            orderService.AddItemToOrder(id, item2.Store, item2.Name, item2.Price, item2.Quantity);
            orderService.AddItemToOrder(id, item3.Store, item3.Name, item3.Price, item3.Quantity);
            Assert.AreEqual(25.50,
                orderService.GetOrder(id).GetPrice());
        }

        [TestMethod]
        public void TestDB()
        {
            var o = orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id);

            orderService.AddItemToOrder(id, item1.Store, item1.Name, item1.Price, item1.Quantity);
            orderService.AddItemToOrder(id, item2.Store, item2.Name, item2.Price, item2.Quantity);
            orderService.AddItemToOrder(id, item3.Store, item3.Name, item3.Price, item3.Quantity);

            orderService.SaveToDB();
            Assert.AreEqual(25.50,
                orderService.GetOrderFromDB(id).GetPrice());
        }

        [TestMethod]
        public void TestOrderRemoveItem()
        {
            var o = orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id);

            orderService.AddItemToOrder(id, item1.Store, item1.Name, item1.Price, item1.Quantity);
            orderService.AddItemToOrder(id, item2.Store, item2.Name, item2.Price, item2.Quantity);
            orderService.AddItemToOrder(id, item3.Store, item3.Name, item3.Price, item3.Quantity);
            orderService.RemoveItemFromOrder(id,"Cluckin Bell","#6 Extra Dip");
            Assert.AreEqual(17,
                orderService.GetOrder(id).GetPrice());
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
