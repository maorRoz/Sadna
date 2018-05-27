using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UseCaseUnitTest
{
    [TestClass]
    public class UseCase_1_6_Test
    {
        private UserService userServiceGuestSession;
        private UserService userServiceRegisteredSession;
        private MarketYard marketSession;
        private string item1;
        private string item2;
        private string item3;
        private string item4;
        private List<string> expected;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceGuestSession = (UserService)marketSession.GetUserService();
            userServiceGuestSession.EnterSystem();
            userServiceRegisteredSession = null;
            expected = new List<string>();
            item1 = new CartItem("Health Potion", "X", 1, 5.0).ToString();
            item2 = new CartItem("Health Potion", "Y", 2, 0.5).ToString();
            item3 = new CartItem("Health Potion", "Y", 2, 6.0).ToString();
            item4 = new CartItem("Health Potion", "M", 5, 7.0).ToString();
        }


        [TestMethod]
        public void ViewCartGuestTest()
        {
            AddAllItems(userServiceGuestSession);
            Assert.AreEqual((int)ViewCartStatus.Success,userServiceGuestSession.ViewCart().Status);
            Assert.IsTrue(userServiceGuestSession.ViewCart().ReportList.SequenceEqual(expected));
        }

        [TestMethod]
        public void ViewCartRegisteredTest()
        {
            userServiceRegisteredSession = DoEnter();
            AddItem1(userServiceRegisteredSession);
            AddItem2(userServiceRegisteredSession);
            AddItem3(userServiceRegisteredSession);
            Assert.AreEqual((int)ViewCartStatus.Success, userServiceRegisteredSession.ViewCart().Status);
            Assert.IsTrue(userServiceRegisteredSession.ViewCart().ReportList.SequenceEqual(expected));
            Assert.IsTrue(userServiceRegisteredSession.ViewCart().ReportList.SequenceEqual(expected));
            userServiceRegisteredSession.SignUp("MaorViewCart1", "no-where", "123","12345678");
            AddItem4(userServiceRegisteredSession);
            Assert.AreEqual((int)ViewCartStatus.Success, userServiceRegisteredSession.ViewCart().Status);
            Assert.IsTrue(userServiceRegisteredSession.ViewCart().ReportList.SequenceEqual(expected));
        }


        [TestMethod]
        public void DidntEnterTest()
        {
            userServiceRegisteredSession = (UserService)marketSession.GetUserService();
            Assert.AreEqual((int)ViewCartStatus.DidntEnterSystem, userServiceRegisteredSession.ViewCart().Status);
            Assert.IsTrue(MarketException.HasErrorRaised());
        }
        [TestMethod]
        public void ViewEmptyCartTest()
        {
            Assert.AreEqual((int)ViewCartStatus.Success, userServiceGuestSession.ViewCart().Status);
            Assert.IsTrue(userServiceGuestSession.ViewCart().ReportList.SequenceEqual(expected));
        }

        [TestMethod]
        public void ViewEmptyCartTest2()
        {
            AddAllItems(userServiceGuestSession);
            userServiceGuestSession.MarketUser.Cart.EmptyCart();
            expected.Clear();
            Assert.AreEqual((int)ViewCartStatus.Success, userServiceGuestSession.ViewCart().Status);
            Assert.IsTrue(userServiceGuestSession.ViewCart().ReportList.SequenceEqual(expected));
            AddItem1(userServiceGuestSession);
            Assert.AreEqual((int)ViewCartStatus.Success, userServiceGuestSession.ViewCart().Status);
            Assert.IsTrue(userServiceGuestSession.ViewCart().ReportList.SequenceEqual(expected));

        }

        [TestCleanup]
        public void CartServiceTestCleanUp()
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
            userService.AddToCart("Health Potion", "X", 1, 5.0);
        }

        private void AddItem2(UserService userService)
        {
            expected.Add(item2);
            userService.AddToCart("Health Potion", "Y", 2, 0.5);
        }

        private void AddItem3(UserService userService)
        {
            expected.Add(item3);
            userService.AddToCart("Health Potion", "Y", 2, 6.0);
        }

        private void AddItem4(UserService userService)
        {
            expected.Add(item4);
            userService.AddToCart("Health Potion", "M", 5, 7.0);
        }

        private void AddAllItems(UserService userService)
        {
            AddItem1(userService);
            AddItem2(userService);
            AddItem3(userService);
            AddItem4(userService);
        }
    }
}
