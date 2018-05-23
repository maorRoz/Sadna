using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class RemoveFromCartTests
    {
        private RemoveFromCartSlave slave;
        private User user;
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;
        private CartItem item1;
        private CartItem item2;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            user = new User(userDbMocker.Object, userID);
            item1 = new CartItem("Health Potion", "X", 1, 5.0);
            item2 = new CartItem("Mana Potion", "Y", 2, 0.5);
        }

        [TestMethod]
        public void RemoveFromCartSuccessTest()
        {
            AddItemsToCart();
            slave = new RemoveFromCartSlave(user);
            slave.RemoveFromCart("Health Potion", "X", 5.0);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(new []{item2},actual);
            slave.RemoveFromCart("Mana Potion", "Y", 0.5);
            actual = user.Cart.GetCartStorage();
            compareCarts(new CartItem[0], actual);
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            AddItemsToCart();
            slave = new RemoveFromCartSlave(null);
            slave.RemoveFromCart("Health Potion", "X", 5.0);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(new []{item1,item2}, actual);
        }

        [TestMethod]
        public void NoItemToEditFoundTest()
        {
            AddItemsToCart();
            slave = new RemoveFromCartSlave(user);
            slave.RemoveFromCart("The Cake Is A LIE", "X",  5.0);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(new[]{item1,item2}, actual);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }

        private void AddItemsToCart()
        {
            user.Cart.AddToCart("Health Potion", "X", 1, 5.0);
            user.Cart.AddToCart("Mana Potion", "Y", 2, 0.5);

        }

        private void compareCarts(CartItem[] expected, CartItem[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
