using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{
    [TestClass]
    public class UseCase1_6_1_Test
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
        public void IncreaseCartItemGuestTest()
        {

        }

        [TestMethod]
        public void DecreaseCartItemGuestTest()
        {

        }

        [TestMethod]
        public void IncreaseCartItemToRegisteredTest()
        {

        }

        [TestMethod]
        public void DecreaseCartItemToRegisteredTest()
        {

        }

        [TestMethod]
        public void DecreaseCartItemToZeroTest()
        {

        }

        [TestMethod]
        public void DecreaseCartItemToNegativeTest()
        {

        }


    }
}
