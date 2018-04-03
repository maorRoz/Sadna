﻿using System;
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
        private UserService userService;
        private StoreService storeService;
        private OrderService orderService;


        [TestInitialize]
        public void BuildOrderPool()
        {
            market = MarketYard.Instance;
            userService = new UserService();
            storeService = new StoreService(userService);
            orderService= (OrderService)market.GetOrderService(userService, storeService);
            orderService.setUsername("Big Smoke");
            item1= new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
            orderIDs = new List<int>();
        }

        [TestMethod]
        public void TestNewService()
        {
            Assert.AreEqual(0, orderService.getOrders().Count);
        }

        [TestMethod]
        public void TestNewWorldOrder() // the new world order is upon us ... 
        {
            var o = orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id); Assert.AreEqual(1, orderService.getOrders().Count);
        }

        [TestMethod]
        public void TestNoOrder()
        {
            Assert.AreEqual(orderService.getOrder(918721), null);
        }

        [TestMethod]
        public void TestOrderWithOneItem()
        {
            var o= orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id);
            orderService.AddItemToOrder(id, item2);
            Assert.AreEqual(7.0,
                orderService.FindOrderItemInOrder(id, "Cluckin Bell", "#9 Large").GetPrice());
        }

        [TestMethod]
        public void TestOrderWithItems()
        {
            var o = orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id);

            orderService.AddItemToOrder(id, item1);
            orderService.AddItemToOrder(id, item2);
            orderService.AddItemToOrder(id, item3);
            Assert.AreEqual(25.50,
                orderService.getOrder(id).GetPrice());
        }

        [TestMethod]
        public void TestOrderRemoveItem()
        {
            var o = orderService.InitOrder();
            int id = o.GetOrderID();
            orderIDs.Add(id);

            orderService.AddItemToOrder(id, item1);
            orderService.AddItemToOrder(id, item2);
            orderService.AddItemToOrder(id, item3);
            orderService.RemoveItemFromOrder(id,"Cluckin Bell","#6 Extra Dip");
            Assert.AreEqual(17,
                orderService.getOrder(id).GetPrice());
        }
        [TestCleanup]
        public void UserTestCleanUp()
        {

            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
