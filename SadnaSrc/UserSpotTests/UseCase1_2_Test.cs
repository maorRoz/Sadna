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

        [TestInitialize]
        public void MarketBuilder()
        {
            var marketSession = new MarketYard();
            userServiceSession = (UserService) marketSession.GetUserService();
        }

        [TestMethod]
        public void RegisteredUserDataTest1()
        {
            RegisteredUserDataTest("Maor", "Here 3", "123");
        }

        [TestMethod]
        public void RegisteredUserDataTest2()
        {
            RegisteredUserDataTest("Maor33", "Here 3", "maor33maormaor333");
        }

        [TestMethod]
        public void RegisteredUserDataTest3()
        {
            RegisteredUserDataTest("Maor", "", "");
        }

        [TestMethod]
        public void RegisteredUserDataTest4()
        {
            RegisteredUserDataTest("Maor", "Here 3", "");
        }

        [TestMethod]
        public void MissingCredentialsTest1()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            doSignUp("Maor", "Here 3", null);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void MissingCredentialsTest2()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            doSignUp(null, null, null);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSession.SignUp("Maor", "Here 3", "123");
            Assert.IsTrue(MarketException.hasErrorRaised());

        }

        [TestMethod]
        public void RegisteredUserCartisEmptyTest()
        {
            doSignUp("Maor", "Here 3", "123");
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.GetUser();
            Assert.AreEqual(0,registeredUser.GetCart().Length);
        }

        [TestMethod]
        public void RegisteredUserPoliciesTest()
        {
            userServiceSession.EnterSystem();
            User user = userServiceSession.GetUser();
            Assert.AreEqual(0, user.GetPolicies().Length);
            userServiceSession.SignUp("Maor", "Here 3", "123");
            user = userServiceSession.GetUser();
            UserPolicy[] expectedPolicies = user.GetPolicies();
            Assert.AreEqual(1, expectedPolicies.Length);
            if (expectedPolicies.Length > 0)
            {
                Assert.AreEqual(expectedPolicies[0].GetState(),UserPolicy.State.RegisteredUser);
            }
        }

        [TestMethod]
        public void SignUpAgainTest()
        {
            doSignUp("Maor", "Here 3", "123");
            Assert.IsFalse(MarketException.hasErrorRaised());
            userServiceSession.SignUp("Maor", "Here 3", "123");
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void SignUpWithExistedName()
        {
            Assert.IsFalse(MarketException.hasErrorRaised());
            doSignUp("UseCase1.2Test", "", "");
            Assert.IsTrue(MarketException.hasErrorRaised());
        }


        [TestMethod]
        public void PromoteToAdminTest()
        {
            doSignUp("Maor", "Here 3", "123");
            RegisteredUser adminUser = (RegisteredUser)userServiceSession.GetUser();
            object[] expectedData = { adminUser.SystemID, "Maor", "Here 3", userServiceSession.GetSecuredPassword("123") };
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            Assert.AreEqual(1, adminUser.GetPolicies().Length);
            adminUser.PromoteToAdmin();
            Assert.AreEqual(0, adminUser.GetCart().Length);
            Assert.IsTrue(expectedData.SequenceEqual(adminUser.ToData()));
            UserPolicy[] expectedPolicies = adminUser.GetPolicies();
            Assert.AreEqual(2, expectedPolicies.Length);
            if (expectedPolicies.Length == 2)
            {
                Assert.AreEqual(expectedPolicies[0].GetState(), UserPolicy.State.RegisteredUser);
                Assert.AreEqual(expectedPolicies[1].GetState(), UserPolicy.State.SystemAdmin);
            }

        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
        }

        private void doSignUp(string name, string address, string password)
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignUp(name, address, password);
        }
        private void RegisteredUserDataTest(string name, string address, string password)
        {
            doSignUp(name, address, password);
            RegisteredUser registeredUser = (RegisteredUser)userServiceSession.GetUser();
            object[] expectedData = { registeredUser.SystemID, name, address, userServiceSession.GetSecuredPassword(password) };
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
        }
    }
}
