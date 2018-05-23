using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class ViewCartTests
    {
        private ViewCartSlave slave;
        private User user;
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;
        private string[] expectedCartStrings;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            user = new User(userDbMocker.Object, userID);
        }

        [TestMethod]
        public void ViewCartTest()
        {
            AddItemsToCart();
            slave = new ViewCartSlave(user);
            slave.ViewCart();
            string[] actualCart = slave.Answer.ReportList;
            Assert.AreEqual(expectedCartStrings.Length, actualCart.Length);
            for (int i = 0; i < expectedCartStrings.Length; i++)
            {
                Assert.AreEqual(expectedCartStrings[i],actualCart[i]);
            }
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            slave = new ViewCartSlave(null);
            slave.ViewCart();
            Assert.IsNull(slave.Answer.ReportList);
        }



        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }

        private void AddItemsToCart()
        {
            user.Cart.AddToCart("Health Potion", "X", 1, 5.0);
            user.Cart.AddToCart("Health Potion", "Y", 2, 0.5);
            user.Cart.AddToCart("Health Potion", "Y", 2, 6.0);
            user.Cart.AddToCart("Health Potion", "M", 5, 7.0);
            expectedCartStrings = new []
            {
                new CartItem("Health Potion", "X", 1, 5.0).ToString(),
                new CartItem("Health Potion", "Y", 2, 0.5).ToString(),
                new CartItem("Health Potion", "Y", 2, 6.0).ToString(),
                new CartItem("Health Potion", "M", 5, 7.0).ToString()
            };

        }
    }
}
