using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;


namespace UserSpotTests.UseCaseUnitTest
{

    [TestClass]
    public class UseCase2_1_Test
    {
        private UserService userServiceSignInSession;
        private UserService userServiceSignUpSession;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSignInSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession = null;
        }

        [TestMethod]
        public void GoodLoginTest1()
        {
            DoSignUpSignIn("MaorLogin1", "Here 4", "123", "12345678");
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void GoodLoginDataTest()
        {
            DoSignUpSignIn("MaorLogin2", "Here 4", "123", "12345678");
            object[] loggedUserData = ((RegisteredUser) userServiceSignInSession.MarketUser).ToData();
            object[] registeredUserData = ((RegisteredUser)userServiceSignUpSession.MarketUser).ToData();
            object[] expectedData = { loggedUserData[0], "MaorLogin2", "Here 4", UserSecurityService.GetSecuredPassword("123"),"12345678" };
            Assert.IsTrue(loggedUserData.SequenceEqual(registeredUserData));
            Assert.IsTrue(loggedUserData.SequenceEqual(expectedData));
        }

        [TestMethod]
        public void MissingCredentialsSignInTest1()
        {
            MissingCredentialsSignInTest("MaorLogin3", "Here 4", "123", "12345678", "MaorLogin", "");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest2()
        {
            MissingCredentialsSignInTest("MaorLogin4", "Here 4", "123", "12345678", "", "");
        }


        [TestMethod]
        public void MissingCredentialsSignInTest3()
        {
            MissingCredentialsSignInTest("MaorLogin5", "Here 4", "123", "12345678", null, "123");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest4()
        {
            MissingCredentialsSignInTest("MaorLogin6", "Here 4", "123", "12345678", null, "");
        }

        [TestMethod]
        public void BadCredentialsSignInTest1()
        {
            BadCredentialsSignInTest("MaorLogin8", "Here 4", "123", "12345678", "MaorLogin8", "124");
        }

        [TestMethod]
        public void BadCredentialsSignInTest2()
        {
            BadCredentialsSignInTest("MaorLogin9", "Here 4", "123", "12345678", "MaorLogin99", "123");
        }

        [TestMethod]
        public void BadCredentialsSignInTest3()
        {
            BadCredentialsSignInTest("MaorLogin10", "Here 4", "123", "12345678", "MaorLogin100", "124");
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.DidntEnterSystem, userServiceSignInSession.SignIn("MaorLogin11", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void DidntSignUpTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("MaorLogin12", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void SignedInUserCartisEmptyTest()
        {
            DoSignUpSignIn("MaorLogin13", "Here 3", "123", "12345678");
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.MarketUser;
            Assert.AreEqual(0, user.Cart.GetCartStorage().Length);
        }

        [TestMethod]
        public void SignedInUserPoliciesTest()
        {
            DoSignUp("MaorLogin14", "Here 3", "123","12345678");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int) SignInStatus.Success, userServiceSignInSession.SignIn("MaorLogin14", "123").Status);
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.MarketUser;
            Assert.IsFalse(user.HasStorePolicies());
            Assert.IsTrue(user.IsRegisteredUser());
            Assert.IsFalse(user.IsSystemAdmin());
        }

        [TestMethod]
        public void SignInAgainTest()
        {
            DoSignUp("MaorLogin15", "Here 3", "123", "12345678");
            DoSignIn("MaorLogin15", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int) SignInStatus.SignedInAlready, userServiceSignInSession.SignIn("MaorLogin15", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void SignInAfterSignUpTest()
        {
            DoSignUp("MaorLogin16", "Here 3", "123", "12345678");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.SignedInAlready, userServiceSignUpSession.SignIn("MaorLogin16", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void PromoteToAdminTest()
        {
            DoSignUpSignIn("MaorLogin17", "Here 3", "123", "12345678");
            RegisteredUser adminUser = (RegisteredUser)userServiceSignInSession.MarketUser;
            object[] expectedData = { adminUser.SystemID, "MaorLogin17", "Here 3", UserSecurityService.GetSecuredPassword("123"),"12345678" };
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.IsTrue(adminUser.IsRegisteredUser());
            Assert.IsFalse(adminUser.IsSystemAdmin());
            Assert.IsFalse(adminUser.HasStorePolicies());
            adminUser.PromoteToAdmin();
            Assert.AreEqual(0, adminUser.Cart.GetCartStorage().Length);
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.IsTrue(adminUser.IsRegisteredUser());
            Assert.IsTrue(adminUser.IsSystemAdmin());
            Assert.IsFalse(adminUser.HasStorePolicies());
        }

        [TestMethod]
        public void GettingErrorTipTest1()
        {
            DoSignUp("MaorLogin18", "Here 3","123", "12345678");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.MistakeTipGiven, userServiceSignInSession.SignIn("MkorLogin18", "123").Status);
        }

        [TestMethod]
        public void GettingErrorTipTest2()
        {
            DoSignUp("MaorLogin19", "Here 3", "123", "12345678");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.MistakeTipGiven, userServiceSignInSession.SignIn("MktrLogin19", "123").Status);
        }
        [TestMethod]
        public void NotGettingErrorTipTest1()
        {
            DoSignUp("MaorLogin20", "Here 3", "123", "12345678");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("MktfLogin20", "123").Status);
        }

        [TestMethod]
        public void NotGettingErrorTipTest2()
        {
            DoSignUp("MaorLogin21", "Here 3", "123", "12345678");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("Mkor_Login21", "123").Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSignUpSession?.CleanSession();
            userServiceSignInSession.CleanSession();
            MarketYard.CleanSession();
        }

        private void DoSignUpSignIn(string name, string address, string password,string creditCard)
        {
            DoSignUp(name, address, password,creditCard);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(name, password);
        }
        private void DoSignIn(string name, string password)
        {
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSignInSession.SignIn(name, password).Status);
        }
        private void DoSignUp(string name,string address , string password, string creditCard)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            Assert.AreEqual((int) SignUpStatus.Success, userServiceSignUpSession.SignUp(name, address, password, creditCard).Status);

        }
        private void MissingCredentialsSignInTest(string name, string address,
            string password, string creditCard, string loginName,string loginPassword)
        {
            DoSignUp(name,address,password, creditCard);
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int) SignInStatus.NullEmptyDataGiven ,userServiceSignInSession.SignIn(loginName, loginPassword).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        private void BadCredentialsSignInTest(string name, string address,
            string password, string creditCard, string loginName, string loginPassword)
        {
            DoSignUp(name, address, password,creditCard);
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn(loginName, loginPassword).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }
    }
}
