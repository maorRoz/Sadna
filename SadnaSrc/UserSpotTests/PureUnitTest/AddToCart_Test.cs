using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.PureUnitTest
{

    [TestClass]
    public class AddToCart_Test
    {
        private UserService userServiceGuestSession;
        private UserService userServiceRegisteredSession;
        private UserService userServiceLoggedSession;
        private MarketYard marketSession;
        private CartItem item1;
        private CartItem item2;
        private CartItem item3;
        private CartItem item4;
        private List<CartItem> expected;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceGuestSession = (UserService)marketSession.GetUserService();
            userServiceGuestSession.EnterSystem();
            userServiceRegisteredSession = null;
            userServiceLoggedSession = null;
            expected = new List<CartItem>();
            item1 = new CartItem("Health Potion", "X", 1, 5.0, "Immediate");
            item2 = new CartItem("Health Potion", "Y", 2, 0.5, "Immediate");
            item3 = new CartItem("Health Potion", "Y", 2, 6.0, "Immediate");
            item4 = new CartItem("Health Potion", "M", 5, 7.0, "Immediate");
        }


        [TestMethod]
        public void AddToGuestCartTest()
        {
            Assert.AreEqual(0,userServiceGuestSession.MarketUser.Cart.GetCartStorage().Length);
            addAllItems(userServiceGuestSession);
            Assert.IsTrue(expected.ToArray().SequenceEqual(userServiceGuestSession.MarketUser.Cart.GetCartStorage()));

        }

        [TestMethod]
        public void AddToSignedSaveCartTest()
        {
            userServiceRegisteredSession = DoEnter();
            addItem1();
            addItem2();
            Assert.IsTrue(expected.ToArray().SequenceEqual(userServiceRegisteredSession.MarketUser.Cart.GetCartStorage()));
        }

        [TestMethod]
        public void AddToLoggedSaveCartTest()
        {

        }

        [TestMethod]
        public void fromGuestToSignedSaveCartTest()
        {

        }

        [TestMethod]
        public void fromGuestToLoggedSaveCartTest()
        {

        }

        [TestMethod]
        public void fromSignedToLoggedSaveCartTest()
        {

        }

        [TestMethod]
        public void fromLoggedToLoggedSaveCartTest()
        {

        }

        [TestMethod]
        public void fromGuestToSignedToLoggedToLoggedSaveCartTest()
        {

        }




        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceLoggedSession?.CleanSession();
            userServiceRegisteredSession?.CleanSession();
            userServiceGuestSession.CleanSession();
            MarketYard.CleanSession();
        }

        private UserService DoEnter()
        {
            UserService userService = (UserService)marketSession.GetUserService();
            userService.EnterSystem();
            return userService;
        }

        private void DoSignUp(string name, string address, string password)
        {
            userServiceRegisteredSession = DoEnter();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceRegisteredSession.SignUp(name, address, password).Status);
            userServiceRegisteredSession.Synch();

        }

        private void DoSignIn(string name, string password)
        {
            userServiceLoggedSession = DoEnter();
            Assert.AreEqual((int)SignInStatus.Success, userServiceLoggedSession.SignIn(name, password).Status);
        }

        private void DoSignUpSignIn(string name, string address, string password)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(name, password);
        }

        private void addItem1()
        {
            expected.Add(item1);
            userServiceGuestSession.AddToCart("Health Potion", "X", 1, 5.0, "Immediate");
        }

        private void addItem2()
        {
            expected.Add(item2);
            userServiceGuestSession.AddToCart("Health Potion", "Y", 2, 0.5, "Immediate");
        }

        private void addItem3()
        {
            expected.Add(item3);
            userServiceGuestSession.AddToCart("Health Potion", "Y", 2, 6.0, "Immediate");
        }

        private void addItem4()
        {
            expected.Add(item4);
            userServiceGuestSession.AddToCart("Health Potion", "M", 5, 7.0, "Immediate");
        }

        private void addAllItems(UserService userService)
        {
            addItem1();
            addItem2();
            addItem3();
            addItem4();
        }

    }
}
