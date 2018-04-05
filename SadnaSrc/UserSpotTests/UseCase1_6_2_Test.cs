using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{
    /// <summary>
    /// Summary description for UseCase1_6_2_Test
    /// </summary>
    [TestClass]
    public class UseCase1_6_2_Test
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
        public void RemoveItemFromGuestCartTest()
        {

        }

        [TestMethod]
        public void RemoveItemFromRegisteredCartTest()
        {

        }

        [TestMethod]
        public void RemoveItemFromLoggedCartTest()
        {

        }

        [TestMethod]
        public void RemoveItemFromGuestCartThenSignUpTest()
        {

        }

        [TestMethod]
        public void RemoveItemFromGuestCartThenSignedInTest()
        {

        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceGuestSession.CleanSession();
            userServiceRegisteredSession?.CleanSession();
            userServiceLoggedSession?.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
