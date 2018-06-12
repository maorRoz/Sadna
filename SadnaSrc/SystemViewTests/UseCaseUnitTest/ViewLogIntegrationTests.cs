using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace SystemViewTests.UseCaseUnitTest
{
    [TestClass]
    public class ViewLogIntegrationTests
    {
        private SystemAdminService adminServiceSession;
        private IUserService userServiceSession;
        private MarketYard marketSession;
        [TestInitialize]
        public void MarketBuilder()
        {
            MarketLog.SetDB(MarketBackUpDB.Instance);
            MarketBackUpDB.Instance.CleanByForce();
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Arik1", "123");
        }

        [TestMethod]
        public void ViewLogSuccessTest()
        {
            var adminService = marketSession.GetSystemAdminService(userServiceSession);
            var answer = adminService.ViewLog();
            Assert.AreEqual((int)ViewSystemLogStatus.Success,answer.Status);
            Assert.AreEqual(10, answer.ReportList.Length);

        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

    }
}
