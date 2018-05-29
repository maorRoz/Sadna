using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UseCaseUnitTest
{

    [TestClass]
    public class UseCase1_6_2_Test
    {
        private UserService userServiceGuestSession;
        private UserService userServiceRegisteredSession;
        private MarketYard marketSession;
        private CartItem item1;
        private CartItem item2;
        private List<CartItem> expected;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceGuestSession = (UserService)marketSession.GetUserService();
            userServiceGuestSession.EnterSystem();
            userServiceRegisteredSession = null;
            expected = new List<CartItem>();
            item1 = new CartItem("Health Potion", null, "X", 1, 5.0);
            item2 = new CartItem("Health Potion", null, "Y", 2, 0.5);
        }


        [TestMethod]
        public void RemoveItemFromGuestCartTest()
        {
            AddAllItems(userServiceGuestSession);
            Assert.AreEqual((int)RemoveFromCartStatus.Success, userServiceGuestSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual(null, userServiceGuestSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0));
            Assert.AreEqual(item2, userServiceGuestSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5));
        }

        [TestMethod]
        public void RemoveItemFromRegisteredCartTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddAllItems(userServiceRegisteredSession);
            userServiceRegisteredSession.SignUp("MaorRemoveItem1", "no-where", "123", "12345678");
            Assert.AreEqual((int)RemoveFromCartStatus.Success, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual(null, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5));
        }


        [TestMethod]
        public void RemoveItemFromGuestCartThenSignUpTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddAllItems(userServiceRegisteredSession);
            Assert.AreEqual((int)RemoveFromCartStatus.Success, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            userServiceRegisteredSession.SignUp("MaorRemoveItem2", "no-where", "123", "12345678");
            Assert.AreEqual(null, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0));
            Assert.AreEqual(item2, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5));
            Assert.AreEqual((int)RemoveFromCartStatus.Success, userServiceRegisteredSession.RemoveFromCart("Y", "Health Potion", 0.5).Status);
            Assert.AreEqual(null, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("X", "Health Potion", 5.0));
            Assert.AreEqual(null, userServiceRegisteredSession.MarketUser.Cart.SearchInCart("Y", "Health Potion", 0.5));
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            userServiceRegisteredSession = (UserService)marketSession.GetUserService();
            Assert.AreEqual((int)RemoveFromCartStatus.DidntEnterSystem, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.IsTrue(MarketException.HasErrorRaised());
        }

        [TestMethod]
        public void NoItemToRemoveFoundTest1()
        {
            userServiceRegisteredSession = DoEnter();
            AddItem1(userServiceRegisteredSession);
            Assert.AreEqual((int)RemoveFromCartStatus.Success, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("Y", "Health Potion", 0.5).Status);
            userServiceRegisteredSession.SignUp("MaorRemoveItem3", "no-where", "123", "12345678");
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("Y", "Health Potion", 0.5).Status);
        }

        [TestMethod]
        public void NoItemToRemoveFoundTest2()
        {
            userServiceRegisteredSession = DoEnter();
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("Y", "Health Potion", 0.5).Status);
            AddAllItems(userServiceRegisteredSession);
            userServiceRegisteredSession.SignUp("MaorRemoveItem4", "no-where", "123", "12345678");
            userServiceRegisteredSession.MarketUser.Cart.EmptyCart();
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("X", "Health Potion", 5.0).Status);
            Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, userServiceRegisteredSession.RemoveFromCart("Y", "Health Potion", 0.5).Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private UserService DoEnter()
        {
            UserService userService = (UserService)marketSession.GetUserService();
            userService.EnterSystem();
            return userService;
        }

        private void AddItem1(UserService userService)
        {
            expected.Add(item1);
            userService.AddToCart("Health Potion", null, "X", 1, 5.0);
        }

        private void AddItem2(UserService userService)
        {
            expected.Add(item2);
            userService.AddToCart("Health Potion", null, "Y", 2, 0.5);
        }

        private void AddAllItems(UserService userService)
        {
            AddItem1(userService);
            AddItem2(userService);
        }
    }
}
