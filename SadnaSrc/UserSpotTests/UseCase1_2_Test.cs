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
    public class UseCase1_2_Test
    {
        private UserService userServiceSession;
        private UserService userServiceSession2;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = new MarketYard();
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = null;
        }

        [TestMethod]
        public void GoodRegisterTest()
        {
            DoSignUp("MaorRegister1", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void RegisteredUserDataTest1()
        {
            RegisteredUserDataTest("MaorRegister2", "Here 3", "123");
        }

        [TestMethod]
        public void RegisteredUserDataTest2()
        {
            RegisteredUserDataTest("MaorRegister3", "Here 3", "maor33maormaor333");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest1()
        {
            MissingCredentialsSignUpTest("MaorRegister4", "Here 3", "");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest2()
        {
            MissingCredentialsSignUpTest("", "", "");
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest3()
        {
            MissingCredentialsSignUpTest("MaorRegister5", "Here 3", null);
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest4()
        {
            MissingCredentialsSignUpTest(null, null, null);
        }

        [TestMethod]
        public void MissingCredentialsSignUpTest5()
        {
            MissingCredentialsSignUpTest(null, "Here 3", "");
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int) SignUpStatus.DidntEnterSystem, userServiceSession.SignUp("MaorRegister6", "Here 3", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void RegisteredUserCartisEmptyTest()
        {
            DoSignUp("MaorRegister7", "Here 3", "123");
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.GetUser();
            Assert.AreEqual(0,registeredUser.GetCart().Length);
        }

        [TestMethod]
        public void RegisteredUserPoliciesTest()
        {
            userServiceSession.EnterSystem();
            User user = userServiceSession.GetUser();
            Assert.AreEqual(0, user.GetStatePolicies().Length);
            Assert.AreEqual(0, user.GetStoreManagerPolicies().Length);
            Assert.AreEqual((int) SignUpStatus.Success, userServiceSession.SignUp("MaorRegister8", "Here 3", "123").Status);
            user = userServiceSession.GetUser();
            StatePolicy[] expectedStates = user.GetStatePolicies();
            Assert.AreEqual(1, expectedStates.Length);
            Assert.AreEqual(expectedStates[0].GetState(),StatePolicy.State.RegisteredUser);
            Assert.AreEqual(0, user.GetStoreManagerPolicies().Length);
        }

        [TestMethod]
        public void SignUpAgainTest()
        {
            DoSignUp("MaorRegister9", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignUpStatus.SignedUpAlready, userServiceSession.SignUp("MaorRegister9", "Here 3", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void SignUpWithExistedName()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignUp("MaorRegister10", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession2.EnterSystem();
            Assert.IsFalse(MarketException.hasErrorRaised());
            Assert.AreEqual((int)SignUpStatus.TakenName, userServiceSession2.SignUp("MaorRegister10", "Here 3", "123").Status);
            Assert.IsTrue(MarketException.hasErrorRaised());

        }


        [TestMethod]
        public void PromoteToAdminTest()
        {
            DoSignUp("MaorRegister11", "Here 3", "123");
            RegisteredUser adminUser = (RegisteredUser)userServiceSession.GetUser();
            object[] expectedData = { adminUser.SystemID, "MaorRegister11", "Here 3", UserService.GetSecuredPassword("123") };
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.AreEqual(1, adminUser.GetStatePolicies().Length);
            Assert.AreEqual(0, adminUser.GetStoreManagerPolicies().Length);
            adminUser.PromoteToAdmin();
            Assert.AreEqual(0, adminUser.GetCart().Length);
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.AreEqual(0, adminUser.GetStoreManagerPolicies().Length);
            StatePolicy[] expectedStatePolicies = adminUser.GetStatePolicies();
            Assert.AreEqual(2, expectedStatePolicies.Length);
            Assert.AreEqual(expectedStatePolicies[0].GetState(), StatePolicy.State.RegisteredUser);
            Assert.AreEqual(expectedStatePolicies[1].GetState(), StatePolicy.State.SystemAdmin);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSession2?.CleanSession();
            userServiceSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            marketSession.Exit();
        }

        private void DoSignUp(string name, string address, string password)
        {
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceSession.SignUp(name, address, password).Status);
        }

        private void MissingCredentialsSignUpTest(string name, string address, string password)
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, userServiceSession.SignUp(name, address, password).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }
        private void RegisteredUserDataTest(string name, string address, string password)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.GetUser();
            object[] expectedData = { registeredUser.SystemID, name, address, UserService.GetSecuredPassword(password) };
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
        }
    }
}
