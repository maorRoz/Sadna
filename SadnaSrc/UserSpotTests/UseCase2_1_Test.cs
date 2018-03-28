using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;


namespace UserSpotTests
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
            marketSession = new MarketYard();
            userServiceSignInSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession = null;
        }

        [TestMethod]
        public void GoodLoginTest1()
        {
            DoSignUpSignIn("MaorLogin", "Here 4", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void GoodLoginDataTest()
        {
            DoSignUpSignIn("MaorLogin", "Here 4", "123");
            object[] loggedUserData = ((RegisteredUser) userServiceSignInSession.GetUser()).ToData();
            object[] registeredUserData = ((RegisteredUser)userServiceSignUpSession.GetUser()).ToData();
            object[] expectedData = { loggedUserData[0], "MaorLogin", "Here 4", UserService.GetSecuredPassword("123") };
            Assert.IsTrue(loggedUserData.SequenceEqual(registeredUserData));
            Assert.IsTrue(loggedUserData.SequenceEqual(expectedData));
        }

        [TestMethod]
        public void MissingCredentialsSignInTest1()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogin", "");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest2()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "", "");
        }


        [TestMethod]
        public void MissingCredentialsSignInTest3()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", null, "123");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest4()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", null, "");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest5()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "", null);
        }

        [TestMethod]
        public void BadCredentialsSignInTest1()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogin", "124");
        }

        [TestMethod]
        public void BadCredentialsSignInTest2()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogi1n", "123");
        }

        [TestMethod]
        public void BadCredentialsSignInTest3()
        {
            MissingBadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogi1n", "124");
        }
        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.SignIn("Maor","123");
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void DidntSignUpTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.SignIn("Maor", "123");
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void SignedInUserCartisEmptyTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.GetUser();
            Assert.AreEqual(0, user.GetCart().Length);
        }

        [TestMethod]
        public void SignedInUserPoliciesTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            userServiceSignInSession.EnterSystem();
            Assert.Equals(userServiceSignUpSession.SignIn("Maor", "123").Status, SignInStatus.Success);
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.GetUser();
            UserPolicy[] expectedPolicies = user.GetPolicies();
            Assert.AreEqual(1, expectedPolicies.Length);
            Assert.AreEqual(expectedPolicies[0].GetState(), UserPolicy.State.RegisteredUser);
        }

        [TestMethod]
        public void SignInAgainTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn("Maor","123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.SignIn("Maor", "123");
        }

        [TestMethod]
        public void SignInAfterSignUpTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.Equals(userServiceSignUpSession.SignIn("Maor", "123").Status, SignInStatus.SignedInAlready);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void PromoteToAdminTest()
        {
            DoSignUpSignIn("Maor", "Here 3", "123");
            RegisteredUser adminUser = (RegisteredUser)userServiceSignInSession.GetUser();
            object[] expectedData = { adminUser.SystemID, "Maor", "Here 3", UserService.GetSecuredPassword("123") };
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.AreEqual(1, adminUser.GetPolicies().Length);
            adminUser.PromoteToAdmin();
            Assert.AreEqual(0, adminUser.GetCart().Length);
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            UserPolicy[] expectedPolicies = adminUser.GetPolicies();
            Assert.AreEqual(2, expectedPolicies.Length);
            Assert.AreEqual(expectedPolicies[0].GetState(), UserPolicy.State.RegisteredUser);
            Assert.AreEqual(expectedPolicies[1].GetState(), UserPolicy.State.SystemAdmin);
        }


        private void DoSignUpSignIn(string name, string address, string password)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(name, password);
        }
        private void DoSignIn(string name, string password)
        {
            userServiceSignInSession.EnterSystem();
            userServiceSignInSession.SignIn(name, password);
        }
        private void DoSignUp(string name,string address , string password)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            userServiceSignUpSession.SignUp(name, address, password);
            userServiceSignInSession.ReConnect();

        }
        private void MissingBadCredentialsSignInTest(string name, string address,
            string password, string loginName,string loginPassword)
        {
            DoSignUp(name,address,password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(loginName, loginPassword);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSignInSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            marketSession.Exit();
        }
    }
}
