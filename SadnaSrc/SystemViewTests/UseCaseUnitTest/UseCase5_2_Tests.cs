using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.AdminView;

namespace SystemViewTests.UseCaseUnitTest
{
    [TestClass]
    public class UseCase5_2_Tests
    {
        private SystemAdminService adminServiceSession;
        private IUserService userServiceSession;
        private MarketYard marketSession;


        private string toRemoveUserNameSoleOwner= "Arik2";
        private string toRemoveUserNameNotSoleOwner = "Arik3";
        private string adminName = "Arik1";
        private string noUserName = "sdasdfsgdhgfdhfdfdgdfgdf";
        private string adminPass = "123";
        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
        }

        [TestMethod]
        public void RemoveUserSoleOwnerTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(toRemoveUserNameSoleOwner).Status);

        }

        [TestMethod]
        public void RemoveUserNotSoleOwnerTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(toRemoveUserNameNotSoleOwner).Status);
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int) RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserNameSoleOwner).Status);
        }

        [TestMethod]
        public void DidntLoggedTest()
        {
            userServiceSession.EnterSystem();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserNameSoleOwner).Status);
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Arik2", "123");
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserNameSoleOwner).Status);
        }

        [TestMethod]
        public void SelfTerminationBlockedTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.SelfTermination, adminServiceSession.RemoveUser(adminName).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void NoUserToRemoveFoundTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NoUserFound, adminServiceSession.RemoveUser(noUserName).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            userServiceSession.CleanSession();
            MarketYard.CleanSession();
        }

        private void DoSignInToAdmin()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(adminName, adminPass);
        }
    }
}
