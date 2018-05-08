using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class ViewUsersTests
    {
        private ViewUsersSlave slave;
        private User user;
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            user = new User(userDbMocker.Object, userID);
        }

        [TestMethod]
        public void NoUsersInDBTest()
        {
            slave = new ViewUsersSlave(user, userDbMocker.Object);
            slave.ViewUsers();
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void GetUsersInDBTest()
        {
            userDbMocker.Setup(x => x.UserNamesInSystem()).Returns(new[] { "Pnina", "Maor", "Zohar" });
            slave = new ViewUsersSlave(user, userDbMocker.Object);
            slave.ViewUsers();
            var expectedUsers = new[] { "Pnina", "Maor", "Zohar" };
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expectedUsers.Length, actual.Length);
            for (int i = 0; i < expectedUsers.Length; i++)
            {
                Assert.AreEqual(expectedUsers[i],actual[i]);
            }

        }

        [TestMethod]
        public void DidntEnterTest()
        {
            slave = new ViewUsersSlave(null,userDbMocker.Object);
            slave.ViewUsers();
            Assert.IsNull(slave.Answer.ReportList);
        }
    }
}
