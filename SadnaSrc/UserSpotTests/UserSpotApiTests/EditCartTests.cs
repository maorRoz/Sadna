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
    public class EditCartTests
    {
        private EditCartItemSlave slave;
        private User user;
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;
        private CartItem item1;
        private CartItem item2;
        private CartItem[] expected;

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
        public void DidntEnterTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(null);
            slave.EditCartItem("Health Potion", "X", 2, 5.0);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void IncreaseCartItemTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("Health Potion", "X", 2, 5.0);
            expected[0].ChangeQuantity(2);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void DecreaseCartItemTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("Mana Potion", "Y", -1, 0.5);
            expected[1].ChangeQuantity(-1);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void DecreaseCartItemToZeroTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("Health Potion", "X", -1, 5.0);
            slave.EditCartItem("Mana Potion", "Y", -2, 0.5);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void DecreaseCartItemToNegativeTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("Health Potion", "X", -2, 5.0);
            slave.EditCartItem("Mana Potion", "Y", -3, 0.5);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void DecreaseCartItemSuccessThenFail()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("Mana Potion", "Y", -1, 0.5);
            expected[1].ChangeQuantity(-1);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
            slave.EditCartItem("Mana Potion", "Y", -1, 0.5);
            actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestMethod]
        public void NoItemToEditFoundTest()
        {
            AddItemsToCart();
            slave = new EditCartItemSlave(user);
            slave.EditCartItem("The Cake Is A LIE", "X", -2, 5.0);
            CartItem[] actual = user.Cart.GetCartStorage();
            compareCarts(expected, actual);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }

        private void AddItemsToCart()
        {
            expected = new[] {item1, item2};
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
