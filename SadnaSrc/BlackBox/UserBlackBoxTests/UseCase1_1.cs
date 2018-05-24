using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace BlackBox.UserBlackBoxTests
{
    [TestClass]
    public class UseCase1_1
    {
        private IUserBridge _bridge;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            _bridge = UserDriver.getBridge();
        }

        [TestMethod]
        public void SuccessGuestEntry()
        {
            Assert.AreEqual((int)EnterSystemStatus.Success, _bridge.EnterSystem().Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }

    }
}