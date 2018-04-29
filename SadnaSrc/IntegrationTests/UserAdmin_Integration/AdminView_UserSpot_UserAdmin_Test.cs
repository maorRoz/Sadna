using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;
using SadnaSrc.MarketHarmony;
using SadnaSrc.AdminView;

namespace IntegrationTests.UserAdmin_Integration
{
    [TestClass]
    public class AdminView_UserSpot_UserAdmin_Test
    {

        private IUserService userServiceSession;
        private IUserService deletedUserSession;
        private IUserService deletedUserSession2;
        private IStoreShoppingService storeShoppingService;
        private IOrderService orderService;
        private SystemAdminService adminServiceSession;
        private UserAdminHarmony userAdminHarmony;
        private MarketYard marketSession;
        private string adminName = "Arik1";
        private string adminPass = "123";
        private string soleOwnerName = "Arik2";
        private string soleOwnerPass = "123";
        private string notSoleOwnerName = "Arik3";
        private string notSoleOwnerPass = "123";
        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userAdminHarmony = null;
            adminServiceSession = null;
            deletedUserSession = null;
            deletedUserSession2 = null;
            storeShoppingService = null;
            orderService = null;
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
            ToSignIn(soleOwnerName, soleOwnerPass);
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
            adminServiceSession.RemoveUser(soleOwnerName);
            deletedUserSession2 = (UserService)marketSession.GetUserService();
            deletedUserSession2.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, deletedUserSession2.SignUp(soleOwnerName, "no-where" , soleOwnerPass, "12345678").Status);
        }

        [TestMethod]
        public void StoreDidntCloseUponUserRemovalTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(notSoleOwnerName).Status);
            storeShoppingService = marketSession.GetStoreShoppingService(ref userServiceSession);
            Assert.AreEqual((int)StoreEnum.Success, storeShoppingService.AddProductToCart("X", "BOX", 3).Status);

        }


        [TestMethod]
        public void StoreClosedUponUserRemovalTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)RemoveUserStatus.Success, adminServiceSession.RemoveUser(soleOwnerName).Status);
            storeShoppingService = marketSession.GetStoreShoppingService(ref userServiceSession);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, storeShoppingService.AddProductToCart("Y", "BOX", 3).Status);
        }


        [TestMethod]
        public void UserReportTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            storeShoppingService = marketSession.GetStoreShoppingService(ref userServiceSession);
            Assert.AreEqual((int)StoreEnum.Success, storeShoppingService.AddProductToCart("The Red Rock", "Goldstar", 5).Status);
            orderService = marketSession.GetOrderService(ref userServiceSession);
            Assert.AreEqual((int)OrderStatus.Success, orderService.BuyEverythingFromCart(new string[] {null}).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByUser("Arik1").Status);
            string[] actualHistory = adminServiceSession.ViewPurchaseHistoryByUser("Arik1").ReportList;
            string[] expectedHistory =
            {
                new PurchaseHistory("Arik1", "Goldstar","The Red Rock", "Immediate",5,55, DateTime.Now.ToShortDateString()).ToString(),
                new PurchaseHistory("Arik1", "Health Potion", "X", "Immediate",2,11.5, "Today").ToString(),
                new PurchaseHistory("Arik1", "INT Potion","Y", "Lottery",2,8.0, "Yesterday").ToString(),
                new PurchaseHistory("Arik1", "Mana Potion", "Y", "Lottery",3,12.0, "Yesterday").ToString()
            };
            Assert.AreEqual(expectedHistory.Length, actualHistory.Length);
            for (int i = 0; i < expectedHistory.Length; i++)
            {
                Assert.AreEqual(expectedHistory[i], actualHistory[i]);
            }
        }

        [TestMethod]
        public void StoreReportTest()
        {
            ToSignIn(adminName, adminPass);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            storeShoppingService = marketSession.GetStoreShoppingService(ref userServiceSession);
            Assert.AreEqual((int)StoreEnum.Success, storeShoppingService.AddProductToCart("X", "BOX", 3).Status);
            orderService = marketSession.GetOrderService(ref userServiceSession);
            Assert.AreEqual((int)OrderStatus.Success, orderService.BuyEverythingFromCart(new string[] {null}).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByUser("Arik1").Status);
            string[] actualHistory = adminServiceSession.ViewPurchaseHistoryByStore("X").ReportList;
            string[] expectedHistory =
            {
                new PurchaseHistory("Arik1", "Health Potion","X", "Immediate",2,11.5, "Today").ToString(),
                new PurchaseHistory("Arik1", "BOX", "X", "Immediate", 3, 300, DateTime.Now.ToShortDateString()).ToString(),
            };
            Assert.AreEqual(expectedHistory.Length,actualHistory.Length);
            for (int i = 0; i < expectedHistory.Length; i++)
            {
                Assert.AreEqual(expectedHistory[i],actualHistory[i]);
            }
        }




        [TestCleanup]
        public void UserAdminTestCleanUp()
        {
            userServiceSession.CleanSession();
            deletedUserSession?.CleanSession();
            deletedUserSession2?.CleanSession();
            storeShoppingService?.CleanSeesion();
            orderService?.CleanSession();
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
