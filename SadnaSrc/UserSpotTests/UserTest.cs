using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{
    [TestClass]
    public class UserTest
    {
        private UserService userServiceSession;
        [TestInitialize]
        public void MarketBuilder()
        {
            var marketSession = new MarketYard();
            userServiceSession = (UserService)marketSession.GetUserService();
        }
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void UserUniqueSystemIDTest()
        {
            Assert.AreEqual(1,3);
        }
    }
}
