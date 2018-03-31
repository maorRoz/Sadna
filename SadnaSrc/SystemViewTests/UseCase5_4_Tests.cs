using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SystemViewTests
{

    [TestClass]
    public class UseCase5_4_Tests
    {
        private SystemAdminService adminServiceSession;
        private UserService userServiceSession;
        private MarketYard marketSession;
        private string adminName = "Arik1";
        private string adminPass = "123";
        private string userNameToView = "Moshe";
        private string noUserName = "MosheXXX";
        private string storeNameToView = "YYY";
        private string noStoreName = "adasdadasdadadasdasdasd";
        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = new MarketYard();
            userServiceSession = (UserService)marketSession.GetUserService();
        }

        [TestMethod]
        public void PurchaseHistoryOfUser()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).Status);
            PurchaseHistory[] expectedHistory = 
            {
                new PurchaseHistory(userNameToView, "Health Potion", "XXX", "Immediate", "Today"),
                new PurchaseHistory(userNameToView, "Mana Potion", storeNameToView, "Lottery", "Yesterday"),
                new PurchaseHistory(userNameToView, "INT Potion", storeNameToView, "Lottery", "Yesterday"),

            };
            Assert.IsTrue(adminServiceSession.LastHistoryReport.SequenceEqual(expectedHistory));
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void PurchaseHistoryOfStore()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).Status);
            PurchaseHistory[] expectedHistory =
            {
                new PurchaseHistory(userNameToView, "Mana Potion", storeNameToView, "Lottery", "Yesterday"),
                new PurchaseHistory(userNameToView, "INT Potion", storeNameToView, "Lottery", "Yesterday"),
                new PurchaseHistory("MosheYYY", "STR Potion", storeNameToView, "Immediate", "Today"),

            };
            Assert.IsTrue(adminServiceSession.LastHistoryReport.SequenceEqual(expectedHistory));
            Assert.IsFalse(MarketException.hasErrorRaised());
        }
        [TestMethod]
        public void DidntEnterTest()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(noUserName).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(noStoreName).Status);
        }

        [TestMethod]
        public void DidntLoggedTest()
        {
            userServiceSession.EnterSystem();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(noUserName).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(noStoreName).Status);
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSession.SignIn("Arik2", "123").Status);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(noUserName).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(noStoreName).Status);
        }

        [TestMethod]
        public void NoUserNameTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoUserFound, adminServiceSession.ViewPurchaseHistoryByUser(noUserName).Status);
            Assert.IsTrue(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void NoStoreNameTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoStoreFound, adminServiceSession.ViewPurchaseHistoryByStore(noStoreName).Status);
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
