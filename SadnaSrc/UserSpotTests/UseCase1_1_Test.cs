using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{
    [TestClass]
    public class UseCase1_1_Test
    {
        private UserService userServiceSession;
        [TestInitialize]
        public void MarketBuilder()
        {
            var marketSession = new MarketYard();
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
        }

        [TestMethod]
        public void GuestHasNoPolicyTest()
        {
            User generatedGuest = userServiceSession.GetUser();
            Assert.AreEqual(0, generatedGuest.GetPolicies().Length);

        }

        [TestMethod]
        public void GuestDataTest()
        {
            User generatedGuest = userServiceSession.GetUser();
            object[] expectedData = { generatedGuest.SystemID, null, null, null };
            Assert.IsTrue(expectedData.SequenceEqual(generatedGuest.ToData()));
        }

        [TestMethod]
        public void NewGuestCartEmptyTest()
        {
            User generatedGuest = userServiceSession.GetUser();
            Assert.AreEqual(0, generatedGuest.GetCart().Length);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
        }
    }
}
