using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.UserSpot;

namespace DbRobustnessTests
{
    [TestClass]
    public class UserSpotApiNoDb
    {
        private IUserService userService;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            var marketSession = MarketYard.Instance;
            userService = marketSession.GetUserService();
        }
        [TestMethod]
        public void EnterSystemTest()
        {
            MarketDB.ToDisable = true;
            var answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.NoDB,answer.Status);
        }

        [TestMethod]
        public void SignUpTest()
        {
            var answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            MarketDB.ToDisable = true;
            answer = userService.SignUp("NoDbUser", "123", "123", "12345678");
            Assert.AreEqual((int)SignUpStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void SignInTest()
        {
            var answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            MarketDB.ToDisable = true;
        }

        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
