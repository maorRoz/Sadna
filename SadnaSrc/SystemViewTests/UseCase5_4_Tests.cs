using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace SystemViewTests
{

    [TestClass]
    public class UseCase5_4_Tests
    {
        private SystemAdminService adminServiceSession;
        private IUserService userServiceSession;
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
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
        }

        [TestMethod]
        public void PurchaseHistoryOfUserTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            string[] expectedHistory = 
            {
                new PurchaseHistory(userNameToView, "Health Potion", "XXX", "Immediate",2,11.5, "Today").ToString(),
                new PurchaseHistory(userNameToView, "INT Potion", storeNameToView, "Lottery",2,8.0, "Yesterday").ToString(),
                new PurchaseHistory(userNameToView, "Mana Potion", storeNameToView, "Lottery",3,12.0, "Yesterday").ToString(),

            };
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).Status);
            Assert.IsTrue(adminServiceSession.ViewPurchaseHistoryByUser(userNameToView).ReportList.SequenceEqual(expectedHistory));
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void PurchaseHistoryOfStoreTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            string[] expectedHistory =
            {
                new PurchaseHistory(userNameToView, "Mana Potion", storeNameToView, "Lottery",3,12.0, "Yesterday").ToString(),
                new PurchaseHistory(userNameToView, "INT Potion", storeNameToView, "Lottery",2,8.0, "Yesterday").ToString(),
                new PurchaseHistory("MosheYYY", "STR Potion", storeNameToView, "Immediate",1,4.0, "Today").ToString(),

            };
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).Status);
            Assert.IsTrue(adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView).ReportList.SequenceEqual(expectedHistory));
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
            MarketYard.CleanSession();
    
        }

        private void DoSignInToAdmin()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(adminName, adminPass);
        }

    }
}
