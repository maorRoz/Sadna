using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace UserSpotTests
{
    [TestClass]
    public class UserTest
    {
        private MarketYard marketSession;
        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = new MarketYard();
        }
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1,1);
        }

        [TestMethod]
        public void fuckyou()
        {
            Assert.AreEqual(2,1);
        }
    }
}
