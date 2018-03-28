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
            MissingCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogin", "");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest2()
        {
            MissingCredentialsSignInTest("MaorLogin", "Here 4", "123", "", "");
        }


        [TestMethod]
        public void MissingCredentialsSignInTest3()
        {
            MissingCredentialsSignInTest("MaorLogin", "Here 4", "123", null, "123");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest4()
        {
            MissingCredentialsSignInTest("MaorLogin", "Here 4", "123", null, "");
        }

        [TestMethod]
        public void MissingCredentialsSignInTest5()
        {
            MissingCredentialsSignInTest("MaorLogin", "Here 4", "123", "", null);
        }

        [TestMethod]
        public void BadCredentialsSignInTest1()
        {
            BadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogin", "124");
        }

        [TestMethod]
        public void BadCredentialsSignInTest2()
        {
            BadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogi1n", "123");
        }

        [TestMethod]
        public void BadCredentialsSignInTest3()
        {
            BadCredentialsSignInTest("MaorLogin", "Here 4", "123", "MaorLogi1n", "124");
        }
        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.DidntEnterSystem, userServiceSignInSession.SignIn("Maor","123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void DidntSignUpTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("Maor", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void SignedInUserCartisEmptyTest()
        {
            DoSignUpSignIn("Maor", "Here 3", "123");
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.GetUser();
            Assert.AreEqual(0, user.GetCart().Length);
        }

        [TestMethod]
        public void SignedInUserPoliciesTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int) SignInStatus.Success, userServiceSignInSession.SignIn("Maor", "123").Status);
            RegisteredUser user = (RegisteredUser)userServiceSignInSession.GetUser();
            UserPolicy[] expectedPolicies = user.GetPolicies();
            Assert.AreEqual(1, expectedPolicies.Length);
            Assert.AreEqual(expectedPolicies[0].GetState(), UserPolicy.State.RegisteredUser);
        }

        [TestMethod]
        public void SignInAgainTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            DoSignIn("Maor","123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int) SignInStatus.SignedInAlready, userServiceSignInSession.SignIn("Maor", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void SignInAfterSignUpTest()
        {
            DoSignUp("Maor", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignInStatus.SignedInAlready, userServiceSignUpSession.SignIn("Maor", "123").Status);
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

        [TestMethod]
        public void GettingErrorTipTest1()
        {
            DoSignUp("Maor","Here 3","123");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.MistakeTipGiven, userServiceSignInSession.SignIn("Mkor", "123").Status);
        }

        [TestMethod]
        public void GettingErrorTipTest2()
        {
            DoSignUp("Maor", "Here 3", "123");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.MistakeTipGiven, userServiceSignInSession.SignIn("Mktr", "123").Status);
        }
        [TestMethod]
        public void NotGettingErrorTipTest1()
        {
            DoSignUp("Maor", "Here 3", "123");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("Mktf", "123").Status);
        }

        [TestMethod]
        public void NotGettingErrorTipTest2()
        {
            DoSignUp("Maor", "Here 3", "123");
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn("Mkor_", "123").Status);
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
            Assert.AreEqual((int)SignInStatus.Success, userServiceSignInSession.SignIn(name, password).Status);
        }
        private void DoSignUp(string name,string address , string password)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            Assert.AreEqual((int) SignUpStatus.Success, userServiceSignUpSession.SignUp(name, address, password).Status);
            userServiceSignInSession.ReConnect();

        }
        private void MissingCredentialsSignInTest(string name, string address,
            string password, string loginName,string loginPassword)
        {
            DoSignUp(name,address,password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int) SignInStatus.NullEmptyDataGiven ,userServiceSignInSession.SignIn(loginName, loginPassword).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        private void BadCredentialsSignInTest(string name, string address,
            string password, string loginName, string loginPassword)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound, userServiceSignInSession.SignIn(loginName, loginPassword).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSignUpSession?.CleanSession();
            userServiceSignInSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            marketSession.Exit();
        }
    }
}
