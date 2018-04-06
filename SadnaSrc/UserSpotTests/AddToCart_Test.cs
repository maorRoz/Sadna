using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{

    [TestClass]
    public class AddToCart_Test
    {
        private UserService userServiceGuestSession;
        private UserService userServiceRegisteredSession;
        private UserService userServiceLoggedSession;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceGuestSession = (UserService)marketSession.GetUserService();
            userServiceGuestSession.EnterSystem();
            userServiceRegisteredSession = null;
            userServiceLoggedSession = null;
        }


        [TestMethod]
        public void AddToGuestCartTest()
        {
            Assert.AreEqual(0,userServiceGuestSession.MarketUser.Cart.GetCartStorage());
            userServiceGuestSession.AddToCart("X","Health Potion",5.0, "Immediate",1);
            
        }

        [TestMethod]
        public void AddToSignedSaveCartTest()
        {

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

        private void DoSignUpSignIn(string name, string address, string password)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(name, password);
        }
        private void DoSignIn(string name, string password)
        {
            userServiceRegisteredSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceRegisteredSession.SignIn(name, password).Status);
        }
        private void DoSignUp(string name, string address, string password)
        {
            userServiceRegisteredSession = (UserService)marketSession.GetUserService();
            userServiceRegisteredSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceRegisteredSession.SignUp(name, address, password).Status);
            userServiceRegisteredSession.Synch();

        }
    }
}
