using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;
using SadnaSrc.MarketHarmony;
using SadnaSrc.AdminView;

namespace IntegrationTests
{
    [TestClass]
    public class UserAdmin_Test
    {

        private UserService userServiceSession;
        private UserService deletedUserSession;
        private SystemAdminService adminServiceSession;
        private UserAdminHarmony userAdminHarmony;
        private MarketYard marketSession;
        private string adminName = "Arik1";
        private string adminPass = "123";
        private string notAdminName = "Arik2";
        private string notAdminPass = "123";
        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userAdminHarmony = null;
            adminServiceSession = null;
        }

        [TestMethod]
        public void IsSystemAdminTest1()
        {
            ToSignIn(adminName, adminPass);
            Assert.IsTrue(userAdminHarmony.IsSystemAdmin());
        }

        [TestMethod]
        public void IsSystemAdminTest2()
        {
            userServiceSession.EnterSystem();
            userAdminHarmony = new UserAdminHarmony(userServiceSession);
            Assert.IsFalse(userAdminHarmony.IsSystemAdmin());
        }

        [TestMethod]
        public void IsSystemAdminTest3()
        {
            userAdminHarmony = new UserAdminHarmony(userServiceSession);
            Assert.IsFalse(userAdminHarmony.IsSystemAdmin());
        }

        [TestMethod]
        public void IsSystemAdminTest4()
        {
            ToSignIn(notAdminName, notAdminPass);
            Assert.IsFalse(userAdminHarmony.IsSystemAdmin());
        }

        [TestMethod]
        public void GetAdminSystemIdTest()
        {
            ToSignIn(adminName, adminPass);
            Assert.AreEqual(1,userAdminHarmony.GetAdminSystemID());
        }

        [TestMethod]
        public void GetAdminNameTest()
        {
            ToSignIn(adminName, adminPass);
            Assert.AreEqual(adminName, userAdminHarmony.GetAdminSystemID());
        }

        [TestMethod]
        public void CantLoginToDeletedUserTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.RemoveUser(notAdminName);
            deletedUserSession = (UserService)marketSession.GetUserService();
            deletedUserSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound ,deletedUserSession.SignIn(notAdminName, notAdminPass).Status);


        }

        [TestMethod]
        public void SignUpWithDeletedUserDataTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.RemoveUser(notAdminName);
            deletedUserSession = (UserService)marketSession.GetUserService();
            deletedUserSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, deletedUserSession.SignUp(notAdminName,"no-where" ,notAdminPass).Status);
        }

        [TestMethod]
        public void StoreClosedUponRemovalTest()
        {
            //TODO: add test. cant check this better then in the unit test at this time
        }

        [TestMethod]

        public void UserReportTest()
        {
            //TODO: add test. cant check this better then in the unit test at this time
        }

        [TestMethod]
        public void StoreReportTest()
        {
            //TODO: add test. cant check this better then in the unit test at this time
        }




        [TestCleanup]
        public void UserAdminTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            deletedUserSession.CleanSession();
            MarketYard.CleanSession();
        }

        private void ToSignIn(string name, string password)
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(name, password);
            userAdminHarmony = new UserAdminHarmony(userServiceSession);
        }
    }
}
