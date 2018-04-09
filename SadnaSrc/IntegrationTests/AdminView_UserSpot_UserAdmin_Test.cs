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
    public class AdminView_UserSpot_UserAdmin_Test
    {

        private UserService userServiceSession;
        private UserService deletedUserSession;
        private UserService deletedUserSession2;
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
            deletedUserSession = null;
            deletedUserSession2 = null;
        }

        [TestMethod]
        public void IsSystemAdminTest1()
        {
            ToSignIn(adminName, adminPass);
            try
            {
                userAdminHarmony.ValidateSystemAdmin();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsSystemAdminTest2()
        {
            userServiceSession.EnterSystem();
            userAdminHarmony = new UserAdminHarmony(userServiceSession);
            try
            {
                userAdminHarmony.ValidateSystemAdmin();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void IsSystemAdminTest3()
        {
            userAdminHarmony = new UserAdminHarmony(userServiceSession);
            try
            {
                userAdminHarmony.ValidateSystemAdmin();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void IsSystemAdminTest4()
        {
            ToSignIn(notAdminName, notAdminPass);
            try
            {
                userAdminHarmony.ValidateSystemAdmin();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
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
            Assert.AreEqual(adminName, userAdminHarmony.GetAdminName());
        }

        [TestMethod]
        public void CantLoginToDeletedUserTest()
        {
            deletedUserSession = (UserService)marketSession.GetUserService();
            deletedUserSession.EnterSystem();
            deletedUserSession.SignUp("DeleteMe", "no-where", "123","12345678");
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.RemoveUser("DeleteMe");
            deletedUserSession2 = (UserService)marketSession.GetUserService();
            deletedUserSession2.EnterSystem();
            Assert.AreEqual((int)SignInStatus.NoUserFound ,deletedUserSession2.SignIn("DeleteMe", "123").Status);


        }

        [TestMethod]
        public void SignUpWithDeletedUserDataTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.RemoveUser(notAdminName);
            deletedUserSession2 = (UserService)marketSession.GetUserService();
            deletedUserSession2.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, deletedUserSession2.SignUp(notAdminName,"no-where" ,notAdminPass,"12345678").Status);
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
            deletedUserSession?.CleanSession();
            deletedUserSession2?.CleanSession();
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
