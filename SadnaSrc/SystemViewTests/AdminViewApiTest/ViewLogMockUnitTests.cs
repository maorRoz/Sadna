using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class ViewLogMockUnitTests
    {
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IAdminDL> adminDbMocker;
        private Mock<IUserAdmin> userAdminMocker;
        private ViewLogSlave slave;
        private string[] expectedEvents;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            adminDbMocker = new Mock<IAdminDL>();
            userAdminMocker = new Mock<IUserAdmin>();
            expectedEvents = new[] { "event1", "event2" };
            adminDbMocker.Setup(x => x.GetEventLogReport()).Returns(expectedEvents);
        }
        [TestMethod]
        public void ViewEventLogsSuccessTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new ViewLogSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewLog();
            Assert.AreEqual((int)ViewSystemLogStatus.Success, slave.Answer.Status);
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expectedEvents.Length, actual.Length);
            for (int i = 0; i < expectedEvents.Length; i++)
            {
                Assert.AreEqual(expectedEvents[i],actual[i]);
            }

        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            userAdminMocker.Setup(x => x.ValidateSystemAdmin()).Throws(new MarketException((int)ViewSystemLogStatus.NotSystemAdmin, ""));
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new ViewLogSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewLog();
            Assert.AreEqual((int)ViewSystemLogStatus.NotSystemAdmin, slave.Answer.Status);
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
