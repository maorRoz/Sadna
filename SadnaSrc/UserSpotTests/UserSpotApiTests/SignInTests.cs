using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;
using SadnaSrc.UserSpot;
namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class SignInTests
    {
        private SignInSlave slave;
        private User guestUser;
        private readonly int registeredUserID = 5000;
        private readonly string registeredUserName = "MaorRegister";
        private readonly string registeredUserAddress = "Here 3";
        private readonly string registeredUserPassword = "123";
        private readonly string encryptedUserPassword = UserSecurityService.GetSecuredPassword("123");
        private readonly string registeredUserCreditCard = "12345678";
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            userDbMocker.Setup(x=>x.LoadUser(It.IsAny<object[]>(),It.IsAny<CartItem[]>()))
                .Returns(new RegisteredUser(userDbMocker.Object, registeredUserID, registeredUserName, registeredUserAddress,
                    encryptedUserPassword, registeredUserCreditCard, new CartItem[0],
                    new[] { new StatePolicy(StatePolicy.State.RegisteredUser) }, new StoreManagerPolicy[0]));
            guestUser = new User(userDbMocker.Object, registeredUserID);
            userDbMocker.Setup(x => x.UserNamesInSystem()).Returns(new[] {"MaorLogin"});
            userDbMocker.Setup(x => x.FindRegisteredUserData(registeredUserName, registeredUserPassword))
                .Returns(new object[0]);

            slave = new SignInSlave(guestUser, userDbMocker.Object);
        }

        [TestMethod]
        public void LoggedUserIsRegisteredTest()
        {
            User user = slave.SignIn(registeredUserName,registeredUserPassword);
            Assert.IsFalse(user.HasStorePolicies());
            Assert.IsFalse(user.IsSystemAdmin());
            Assert.IsTrue(user.IsRegisteredUser());
        }

        [TestMethod]
        public void LoggedUserDataTest()
        {
            User user = slave.SignIn(registeredUserName, registeredUserPassword);
            object[] expected = { registeredUserID, registeredUserName, registeredUserAddress, encryptedUserPassword,
                registeredUserCreditCard };
            object[] actual = user.ToData();
            CompareUserToData(expected, actual);
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            slave = new SignInSlave(null, userDbMocker.Object);
            User user = slave.SignIn(registeredUserName, registeredUserPassword);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void DidntSignUpTest()
        {
            User user = slave.SignIn("MaorLogin11", registeredUserPassword);
            BadSignUpToDataCompare(guestUser);
        }

        [TestMethod]
        public void MistakeUserName()
        {
            User user = slave.SignIn("MaorLogit", registeredUserPassword);
            BadSignUpToDataCompare(guestUser);
        }

        [TestMethod]
        public void LoggedUserCartIsEmptyTest()
        {
            User user = slave.SignIn(registeredUserName, registeredUserPassword);
            Assert.AreEqual(0, user.Cart.GetCartStorage().Length);
        }

        [TestMethod]
        public void SignInAgainTest()
        {
            User user = slave.SignIn("MaorLogit", registeredUserPassword);
            slave = new SignInSlave(user, userDbMocker.Object);
            user = slave.SignIn("some other name", registeredUserPassword);
            object[] expected = { registeredUserID, registeredUserName, registeredUserAddress, encryptedUserPassword,
                registeredUserCreditCard };
            object[] actual = user.ToData();
            CompareUserToData(expected, actual);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }

        private void CompareUserToData(object[] expected, object[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        private void BadSignUpToDataCompare(User user)
        {
            Assert.IsFalse(user.IsRegisteredUser());
            object[] expected = guestUser.ToData();
            object[] actual = user.ToData();
            CompareUserToData(expected, actual);
        }
    }
}
