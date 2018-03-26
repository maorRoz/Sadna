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
            doSignUp("Maor", "Here 3", null);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void MissingCredentialsTest2()
        {
            doSignUp(null, null, null);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void DidntEnteredSystemTest()
        {
            userServiceSession.SignUp("Maor", "Here 3", "123");
            Assert.IsTrue(MarketException.hasErrorRaised());

        }


        [TestMethod]
        public void PromoteToAdminTest()
        {
            //  registeredUser.PromoteToAdmin();
            //  Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
            //    registeredUser.PromoteToAdmin();
            //   Assert.AreEqual(2, registeredUser.GetPolicies().Length);
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
