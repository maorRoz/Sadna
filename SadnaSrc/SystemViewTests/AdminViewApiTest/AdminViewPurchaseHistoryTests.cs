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
    public class AdminViewPurchaseHistoryTests
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IAdminDL> adminDbMocker;
        private Mock<IUserAdmin> userAdminMocker;
        private AdminViewPurchaseHistorySlave slave;

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
        public void ViewPurchaseHistoryByUserSuccessTest()
        {
            string[] expected = {"some history1","some history2"};
            adminDbMocker.Setup(x => x.IsUserNameExistInHistory(It.IsAny<string>()))
                .Returns(true);
            adminDbMocker.Setup(x => x.GetPurchaseHistory(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(expected);
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByUser("who?");
            string[] actual = slave.Answer.ReportList;
            Assert.AreEqual(expected.Length,actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i],actual[i]);
            }
        }

        [TestMethod]
        public void ViewPurchaseHistoryByStoreSuccessTest()
        {
            string[] expected = { "some history1", "some history2" };
            adminDbMocker.Setup(x => x.IsStoreExistInHistory(It.IsAny<string>()))
                .Returns(true);
            adminDbMocker.Setup(x => x.GetPurchaseHistory(It.IsAny<string>(),It.IsAny<string>()))
                .Returns(expected);
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByStore("who?");
            string[] actual = slave.Answer.ReportList;
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void NotSystemAdminTest1()
        {
            adminDbMocker.Setup(x => x.IsUserNameExistInHistory(It.IsAny<string>()))
                .Returns(true);
            userAdminMocker.Setup(x => x.ValidateSystemAdmin())
                .Throws(new MarketException((int)ViewPurchaseHistoryStatus.NotSystemAdmin, ""));
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByUser("who?");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, slave.Answer.Status);
        }

        [TestMethod]
        public void NotSystemAdminTest2()
        {
            adminDbMocker.Setup(x => x.IsStoreExistInHistory(It.IsAny<string>()))
                .Returns(true);
            userAdminMocker.Setup(x => x.ValidateSystemAdmin())
                .Throws(new MarketException((int)ViewPurchaseHistoryStatus.NotSystemAdmin, ""));
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByStore("who?");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, slave.Answer.Status);
        }

        [TestMethod]
        public void NoUserTest()
        {
            adminDbMocker.Setup(x => x.IsUserNameExistInHistory(It.IsAny<string>()))
                .Returns(false);
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByUser("who?");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, slave.Answer.Status);
        }

        [TestMethod]
        public void NoStoreTest()
        {
            adminDbMocker.Setup(x => x.IsStoreExistInHistory(It.IsAny<string>()))
                .Returns(false);
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new AdminViewPurchaseHistorySlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewPurchaseHistoryByStore("who?");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, slave.Answer.Status);
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }


    }
}
