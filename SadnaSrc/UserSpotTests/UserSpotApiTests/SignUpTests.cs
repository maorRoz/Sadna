using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class SignUpTests
    {
        private SignUpSlave slave;
        private int registeredUserID = 5000;
        private User guestUser;
        private string registeredUserName = "MaorRegister";
        private string registeredUserAddress = "Here 3";
        private string registeredUserPassword = "123";
        private string encryptedUserPassword = UserSecurityService.GetSecuredPassword("123");
        private string registeredUserCreditCard = "12345678";
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            userDbMocker.Setup(x => x.RegisterUser(It.IsAny<int>(),It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CartItem[]>()))
                .Returns(new RegisteredUser(userDbMocker.Object,registeredUserID, registeredUserName,registeredUserAddress,
                    encryptedUserPassword, registeredUserCreditCard,new CartItem[0],
                    new []{new StatePolicy(StatePolicy.State.RegisteredUser)}, new StoreManagerPolicy[0]));
            userDbMocker.Setup(x => x.IsUserNameExist(It.IsAny<string>())).Returns(false);
            guestUser = new User(userDbMocker.Object, registeredUserID);
            slave = new SignUpSlave(guestUser, userDbMocker.Object);
        }

        [TestMethod]
        public void RegisteredUserIsRegisteredTest()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, registeredUserCreditCard);
            Assert.IsFalse(user.IsSystemAdmin());
            Assert.IsTrue(user.IsRegisteredUser());

        }

        [TestMethod]
        public void RegisteredUserDataTest()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, registeredUserCreditCard);
            object[] expected = { registeredUserID, registeredUserName, registeredUserAddress, encryptedUserPassword,
                registeredUserCreditCard };
            object[] actual = user.ToData();
            CompareUserToData(expected, actual);
        }

        [TestMethod]
        public void MissingCredentialsTest1()
        {
            User user = slave.SignUp("", registeredUserAddress, registeredUserPassword, registeredUserCreditCard);
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void MissingCredentialsTest2()
        {
            User user = slave.SignUp("", null, registeredUserPassword, registeredUserCreditCard);
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void MissingCredentialsTest3()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, null, registeredUserCreditCard);
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void MissingCredentialsTest4()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, "", null);
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void CreditCardIsTooShort()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, "123456");
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void CreditCardIsTooLong()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, "123456789");
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void CreditCardHasInvalidChars()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, "12#$%^78");
            BadSignUpToDataCompare(user);
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            slave = new SignUpSlave(null,userDbMocker.Object);
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword,
                registeredUserCreditCard);
            Assert.IsNull(user);

        }

        [TestMethod]
        public void RegisteredUserCartIsEmptyTest()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, registeredUserCreditCard);
            Assert.AreEqual(0, user.Cart.GetCartStorage().Length);
        }

        [TestMethod]
        public void SignUpAgainTest()
        {
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword, registeredUserCreditCard);
            slave = new SignUpSlave(user,userDbMocker.Object);
            user = slave.SignUp("some other name", registeredUserAddress, registeredUserPassword,
                registeredUserCreditCard);
            object[] expected = { registeredUserID, registeredUserName, registeredUserAddress, encryptedUserPassword,
                registeredUserCreditCard };
            object[] actual = user.ToData();
            CompareUserToData(expected,actual);
        }


        [TestMethod]
        public void SignUpWithExistedName()
        {
            userDbMocker.Setup(x => x.IsUserNameExist(It.IsAny<string>())).Returns(true);
            slave = new SignUpSlave(guestUser, userDbMocker.Object);
            User user = slave.SignUp(registeredUserName, registeredUserAddress, registeredUserPassword,
                registeredUserCreditCard);
            BadSignUpToDataCompare(user);
        }



        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }

        public void CompareUserToData(object[] expected,object[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        public void BadSignUpToDataCompare(User user)
        {
            Assert.IsFalse(user.IsRegisteredUser());
            object[] expected = guestUser.ToData();
            object[] actual = user.ToData();
            CompareUserToData(expected, actual);
        }
    }
}
