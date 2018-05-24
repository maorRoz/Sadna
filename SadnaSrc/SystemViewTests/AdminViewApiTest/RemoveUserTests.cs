using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.AdminView;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class RemoveUserTests
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IAdminDL> adminDbMocker;
        private Mock<IUserAdmin> userAdminMocker;
        private RemoveUserSlave slave;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            adminDbMocker = new Mock<IAdminDL>();
            userAdminMocker = new Mock<IUserAdmin>();
        }

        [TestMethod]
        public void RemoveUserSuccessTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new RemoveUserSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.RemoveUser("who?");
            Assert.AreEqual(slave.Answer.Status, (int)RemoveUserStatus.Success);
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            userAdminMocker.Setup(x => x.ValidateSystemAdmin()).Throws(new MarketException((int)RemoveUserStatus.NotSystemAdmin, ""));
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new RemoveUserSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.RemoveUser("who?");
            Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, slave.Answer.Status);
        }

        [TestMethod]
        public void SelfTerminationBlockedTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new RemoveUserSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.RemoveUser("Moshe");
            Assert.AreEqual(slave.Answer.Status, (int)RemoveUserStatus.SelfTermination);

        }

        [TestMethod]
        public void NoUserToRemoveTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("SleepyAdmin");
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(false);
            slave = new RemoveUserSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.RemoveUser("Moshe");
            Assert.AreEqual(slave.Answer.Status, (int)RemoveUserStatus.NoUserFound);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }


    }
}
