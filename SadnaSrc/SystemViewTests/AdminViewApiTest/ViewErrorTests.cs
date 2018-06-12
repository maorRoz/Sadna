using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class ViewErrorTests
    {
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IAdminDL> adminDbMocker;
        private Mock<IUserAdmin> userAdminMocker;
        private ViewErrorSlave slave;
        private string[] expectedErrors;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            adminDbMocker = new Mock<IAdminDL>();
            userAdminMocker = new Mock<IUserAdmin>();
            expectedErrors = new[] {"error1", "error2"};
            adminDbMocker.Setup(x => x.GetEventErrorLogReport()).Returns(expectedErrors);
        }
        [TestMethod]
        public void ViewErrorsSuccessTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new ViewErrorSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewError();
            Assert.AreEqual((int)ViewSystemErrorStatus.Success,slave.Answer.Status);
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expectedErrors.Length, actual.Length);
            for (int i = 0; i < expectedErrors.Length; i++)
            {
                Assert.AreEqual(expectedErrors[i], actual[i]);
            }
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userAdminMocker.Setup(x => x.GetAdminName()).Returns("Moshe");
            userAdminMocker.Setup(x => x.ValidateSystemAdmin()).Throws(new MarketException((int)ViewSystemErrorStatus.NotSystemAdmin, ""));
            adminDbMocker.Setup(x => x.IsUserExist(It.IsAny<string>())).Returns(true);
            slave = new ViewErrorSlave(adminDbMocker.Object, userAdminMocker.Object);
            slave.ViewError();
            Assert.AreEqual((int)ViewSystemErrorStatus.NotSystemAdmin, slave.Answer.Status);
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }


    }
}
