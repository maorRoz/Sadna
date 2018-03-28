using System;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class OrderPoolTest1
    {
        private OrderService orderService;
        private User user;


        [TestInitialize]
        public void BuildOrderPool()
        {
            user = new User(5);
            orderService= new OrderService("Igor8757",false, new SQLiteConnection()); // TODO change the SQLite connection
        }

        [TestMethod]
        public void TestEmptyOrder()
        {
            Assert.AreEqual(0, orderService.getOrders().Count);
        }
    }
}
