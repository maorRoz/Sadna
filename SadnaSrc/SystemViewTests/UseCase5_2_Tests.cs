using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;
using SadnaSrc.AdminView;

namespace SystemViewTests
{
    [TestClass]
    public class UseCase5_2_Tests
    {
        private SystemAdminService adminServiceSession;
        private UserService userServiceSession;
        private MarketYard marketSession;


        private int toRemoveUserIdSoleOwner= 2;
        private int toRemoveUserIdNotSoleOwner = 3;
        private int adminID = 1;
        private string adminName = "Arik1";
        private string adminPass = "123";
        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = new MarketYard();
            userServiceSession = (UserService)marketSession.GetUserService();
        }

        [TestMethod]
        public void RemoveUserTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(toRemoveUserIdSoleOwner).Status);
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void RemoveUserNotSoleOwnerTest()
        {
            //TODO: should check if store has been closed by this operation when StoreCenter Module will be ready
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(toRemoveUserIdSoleOwner).Status);

        }

        [TestMethod]
        public void RemoveUserSoleOwnerTest()
        {
            //TODO: should check if store has been closed by this operation when StoreCenter Module will be ready
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(toRemoveUserIdNotSoleOwner).Status);
        }

        [TestMethod]
        public void DidntEnterTest()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int) RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserIdSoleOwner).Status);
        }

        [TestMethod]
        public void DidntLoggedTest()
        {
            userServiceSession.EnterSystem();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserIdSoleOwner).Status);
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSession.SignIn("Arik2", "123").Status);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NotSystemAdmin, adminServiceSession.RemoveUser(toRemoveUserIdSoleOwner).Status);
        }

        [TestMethod]
        public void SelfTerminationBlockedTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.SelfTermination, adminServiceSession.RemoveUser(adminID).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void NoUserToRemoveFoundTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.NoUserFound, adminServiceSession.RemoveUser(53453).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            marketSession.Exit();
        }

        private void DoSignInToAdmin()
        {
            Assert.AreEqual((int)EnterSystemStatus.Success, userServiceSession.EnterSystem().Status);
            Assert.AreEqual((int)SignInStatus.Success, userServiceSession.SignIn(adminName, adminPass).Status);
        }
    }
}
