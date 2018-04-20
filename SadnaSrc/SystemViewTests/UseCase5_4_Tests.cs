using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.MarketHarmony;
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
        private string userNameToView1 = "Arik1";
        private string userNameToView2 = "Arik3";
        private string noUserName = "MosheXXX";
        private string storeNameToView1 = "X";
        private string storeNameToView2 = "Y";
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
                new PurchaseHistory(userNameToView1, "Health Potion", storeNameToView1, "Immediate",2,11.5, "Today").ToString(),
                new PurchaseHistory(userNameToView1, "INT Potion", storeNameToView2, "Lottery",2,8.0, "Yesterday").ToString(),
                new PurchaseHistory(userNameToView1, "Mana Potion", storeNameToView2, "Lottery",3,12.0, "Yesterday").ToString(),

            };
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView1).Status);
            Assert.IsTrue(adminServiceSession.ViewPurchaseHistoryByUser(userNameToView1).ReportList.SequenceEqual(expectedHistory));
            Assert.IsFalse(MarketException.hasErrorRaised());
        }

        [TestMethod]
        public void PurchaseHistoryOfStoreTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            string[] expectedHistory =
            {
                new PurchaseHistory(userNameToView1, "Mana Potion", storeNameToView2, "Lottery",3,12.0, "Yesterday").ToString(),
                new PurchaseHistory(userNameToView1, "INT Potion", storeNameToView2, "Lottery",2,8.0, "Yesterday").ToString(),
                new PurchaseHistory(userNameToView2, "STR Potion", storeNameToView2, "Immediate",1,4.0, "Today").ToString(),

            };
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView2).Status);
            Assert.IsTrue(adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView2).ReportList.SequenceEqual(expectedHistory));
            Assert.IsFalse(MarketException.hasErrorRaised());
        }
        [TestMethod]
        public void DidntEnterTest()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView1).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView2).Status);
        }

        [TestMethod]
        public void DidntLoggedTest()
        {
            userServiceSession.EnterSystem();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView1).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView2).Status);
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userServiceSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSession.SignIn("Arik2", "123").Status);
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByUser(userNameToView1).Status);
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NotSystemAdmin, adminServiceSession.ViewPurchaseHistoryByStore(storeNameToView2).Status);
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
