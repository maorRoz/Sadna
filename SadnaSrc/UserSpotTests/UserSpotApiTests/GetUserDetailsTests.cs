using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class GetUserDetailsTests
    {
        private GetUserDetailsSlave slave;
        private User guest;
        private RegisteredUser registered;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            guest = new User(userDbMocker.Object, userID);
            registered = new RegisteredUser(userDbMocker.Object, userID, "Moshe", "Here 3", "123", "12345678",
                new CartItem[0],
                new StatePolicy[0], new StoreManagerPolicy[0]);
        }

        [TestMethod]
        public void GuestDetailsTest()
        {
            slave = new GetUserDetailsSlave(guest);
            slave.GetUserDetails();
            var actual = slave.Answer.ReportList;
            var expectedDetails = new string[] {null, null, null};
            Assert.AreEqual(expectedDetails.Length, actual.Length);
            for (int i = 0; i < expectedDetails.Length; i++)
            {
                Assert.AreEqual(expectedDetails[i], actual[i]);
            }
        }

        [TestMethod]
        public void RegisteredDetailsTest()
        {
            slave = new GetUserDetailsSlave(registered);
            slave.GetUserDetails();
            var expectedDetails = new[] { "Moshe", "Here 3","12345678" };
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expectedDetails.Length, actual.Length);
            for (int i = 0; i < expectedDetails.Length; i++)
            {
                Assert.AreEqual(expectedDetails[i], actual[i]);
            }

        }

        [TestMethod]
        public void DidntEnterTest()
        {
            slave = new GetUserDetailsSlave(null);
            slave.GetUserDetails();
            Assert.IsNull(slave.Answer.ReportList);
        }
    }
}
