using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UserSpotTests
{
    [TestClass]
    public class UserTest
    {
        private int stam = 0;
        [TestInitialize]
        public void stamNo()
        {
            stam = 2;
        }
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1,stam);
        }

        [TestMethod]
        public void fuckyou()
        {
            Assert.AreEqual(2,stam);
        }
    }
}
