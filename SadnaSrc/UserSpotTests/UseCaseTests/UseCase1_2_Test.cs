using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UseCaseUnitTest
{

    [TestClass]
    public class UseCase1_2_Test
    {
        private UserService userServiceSession;
        private UserService userServiceSession2;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = null;
        }

        [TestMethod]
        public void GoodRegisterTest()
        {
            DoSignUp("MaorRegister1", "Here 3", "123", "12345678");
            Assert.IsFalse(MarketException.HasErrorRaised());
        }

        [TestMethod]
        public void BadRegisterTest()
        {
            Assert.AreEqual((int)SignUpStatus.BadInput, userServiceSession.SignUp("Ma'orRegister1", "Here 3", "123", "12345678").Status);
            Assert.IsTrue(MarketException.HasErrorRaised());
        }

        [TestMethod]
        public void RegisteredUserDataTest1()
        {
            RegisteredUserDataTest("MaorRegister2", "Here 3", "123", "12345678");
        }

        [TestMethod]
        public void RegisteredUserDataTest2()
        {
            RegisteredUserDataTest("MaorRegister3", "Here 3", "maor33maormaor333", "12345678");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest1()
        {
            MissingCredentialsSignUpTest("MaorRegister4", "Here 3", "", "12345678");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest2()
        {
            MissingCredentialsSignUpTest("", "", "","");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest3()
        {
            MissingCredentialsSignUpTest("MaorRegister5", "Here 3","123", null);
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest4()
        {
            MissingCredentialsSignUpTest("MaorRegister15", "Here 3", "123", null);
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest5()
        {
            MissingCredentialsSignUpTest("MaorRegister16", "Here 3", "123", "");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest6()
        {
            MissingCredentialsSignUpTest("MaorRegister17", "Here 3", null, "12345678");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest7()
        {
            MissingCredentialsSignUpTest(null, "Here 3","", "12345678");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest8()
        {
            MissingCredentialsSignUpTest("MaorRegister18", null, "123", "12345678");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest9()
        {
            MissingCredentialsSignUpTest(null, null, null,null);
        }

        [TestMethod]
        public void CreditCardIsTooShort()
        {
            MissingCredentialsSignUpTest("MaorRegister19", "no-where", "123", "123456");
        }

        [TestMethod]
        public void CreditCardIsTooLong()
        {
            MissingCredentialsSignUpTest("MaorRegister20", "no-where", "123", "123456789");
        }

        [TestMethod]
        public void CreditCardHasInvalidChars()
        {
            MissingCredentialsSignUpTest("MaorRegister21", "no-where", "123", "12#$%^78");
        }


        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.HasErrorRaised());
            Assert.AreEqual((int) SignUpStatus.DidntEnterSystem, userServiceSession.SignUp("MaorRegister6", "Here 3", "123", "12345678").Status);
            Assert.IsTrue(MarketException.HasErrorRaised());

        }

        [TestMethod]
        public void RegisteredUserCartIsEmptyTest()
        {
            DoSignUp("MaorRegister7", "Here 3", "123", "12345678");
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.MarketUser;
            Assert.AreEqual(0, registeredUser.Cart.GetCartStorage().Length);
        }

        [TestMethod]
        public void RegisteredUserPoliciesTest()
        {
            userServiceSession.EnterSystem();
            User user = userServiceSession.MarketUser;
            Assert.IsFalse(user.IsRegisteredUser());
            Assert.AreEqual((int) SignUpStatus.Success, userServiceSession.SignUp("MaorRegister8", "Here 3", "123", "12345678").Status);
            user = userServiceSession.MarketUser;
            Assert.IsFalse(user.IsSystemAdmin());
            Assert.IsTrue(user.IsRegisteredUser());
        }

        [TestMethod]
        public void SignUpAgainTest()
        {
            DoSignUp("MaorRegister9", "Here 3", "123", "12345678");
            Assert.IsFalse(MarketException.HasErrorRaised());
            Assert.AreEqual((int)SignUpStatus.SignedUpAlready, userServiceSession.SignUp("MaorRegister9", "Here 3", "123", "12345678").Status);
            Assert.IsTrue(MarketException.HasErrorRaised());
        }

        [TestMethod]
        public void SignUpWithExistedName()
        {
            Assert.IsFalse(MarketException.HasErrorRaised());
            DoSignUp("MaorRegister10", "Here 3", "123", "12345678");
            Assert.IsFalse(MarketException.HasErrorRaised());
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession2.EnterSystem();
            Assert.IsFalse(MarketException.HasErrorRaised());
            Assert.AreEqual((int)SignUpStatus.TakenName, userServiceSession2.SignUp("MaorRegister10", "Here 3", "123", "12345678").Status);
            Assert.IsTrue(MarketException.HasErrorRaised());

        }


        [TestMethod]
        public void PromoteToAdminTest()
        {
            DoSignUp("MaorRegister11", "Here 3", "123", "12345678");
            RegisteredUser adminUser = (RegisteredUser)userServiceSession.MarketUser;
            object[] expectedData = { adminUser.SystemID, "MaorRegister11", "Here 3", UserSecurityService.GetSecuredPassword("123"),"12345678" };
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.IsTrue(adminUser.IsRegisteredUser());
            Assert.IsFalse(adminUser.IsSystemAdmin());
            Assert.IsFalse(adminUser.HasStorePolicies());
            adminUser.PromoteToAdmin();
            Assert.AreEqual(0, adminUser.Cart.GetCartStorage().Length);
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.IsFalse(adminUser.HasStorePolicies());
            Assert.IsTrue(adminUser.IsRegisteredUser());
            Assert.IsTrue(adminUser.IsSystemAdmin());
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void DoSignUp(string name, string address, string password,string creditCard)
        {
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceSession.SignUp(name, address, password, creditCard).Status);
        }

        private void MissingCredentialsSignUpTest(string name, string address, string password, string creditCard)
        {
            Assert.IsFalse(MarketException.HasErrorRaised());
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, userServiceSession.SignUp(name, address, password, creditCard).Status);
            Assert.IsTrue(MarketException.HasErrorRaised());
        }
        private void RegisteredUserDataTest(string name, string address, string password, string creditCard)
        {
            DoSignUp(name, address, password, creditCard);
            Assert.IsFalse(MarketException.HasErrorRaised());
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.MarketUser;
            object[] expectedData = { registeredUser.SystemID, name, address, UserSecurityService.GetSecuredPassword(password),creditCard };
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
        }
    }
}
