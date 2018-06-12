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
    public class ViewErrorIntegrationTests
    {
        private SystemAdminService adminServiceSession;
        private IUserService userServiceSession;
        private MarketYard marketSession;
        [TestInitialize]
        public void MarketBuilder()
        {
            MarketBackUpDB.Instance.CleanByForce();
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
            userServiceSession.EnterSystem();
        }

        [TestMethod]
        public void ViewErrorSuccessTest()
        {
            var answer = userServiceSession.SignIn("Moshe", "1234");
            Assert.AreEqual((int)SignInStatus.NoUserFound, answer.Status);
            answer = userServiceSession.SignIn("Arik1", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            var adminService = marketSession.GetSystemAdminService(userServiceSession);
            answer = adminService.ViewError();
            Assert.AreEqual((int)ViewSystemLogStatus.Success, answer.Status);
            Assert.AreEqual(1,answer.ReportList.Length);
        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
