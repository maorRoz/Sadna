using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class EnterSystemTests
    {
        private User generatedGuest;

        [TestInitialize]
        public void MarketBuilder()
        {
            var marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            var userDbMocker = new Mock<IUserDL>();
            userDbMocker.Setup(x => x.SaveUser(It.IsAny<User>()));
            userDbMocker.Setup(x => x.GetAllSystemIDs()).Returns(new[] {5000, 7000, 9000});
            generatedGuest = new EnterSystemSlave(userDbMocker.Object).EnterSystem();
        }    

        [TestMethod]
        public void GuestHasNoPolicyTest()
        {
            Assert.IsFalse(generatedGuest.HasStorePolicies());
            Assert.IsFalse(generatedGuest.IsRegisteredUser());

        }

        [TestMethod]
        public void GuestDataTest()
        {
            object[] expectedData = { generatedGuest.SystemID, null, null, null,null };
            Assert.IsTrue(expectedData.SequenceEqual(generatedGuest.ToData()));
        }

        [TestMethod]
        public void NewGuestCartEmptyTest()
        {
            Assert.AreEqual(0, generatedGuest.Cart.GetCartStorage().Length);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
